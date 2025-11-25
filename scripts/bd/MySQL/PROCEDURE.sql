
USE 5to_SistemaDeBoleteria;

-- DELIMITER $$
-- DROP PROCEDURE IF EXISTS ReporteVentas $$
-- CREATE PROCEDURE ReporteVentas()
-- BEGIN
--     START TRANSACTION;
--     SELECT 
--         E.Nombre AS NombreEvento,
--         COUNT(EN.IdEntrada) AS EntradasVendidas,
--         SUM(T.Precio) AS TotalRecaudado
--     FROM Entrada EN
--     JOIN Orden O ON EN.IdOrden = O.IdOrden
--     JOIN Funcion F ON F.IdFuncion = O.IdFuncion
--     JOIN Evento E ON E.IdEvento = F.IdEvento
--     JOIN Tarifa T ON T.IdTarifa = O.IdTarifa
--     WHERE EN.Anulada = FALSE
--     GROUP BY E.IdEvento, E.Nombre;
--     COMMIT;

-- END$$
--========================== FUNCION ================================================
--===================================================================================

--========================== EVENTO =================================================

-- DROP PROCEDURE IF EXISTS PublicarEvento $$
-- CREATE PROCEDURE PublicarEvento(IN unIdEvento INT)
-- BEGIN
--     DECLARE EXIT HANDLER FOR SQLEXCEPTION
--     BEGIN
--         SIGNAL SQLSTATE '45000'
--         SET MESSAGE_TEXT = 'Error al publicar evento';
--     END;
--     START TRANSACTION;
--         UPDATE Evento
--         SET Estado = 'Publicado'
--         WHERE IdEvento = unIdEvento;
        
--         UPDATE Funcion
--         SET Cancelado = FALSE
--         WHERE IdEvento = unIdEvento;
--     COMMIT;
-- END$$

-- DROP PROCEDURE IF EXISTS CancelarEvento $$
-- CREATE PROCEDURE CancelarEvento(IN unIdEvento INT) 
-- BEGIN
--     DECLARE EXIT HANDLER FOR SQLEXCEPTION
--     BEGIN
--         SIGNAL SQLSTATE '45000'
--         SET MESSAGE_TEXT = 'Error al cancelar evento';
--     END;
--     START TRANSACTION;

--         UPDATE Entrada
--         SET Estado = 'Anulado'
--         WHERE IdOrden IN (SELECT IdOrden 
--                          FROM Orden O 
--                          JOIN Funcion F USING (IdFuncion) 
--                          WHERE F.IdEvento = unIdEvento);

--         UPDATE Tarifa
--         SET Estado = 'Suspendida'
--         WHERE IdFuncion IN (SELECT IdFuncion 
--                            FROM Funcion 
--                            WHERE IdEvento = unIdEvento);
--         UPDATE Funcion
--         SET Cancelado = TRUE
--         WHERE IdEvento = unIdEvento;

--         UPDATE Evento
--         SET Estado = 'Cancelado'
--         WHERE IdEvento = unIdEvento;
--     COMMIT;
-- END$$

--===================================================================================

-- ====================================== ORDEN ===========================================================

-- DROP PROCEDURE IF EXISTS `AltaOrden`;

-- CREATE PROCEDURE AltaOrden(OUT unIdOrden INT, unIdTarifa INT, unIdCliente INT, unIdFuncion INT, unMedioDePago ENUM('Efectivo','Transferencia','Debito','Credito'))
-- BEGIN
--     DECLARE EXIT HANDLER FOR SQLEXCEPTION
--     BEGIN
--         SIGNAL SQLSTATE '45000'
--         SET MESSAGE_TEXT = 'Error al dar de alta la Orden. Revertiendo cambios';
--     END;

--     START TRANSACTION;
--         INSERT INTO Orden (IdTarifa, IdCliente, IdFuncion, Emision, Cierre, MedioDePago)
--         VALUES (unIdTarifa, unIdCliente, unIdFuncion, NOW(), DATE_ADD(NOW(), INTERVAL 15 MINUTE), unMedioDePago );

--         SET unIdOrden = LAST_INSERT_ID();

--         -- Reserva Stock
--         UPDATE Tarifa
--         SET Stock = Stock - 1
--         WHERE IdFuncion = unIdFuncion AND Estado = 'Activa' AND IdTarifa = unIdTarifa;
--     COMMIT;
-- END$$

-- DROP PROCEDURE IF EXISTS `PagarOrden`;

-- CREATE PROCEDURE PagarOrden (IN unIdOrden INT)
-- BEGIN
--     DECLARE unTipoEntrada VARCHAR(60);
--     DECLARE unIdEntrada INT;

--     DECLARE EXIT HANDLER FOR SQLEXCEPTION
--     BEGIN
--         SIGNAL SQLSTATE '45000'
--         SET MESSAGE_TEXT = 'Ocurrio un problema, la orden no pudo ser abonada. Revertiendo cambios';
--     END;

--     START TRANSACTION;
--         SELECT TipoEntrada INTO unTipoEntrada
--         FROM Tarifa
--         JOIN Orden USING (IdTarifa)
--         WHERE IdOrden = unIdOrden;

--         UPDATE Orden
--         SET Estado = 'Abonado',
--             Cierre = NOW()
--         WHERE IdOrden = unIdOrden;

--         INSERT INTO Entrada (IdOrden, TipoEntrada, Emision, Liquidez)
--         VALUES (unIdOrden, unTipoEntrada, NOW(), DATE_ADD(NOW(), INTERVAL 1 DAY));

--         SET unIdEntrada = LAST_INSERT_ID();

--         INSERT INTO QR (IdEntrada, Codigo)
--         VALUES (unIdEntrada, CONCAT('QR-', UUID()));
--     COMMIT;
-- END$$


-- DROP PROCEDURE IF EXISTS CancelarOrden $$
-- CREATE PROCEDURE CancelarOrden (unIdOrden INT)
-- BEGIN
--     DECLARE EXIT HANDLER FOR SQLEXCEPTION
--     BEGIN
--         ROLLBACK;
--         SIGNAL SQLSTATE '45000'
--         SET MESSAGE_TEXT = 'No se pudo cancelar la orden. Revertiendo cambios.';
--     END;
--     START TRANSACTION;
--         UPDATE Orden
--         SET Estado = 'Cancelado',
--             Cierre = NOW()
--         WHERE IdOrden = unIdOrden;

--         -- Devuelve la reserva del Stock
--         UPDATE Tarifa
--         SET Stock = Stock + 1 
--         WHERE IdFuncion = (SELECT IdFuncion
--                            FROM Orden
--                            WHERE IdOrden = unIdOrden);
--     COMMIT;
-- END$$


--=========================================================================================================


--====================================== ENTRADA ==========================================================


-- DROP PROCEDURE IF EXISTS CancelarEntrada $$
-- CREATE PROCEDURE CancelarEntrada(IN unIdEntrada INT)
-- BEGIN
--     DECLARE unIdFuncion INT;

--     IF EXISTS (SELECT 1 FROM Entrada WHERE IdEntrada = unIdEntrada AND Anulada = FALSE) THEN
        
--         UPDATE Entrada
--         SET Estado = 'Anulado'
--         WHERE IdEntrada = unIdEntrada;

--         UPDATE QR
--         SET TipoEstado = 'YaUsada'
--         WHERE IdEntrada = unIdEntrada;

--         SELECT F.IdFuncion INTO unIdFuncion
--         FROM Funcion F
--         JOIN Orden O ON F.IdSesion = O.IdSesion
--         JOIN Entrada E ON E.IdOrden = O.IdOrden
--         WHERE E.IdEntrada = unIdEntrada;

--         UPDATE Tarifa
--         SET Stock = Stock + 1
--         WHERE IdFuncion = unIdFuncion;

--     ELSE
--         SIGNAL SQLSTATE '45000'
--         SET MESSAGE_TEXT = 'La entrada no existe o ya fue anulada.';
--     END IF;
--     COMMIT;
-- END$$

--========================================================================================================