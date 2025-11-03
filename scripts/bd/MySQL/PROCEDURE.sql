
USE 5to_sistemadeboleteria;

DELIMITER $$
CREATE PROCEDURE ReporteVentas()
BEGIN
    START TRANSACTION;
    SELECT 
        E.Nombre AS NombreEvento,
        COUNT(EN.IdEntrada) AS EntradasVendidas,
        SUM(T.Precio) AS TotalRecaudado
    FROM Entrada EN
    JOIN Orden O ON EN.IdOrden = O.IdOrden
    JOIN Funcion F ON F.IdFuncion = O.IdFuncion
    JOIN Evento E ON E.IdEvento = F.IdEvento
    JOIN Tarifa T ON T.IdTarifa = O.IdTarifa
    WHERE EN.Anulada = FALSE
    GROUP BY E.IdEvento, E.Nombre;
    COMMIT;

END$$

--========================== EVENTO =================================================

CREATE PROCEDURE PublicarEvento(IN unIdEvento INT)
BEGIN
START TRANSACTION;
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
        
        UPDATE Funcion
        SET Cancelado = FALSE
        WHERE IdEvento = unIdEvento;

        COMMIT;
    ELSE
        ROLLBACK;
        SIGNAL SQLSTATE '45000' 
        SET MESSAGE_TEXT = 'El evento no puede publicarse: no tiene funciones o tarifas activas.';
    END IF;
END$$

--===================================================================================

-- ====================================== ORDEN ===========================================================

DROP PROCEDURE `AltaOrden`;

CREATE PROCEDURE AltaOrden(OUT unIdOrden INT, unIdTarifa INT, unIdCliente INT, unIdFuncion INT, unMedioDePago ENUM('Efectivo','Transferencia','Debito','Credito'))
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Error al dar de alta la Orden. Revertiendo cambios';
    END;

    START TRANSACTION;
        INSERT INTO Orden (IdTarifa, IdCliente, IdFuncion, Emision, Cierre, MedioDePago)
        VALUES (unIdTarifa, unIdCliente, unIdFuncion, NOW(), DATE_ADD(NOW(), INTERVAL 15 MINUTE), unMedioDePago );

        SET unIdOrden = LAST_INSERT_ID();

        -- Reserva Stock
        UPDATE Tarifa
        SET Stock = Stock - 1
        WHERE IdFuncion = unIdFuncion AND Estado = 'Activa' AND IdTarifa = unIdTarifa;
    COMMIT;
END$$

DROP PROCEDURE `PagarOrden`;

CREATE PROCEDURE PagarOrden (unIdOrden INT)
BEGIN
    DECLARE unTipoEntrada ENUM('General', 'VIP', 'PLUS');
    DECLARE unIdEntrada INT;
    DECLARE EstadoDelOrden VARCHAR(20);

    START TRANSACTION;
        SELECT T.TipoEntrada, O.Estado INTO unTipoEntrada, EstadoDelOrden
        FROM Orden O
        JOIN Tarifa T USING (IdTarifa)
        WHERE O.IdOrden = unIdOrden;

        IF EstadoDelOrden = 'Cancelado'
        THEN
            ROLLBACK;
            SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'No se puede pagar una orden que se encuentra Cancelada';
        END IF;
        
        IF EstadoDelOrden = 'Abonado'
        THEN
            ROLLBACK;
            SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'No se puede pagar la orden porque ya se encuentra pagada';
        END IF;

        IF (SELECT Cierre FROM Orden WHERE IdOrden = unIdOrden)< NOW()
        THEN
            ROLLBACK;
            SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Ya pasaron los 15 min habiles para pagar la orden.';
        END IF;

        IF EstadoDelOrden = 'Creado'
        THEN
            UPDATE Orden
            SET Estado = 'Abonado',
                Cierre = NOW()
            WHERE IdOrden = unIdOrden;
        ELSE
            ROLLBACK;
            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Ocurrio un problema, la orden no pudo ser abonada. Revertiendo cambios';
        END IF;

        INSERT INTO Entrada (IdOrden, TipoEntrada, Emision, Liquidez)
        VALUES (unIdOrden, unTipoEntrada, NOW(), DATE_ADD(NOW(), INTERVAL 1 DAY));

        SET unIdEntrada = LAST_INSERT_ID();

        INSERT INTO QR (IdEntrada, Codigo)
        VALUES (unIdEntrada, CONCAT('QR-', UUID()));
    COMMIT;
END$$

CREATE PROCEDURE CancelarOrden (unIdOrden INT)
BEGIN
    DECLARE unIdFuncion INT;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'No se pudo cancelar la orden';
    END;
    START TRANSACTION;
        SELECT IdFuncion INTO unIdFuncion
        FROM Orden
        WHERE IdOrden = unIdOrden;

        UPDATE Orden
        SET Estado = 'Cancelado',
            Cierre = NOW()
        WHERE IdOrden = unIdOrden;

        -- Devuelve la reserva de Stock
        UPDATE Tarifa
        SET Stock = Stock + 1 
        WHERE IdFuncion = unIdFuncion;
    COMMIT;
END$$
--=========================================================================================================


--====================================== ENTRADA ==========================================================
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

-- CREATE PROCEDURE ComprarEntradaUno(
--     IN unIdCliente INT,
--     IN unIdFuncion INT,
--     IN tipoEntrada ENUM('General','VIP','PLUS'),
--     IN medioPago ENUM('Efectivo','Transferencia','Debito','Credito')
-- )
-- BEGIN
--     START TRANSACTION
--         DECLARE unIdSesion INT;
--         DECLARE unIdOrden INT;
--         DECLARE unIdEntrada INT;
--         DECLARE precio DECIMAL(10,2);
        
--         SELECT IdSesion INTO unIdSesion
--         FROM Funcion
--         WHERE IdFuncion = unIdFuncion;

    
--         SELECT Precio INTO precio
--         FROM Tarifa
--         WHERE IdFuncion = unIdFuncion AND Estado = 'Activa'
--         LIMIT 1;

--         INSERT INTO Orden (IdTarifa, IdSesion, IdCliente, Estado, Emision, Cierre, MedioDePago)
--         VALUES (
--             (SELECT IdTarifa FROM Tarifa WHERE IdFuncion = unIdFuncion AND Estado = 'Activa' LIMIT 1),
--             unIdSesion,
--             unIdCliente,
--             'Abonado',
--             NOW(),
--             DATE_ADD(NOW(), INTERVAL 1 DAY),
--             medioPago
--         );

--         SET unIdOrden = LAST_INSERT_ID();

--         INSERT INTO Entrada (IdOrden, TipoEntrada, Emision, Liquidez)
--         VALUES (unIdOrden, tipoEntrada, NOW(), DATE_ADD(NOW(), INTERVAL 1 DAY));

--         SET unIdEntrada = LAST_INSERT_ID();

    
--         INSERT INTO QR (IdEntrada, TipoEstado, Codigo)
--         VALUES (unIdEntrada, 'Ok', CONCAT('QR-', UUID()));

    
--         UPDATE Tarifa
--         SET Stock = Stock - 1
--         WHERE IdFuncion = unIdFuncion AND Estado = 'Activa';
--     COMMIT;

-- END$$

-- CREATE PROCEDURE ComprarEntradaDos(IN unIdOrden INT UNSIGNED, IN unTipoEntrada) --BORRADOR 
-- BEGIN
--         INSERT INTO Entrada (IdOrden, TipoEntrada, Emision, Liquidez)
--         VALUES (unIdOrden, unTipoEntrada, NOW(), DATE_ADD(NOW(), INTERVAL 1 DAY));

--         SET unIdEntrada = LAST_INSERT_ID();
    
--         INSERT INTO QR (IdEntrada, TipoEstado, Codigo)
--         VALUES (unIdEntrada, 'Ok', CONCAT('QR-', UUID()));

--         -- Esto va para la creacion de la orden
--         UPDATE Tarifa
--         SET Stock = Stock - 1
--         WHERE IdFuncion = unIdFuncion AND Estado = 'Activa';
--         --
-- END$$

--========================================================================================================
DELIMITER ;
