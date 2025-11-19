USE 5to_SistemaDeBoleteria;
DELIMITER $$

-- ===========================
-- ReporteVentas
-- ===========================
DROP PROCEDURE IF EXISTS ReporteVentas $$
CREATE PROCEDURE ReporteVentas()
BEGIN
    -- Reporte de entradas pagadas por evento con total recaudado
    SELECT 
        E.IdEvento,
        E.Nombre AS NombreEvento,
        COUNT(EN.IdEntrada) AS EntradasVendidas,
        IFNULL(SUM(T.Precio),0) AS TotalRecaudado
    FROM Entrada EN
    JOIN Orden O USING (IdOrden)
    JOIN Tarifa T USING (IdTarifa)
    JOIN Funcion F ON F.`IdFuncion` = T.`IdFuncion`
    JOIN Evento E USING (IdEvento)
    WHERE EN.Estado = 'Pagado'
    GROUP BY E.IdEvento, E.Nombre;
END$$

-- ===========================
-- PublicarEvento
-- ===========================
DROP PROCEDURE IF EXISTS PublicarEvento $$
CREATE PROCEDURE PublicarEvento(IN unIdEvento INT)
BEGIN
    DECLARE countFunciones INT DEFAULT 0;
    DECLARE countTarifasConStock INT DEFAULT 0;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Error al publicar evento. Revirtiendo.';
    END;

    START TRANSACTION;

    -- Verificar existencia del evento
    IF NOT EXISTS (SELECT 1 FROM Evento WHERE IdEvento = unIdEvento) THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Evento no existe.';
    END IF;

    -- Validar que no esté ya publicado
    IF (SELECT Estado FROM Evento WHERE IdEvento = unIdEvento) = 'Publicado' THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'El evento ya está publicado.';
    END IF;

    -- Verificar que tenga funciones afiliadas
    SELECT COUNT(*) INTO countFunciones
    FROM Funcion
    WHERE IdEvento = unIdEvento;

    IF countFunciones = 0 THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'No se puede publicar: el evento no posee funciones asociadas.';
    END IF;

    -- Verificar que existan tarifas activas con stock (>0) para las funciones del evento
    SELECT COUNT(*) INTO countTarifasConStock
    FROM Tarifa T
    JOIN Funcion F USING (IdFuncion)
    WHERE F.IdEvento = unIdEvento
      AND T.Stock > 0
      AND T.Estado = 'Activa';

    IF countTarifasConStock = 0 THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'No se puede publicar: no existen tarifas activas con stock para el evento.';
    END IF;

    -- Actualizar estado del evento y garantizar funciones no canceladas
    UPDATE Evento
    SET Estado = 'Publicado'
    WHERE IdEvento = unIdEvento;

    UPDATE Funcion
    SET Cancelado = FALSE
    WHERE IdEvento = unIdEvento;

    COMMIT;
END$$

-- ===========================
-- CancelarEvento
-- ===========================
DROP PROCEDURE IF EXISTS CancelarEvento $$
CREATE PROCEDURE CancelarEvento(IN unIdEvento INT)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Error al cancelar evento. Revirtiendo.';
    END;

    START TRANSACTION;

    -- Verificar existencia
    IF NOT EXISTS (SELECT 1 FROM Evento WHERE IdEvento = unIdEvento) THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Evento no existe.';
    END IF;

    -- Si ya está cancelado -> notificar
    IF (SELECT Estado FROM Evento WHERE IdEvento = unIdEvento) = 'Cancelado' THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'El evento ya se encuentra cancelado.';
    END IF;

    -- Cancelar funciones relacionadas
    UPDATE Funcion
    SET Cancelado = TRUE
    WHERE IdEvento = unIdEvento;

    -- Suspender tarifas de las funciones del evento
    UPDATE Tarifa
    SET Estado = 'Suspendida'
    WHERE IdFuncion IN (SELECT IdFuncion FROM Funcion WHERE IdEvento = unIdEvento);

    -- Cancelar órdenes relacionadas (si no están ya canceladas)
    UPDATE Orden
    SET Estado = 'Cancelado', Cierre = NOW()
    WHERE IdFuncion IN (SELECT IdFuncion FROM Funcion WHERE IdEvento = unIdEvento)
      AND Estado <> 'Cancelado';

    -- Anular entradas relacionadas
    UPDATE Entrada
    SET Estado = 'Anulado'
    WHERE IdOrden IN (
        SELECT IdOrden FROM Orden WHERE IdFuncion IN (SELECT IdFuncion FROM Funcion WHERE IdEvento = unIdEvento)
    );

    -- Marcar QR asociadas a esas entradas como 'NoExiste'
    UPDATE QR
    SET TipoEstado = 'NoExiste'
    WHERE IdEntrada IN (
        SELECT IdEntrada FROM Entrada WHERE IdOrden IN (
            SELECT IdOrden FROM Orden WHERE IdFuncion IN (SELECT IdFuncion FROM Funcion WHERE IdEvento = unIdEvento)
        )
    );

    -- Actualizar estado del evento
    UPDATE Evento
    SET Estado = 'Cancelado'
    WHERE IdEvento = unIdEvento;

    COMMIT;
END$$

-- ===========================
-- AltaOrden
-- OUT unIdOrden
-- ===========================
DROP PROCEDURE IF EXISTS AltaOrden $$
CREATE PROCEDURE AltaOrden(OUT unIdOrden INT, IN unIdTarifa INT, IN unIdCliente INT, IN unIdFuncion INT, IN unMedioDePago ENUM('Efectivo','Transferencia','Debito','Credito'))
BEGIN
    DECLARE funcionCancelada BOOLEAN DEFAULT FALSE;
    DECLARE eventoEstado VARCHAR(40);

    START TRANSACTION;

    -- Validaciones básicas: existencia y vínculo tarifa-función
    IF NOT EXISTS (SELECT * FROM Tarifa WHERE IdTarifa = unIdTarifa AND IdFuncion = unIdFuncion) THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Tarifa/Función no válida o no vinculada.';
    END IF;

    SELECT Cancelado INTO funcionCancelada FROM Funcion WHERE IdFuncion = unIdFuncion LIMIT 1;
    SELECT `Estado` INTO eventoEstado FROM Evento E JOIN Funcion F USING (IdEvento) WHERE F.IdFuncion = unIdFuncion LIMIT 1;

    IF(EXISTS (SELECT * FROM tarifa WHERE `IdTarifa` = unIdTarifa AND `Estado` = 'Suspendida' OR `Estado` = 'Agotada' OR `Estado` = 'Inactiva'))
    THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'La tarifa no está activa.';
    END IF;

    IF (eventoEstado = 'Cancelado') THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'No se puede crear orden: el evento está cancelado.';
    END IF;

    IF (funcionCancelada = 1) THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'No se puede crear orden: la función está cancelada.';
    END IF;

    IF (SELECT `Stock` FROM tarifa WHERE `IdTarifa` = unIdTarifa AND `Stock` <= 0) THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Stock insuficiente para la tarifa seleccionada.';
    END IF;

    -- Insertar orden (reserva)
    INSERT INTO Orden (IdTarifa, IdFuncion, IdCliente, Estado, Emision, Cierre, MedioDePago)
    VALUES (unIdTarifa, unIdFuncion, unIdCliente, 'Creado', NOW(), DATE_ADD(NOW(), INTERVAL 15 MINUTE), unMedioDePago);

    SET unIdOrden = LAST_INSERT_ID();

    -- Reservar stock (disminuir en 1) y actualizar estado si se agota
    UPDATE Tarifa
    SET Stock = Stock - 1
    WHERE IdTarifa = unIdTarifa;

    UPDATE Tarifa
    SET Estado = 'Agotada'
    WHERE IdTarifa = unIdTarifa AND Stock <= 0;

    COMMIT;
END$$

-- ===========================
-- PagarOrden
-- ===========================
DROP PROCEDURE IF EXISTS PagarOrden $$
CREATE PROCEDURE PagarOrden (IN unIdOrden INT)
BEGIN
    DECLARE estadoOrden VARCHAR(30);
    DECLARE idTarifa INT;
    DECLARE idFuncion INT;
    DECLARE tipoEntrada VARCHAR(20);
    DECLARE idEntrada INT;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Ocurrió un problema, la orden no pudo ser abonada. Revirtiendo cambios.';
    END;

    START TRANSACTION;

    -- Validar existencia de la orden
    IF NOT EXISTS (SELECT 1 FROM Orden WHERE IdOrden = unIdOrden) THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Orden inexistente.';
    END IF;

    SELECT Estado, IdTarifa, IdFuncion INTO estadoOrden, idTarifa, idFuncion FROM Orden WHERE IdOrden = unIdOrden LIMIT 1;

    IF estadoOrden = 'Abonado' THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'La orden ya está abonada.';
    END IF;

    -- Revisar que el evento y la función no estén cancelados
    IF (SELECT Estado FROM Evento E JOIN Funcion F USING (IdEvento) WHERE F.IdFuncion = idFuncion LIMIT 1) = 'Cancelado' THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'No se puede pagar: el evento está cancelado.';
    END IF;

    IF (SELECT Cancelado FROM Funcion WHERE IdFuncion = idFuncion LIMIT 1) = TRUE THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'No se puede pagar: la función está cancelada.';
    END IF;

    -- Obtener tipo de entrada desde la tarifa
    SELECT TipoEntrada INTO tipoEntrada FROM Tarifa WHERE IdTarifa = idTarifa LIMIT 1;

    -- Actualizar orden como abonada
    UPDATE Orden
    SET Estado = 'Abonado',
        Cierre = NOW()
    WHERE IdOrden = unIdOrden;

    -- Crear entrada pagada
    INSERT INTO Entrada (IdOrden, TipoEntrada, Emision, Liquidez, Estado)
    VALUES (unIdOrden, tipoEntrada, NOW(), DATE_ADD(NOW(), INTERVAL 1 DAY), 'Pagado');

    SET idEntrada = LAST_INSERT_ID();

    -- Crear QR asociado
    INSERT INTO QR (IdEntrada, Codigo, TipoEstado)
    VALUES (idEntrada, CONCAT('QR-', UUID()), 'Ok');

    COMMIT;
END$$

-- ===========================
-- CancelarOrden
-- ===========================
DROP PROCEDURE IF EXISTS CancelarOrden $$
CREATE PROCEDURE CancelarOrden (IN unIdOrden INT)
BEGIN
    DECLARE estadoOrden VARCHAR(30);
    DECLARE idTarifa INT;
    DECLARE idEntrada INT;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'No se pudo cancelar la orden. Revirtiendo cambios.';
    END;

    START TRANSACTION;

    -- Validar existencia
    IF NOT EXISTS (SELECT 1 FROM Orden WHERE IdOrden = unIdOrden) THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Orden inexistente.';
    END IF;

    SELECT Estado, IdTarifa INTO estadoOrden, idTarifa FROM Orden WHERE IdOrden = unIdOrden LIMIT 1;

    IF estadoOrden = 'Cancelado' THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'La orden ya está cancelada.';
    END IF;

    -- Marcar orden como cancelada
    UPDATE Orden
    SET Estado = 'Cancelado',
        Cierre = NOW()
    WHERE IdOrden = unIdOrden;

    -- Liberar stock (sumar la unidad previamente reservada) y actualizar estado si corresponde
    UPDATE Tarifa
    SET Stock = Stock + 1,
        Estado = CASE WHEN Estado = 'Agotada' AND Stock + 1 > 0 THEN 'Activa' ELSE Estado END
    WHERE IdTarifa = idTarifa;

    -- Si tenía entrada asociada: anularla y marcar su QR
    SELECT IdEntrada INTO idEntrada FROM Entrada WHERE IdOrden = unIdOrden LIMIT 1;

    IF idEntrada IS NOT NULL THEN
        UPDATE Entrada
        SET Estado = 'Anulado'
        WHERE IdEntrada = idEntrada;

        UPDATE QR
        SET TipoEstado = 'NoExiste'
        WHERE IdEntrada = idEntrada;
    END IF;

    COMMIT;
END$$

-- ===========================
-- CancelarEntrada
-- ===========================
DROP PROCEDURE IF EXISTS CancelarEntrada $$
CREATE PROCEDURE CancelarEntrada(IN unIdEntrada INT)
BEGIN
    DECLARE estadoEntrada VARCHAR(30);
    DECLARE idOrden INT;
    DECLARE idTarifa INT;
    DECLARE idQr INT;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Error al cancelar la entrada. Revirtiendo.';
    END;

    START TRANSACTION;

    -- Validar existencia de la entrada
    IF NOT EXISTS (SELECT 1 FROM Entrada WHERE IdEntrada = unIdEntrada) THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'La entrada no existe.';
    END IF;

    SELECT Estado, IdOrden INTO estadoEntrada, idOrden FROM Entrada WHERE IdEntrada = unIdEntrada LIMIT 1;

    IF estadoEntrada = 'Anulado' THEN
        ROLLBACK;
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'La entrada ya se encuentra anulada.';
    END IF;

    -- Anular la entrada
    UPDATE Entrada
    SET Estado = 'Anulado'
    WHERE IdEntrada = unIdEntrada;

    -- Liberar stock: obtener tarifa desde la orden y sumar 1
    SELECT IdTarifa INTO idTarifa FROM Orden WHERE IdOrden = idOrden LIMIT 1;

    IF idTarifa IS NOT NULL THEN
        UPDATE Tarifa
        SET Stock = Stock + 1,
            Estado = CASE WHEN Estado = 'Agotada' AND Stock + 1 > 0 THEN 'Activa' ELSE Estado END
        WHERE IdTarifa = idTarifa;
    END IF;

    -- Marcar QR asociado como 'NoExiste' (o eliminar según política)
    SELECT IdQR INTO idQr FROM QR WHERE IdEntrada = unIdEntrada LIMIT 1;
    IF idQr IS NOT NULL THEN
        UPDATE QR
        SET TipoEstado = 'NoExiste'
        WHERE IdQR = idQr;
    END IF;

    COMMIT;
END$$

DELIMITER ;
