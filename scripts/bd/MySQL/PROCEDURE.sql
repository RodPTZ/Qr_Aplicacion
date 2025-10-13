CREATE PROCEDURE ValidarQR( unIdEntrada INT)
BEGIN
    DECLARE Ahora TIME DEFAULT CAST(NOW());
    DECLARE esHoy BOOL DEFAULT FALSE;
    DECLARE dentroDelHorario BOOL DEFAULT FALSE;

    SET @Apertura = (SELECT Apertura FROM Sesion WHERE IdSesion = (SELECT O.IdSesion FROM Orden O JOIN Entrada USING (IdOrden)
    WHERE IdEntrada = unIdEntrada));
    
    SET @Cierre = (SELECT Cierre FROM Sesion WHERE IdSesion = (SELECT O.IdSesion FROM Orden O JOIN Entrada USING (IdOrden)
    WHERE IdEntrada = unIdEntrada));

    IF((SELECT Liquidez FROM Entrada WHERE IdEntrada = unIdEntrada) = NOW())
    THEN 
        SET esHoy = TRUE;
    END IF;

    IF((SELECT Anulada FROM Entrada WHERE IdEntrada = unIdEntrada) IS TRUE)
    THEN
        UPDATE QR
        SET TipoEstado = 'FirmaInvalida'
        WHERE IdEntrada = unIdEntrada;
    END IF;

    IF(Ahora >= @Apertura AND Ahora <= @Cierre)
    THEN
        SET dentroDelHorario = TRUE
    END IF;
 
    IF(esHoy)
    THEN
        IF(dentroDelHorario)
        THEN
            IF((SELECT TipoEstado FROM QR WHERE IdEntrada = unIdEntrada) = 'Ok')
            THEN
                UPDATE QR
                SET TipoEstado = 'YaUsada'
                WHERE IdEntrada = unIdEntrada;
            ELSE IF((SELECT TipoEstado FROM QR WHERE IdEntrada = unIdEntrada) != 'YaUsada')
                UPDATE QR
                SET TipoEstado = 'Ok'
                WHERE IdEntrada = unIdEntrada;
            END IF;
        ELSE
            UPDATE QR
            SET TipoEstado = 'FirmaInvalida'
            WHERE IdEntrada= unIdEntrada;
        END IF;
    ELSE IF((SELECT Liquidez FROM Entrada WHERE IdEntrada = unIdEntrada)< DATE.NOW())
        UPDATE QR
        SET TipoEstado = 'Expirada'
        WHERE IdEntrada = unIdEntrada;
    END IF;
END;
CREATE PROCEDURE ReporteVentas()
BEGIN
    SELECT E.Nombre AS Evento,
           COUNT(EN.IdEntrada) AS EntradasVendidas,
           SUM(T.Precio) AS TotalRecaudado
    FROM Entrada EN
    JOIN Orden O ON EN.IdOrden = O.IdOrden
    JOIN Sesion S ON S.IdSesion = O.IdSesion
    JOIN Evento E ON E.IdEvento = S.IdEvento
    JOIN Funcion F ON F.IdEvento = E.IdEvento
    JOIN Tarifa T ON T.IdFuncion = F.IdFuncion
    WHERE EN.Anulada = FALSE
    GROUP BY E.IdEvento;
END;
            UPDATE QR
            SET TipoEstado = 'FirmaInvalida'
            WHERE IdEntrada= unIdEntrada;
        END IF;
    ELSE IF((SELECT Liquidez FROM Entrada WHERE IdEntrada = unIdEntrada)< DATE.NOW())
        UPDATE QR
        SET TipoEstado = 'Expirada'
        WHERE IdEntrada = unIdEntrada;
    END IF;
END;
CREATE PROCEDURE ComprarEntrada(
    IN unIdCliente INT,
    IN unIdFuncion INT,
    IN tipo ENUM('General','VIP','Plus'),
    IN medioPago VARCHAR(30)
)
BEGIN
    DECLARE unIdSesion INT;
    DECLARE unIdOrden INT;
    DECLARE unIdEntrada INT;
    DECLARE unIdQR INT;
    DECLARE precio DECIMAL(10,2);

    SET unIdSesion = (SELECT IdSesion FROM Funcion WHERE IdFuncion = unIdFuncion);

    SET precio = (SELECT Precio FROM Tarifa WHERE IdFuncion = unIdFuncion AND Estado = TRUE LIMIT 1);

   
    INSERT INTO Orden (IdSesion, IdCliente, TipoEntrada, Emision, Cierre, MedioDePago)
    VALUES (unIdSesion, unIdCliente, tipo, NOW(), DATE_ADD(NOW(), INTERVAL 1 DAY), medioPago);

    SET unIdOrden = LAST_INSERT_ID();

    
    INSERT INTO Entrada (IdOrden, TipoEntrada, Emision, Liquidez)
    VALUES (unIdOrden, tipo, NOW(), DATE_ADD(NOW(), INTERVAL 1 DAY));

    SET unIdEntrada = LAST_INSERT_ID();

    
    INSERT INTO QR (IdEntrada, TipoEstado, Código)
    VALUES (unIdEntrada, 'Ok', CONCAT('QR-', UUID()));

    UPDATE Tarifa
    SET Stock = Stock - 1
    WHERE IdFuncion = unIdFuncion AND Estado = TRUE;
END;
CREATE PROCEDURE PublicarEvento(IN unIdEvento INT)
BEGIN
    IF EXISTS (SELECT * FROM Funcion WHERE IdEvento = unIdEvento)
       AND EXISTS (SELECT * FROM Tarifa T JOIN Funcion F ON F.IdFuncion=T.IdFuncion WHERE F.IdEvento=unIdEvento)
    THEN
        UPDATE Evento
        SET publicado = TRUE
        WHERE IdEvento = unIdEvento;
    ELSE
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El evento no puede publicarse: no tiene funciones o tarifas activas.';
    END IF;
END;

