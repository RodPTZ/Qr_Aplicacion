DELIMITER $$
CREATE PROCEDURE ReporteVentas()
BEGIN
    START TRANSACTION
    SELECT 
        E.Nombre AS NombreEvento,
        COUNT(EN.IdEntrada) AS EntradasVendidas,
        SUM(T.Precio) AS TotalRecaudado
    FROM Entrada EN
    JOIN Orden O ON EN.IdOrden = O.IdOrden
    JOIN Sesion S ON S.IdSesion = O.IdSesion
    JOIN Evento E ON E.IdEvento = S.IdEvento
    JOIN Funcion F ON F.IdEvento = E.IdEvento
    JOIN Tarifa T ON T.IdFuncion = F.IdFuncion
    WHERE EN.Anulada = FALSE
    GROUP BY E.IdEvento, E.Nombre;
    COMMIT;

END$$

CREATE PROCEDURE ComprarEntrada(
    IN unIdCliente INT,
    IN unIdFuncion INT,
    IN tipoEntrada ENUM('General','VIP','PLUS'),
    IN medioPago ENUM('Efectivo','Transferencia','Debito','Credito')
)
BEGIN
    START TRANSACTION
        DECLARE unIdSesion INT;
        DECLARE unIdOrden INT;
        DECLARE unIdEntrada INT;
        DECLARE precio DECIMAL(10,2);
        
        SELECT IdSesion INTO unIdSesion
        FROM Funcion
        WHERE IdFuncion = unIdFuncion;

    
        SELECT Precio INTO precio
        FROM Tarifa
        WHERE IdFuncion = unIdFuncion AND Estado = 'Activa'
        LIMIT 1;

        INSERT INTO Orden (IdTarifa, IdSesion, IdCliente, Estado, Emision, Cierre, MedioDePago)
        VALUES (
            (SELECT IdTarifa FROM Tarifa WHERE IdFuncion = unIdFuncion AND Estado = 'Activa' LIMIT 1),
            unIdSesion,
            unIdCliente,
            'Abonado',
            NOW(),
            DATE_ADD(NOW(), INTERVAL 1 DAY),
            medioPago
        );

        SET unIdOrden = LAST_INSERT_ID();

        INSERT INTO Entrada (IdOrden, TipoEntrada, Emision, Liquidez)
        VALUES (unIdOrden, tipoEntrada, NOW(), DATE_ADD(NOW(), INTERVAL 1 DAY));

        SET unIdEntrada = LAST_INSERT_ID();

    
        INSERT INTO QR (IdEntrada, TipoEstado, Codigo)
        VALUES (unIdEntrada, 'Ok', CONCAT('QR-', UUID()));

    
        UPDATE Tarifa
        SET Stock = Stock - 1
        WHERE IdFuncion = unIdFuncion AND Estado = 'Activa';
    COMMIT;

END$$


CREATE PROCEDURE PublicarEvento(IN unIdEvento INT)
BEGIN
START TRANSACTION
    IF EXISTS (SELECT 1 FROM Funcion WHERE IdEvento = unIdEvento)
       AND EXISTS (
            SELECT 1
            FROM Tarifa T
            JOIN Funcion F ON F.IdFuncion = T.IdFuncion
            WHERE F.IdEvento = unIdEvento AND T.Estado = 'Activa'
       )
    THEN
        UPDATE Evento
        SET Estado = 'Publicado'
        WHERE IdEvento = unIdEvento;
         COMMIT;
    ELSE
    ROLLBACK
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El evento no puede publicarse: no tiene funciones o tarifas activas.';
    END IF;
END$$


CREATE TRIGGER trg_CancelarFuncionesEvento
AFTER UPDATE ON Evento
FOR EACH ROW
BEGIN
    IF NEW.Estado = 'Cancelado' AND OLD.Estado <> 'Cancelado' THEN
        UPDATE Funcion
        SET Cancelado = TRUE
        WHERE IdEvento = NEW.IdEvento;

        UPDATE Tarifa
        SET Estado = 'Suspendida'
        WHERE IdFuncion IN (SELECT IdFuncion FROM Funcion WHERE IdEvento = NEW.IdEvento);
    END IF;
    COMMIT;,
END$$



CREATE PROCEDURE CancelarEntrada(IN unIdEntrada INT)
BEGIN
    DECLARE unIdFuncion INT;

    IF EXISTS (SELECT 1 FROM Entrada WHERE IdEntrada = unIdEntrada AND Anulada = FALSE) THEN
        
        UPDATE Entrada
        SET Anulada = TRUE
        WHERE IdEntrada = unIdEntrada;

        UPDATE QR
        SET TipoEstado = 'YaUsada'
        WHERE IdEntrada = unIdEntrada;

        SELECT F.IdFuncion INTO unIdFuncion
        FROM Funcion F
        JOIN Orden O ON F.IdSesion = O.IdSesion
        JOIN Entrada E ON E.IdOrden = O.IdOrden
        WHERE E.IdEntrada = unIdEntrada;

        UPDATE Tarifa
        SET Stock = Stock + 1
        WHERE IdFuncion = unIdFuncion;

    ELSE
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'La entrada no existe o ya fue anulada.';
    END IF;
    COMMIT;
END$$

DELIMITER ;
