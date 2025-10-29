DELIMITER $$
DROP TRIGGER IF EXISTS VerificarCompraEntrada $$
CREATE TRIGGER VerificarCompraEntrada
BEFORE INSERT ON Entrada
FOR EACH ROW
BEGIN
    DECLARE vIdTarifa INT;
    DECLARE vIdFuncion INT;
    DECLARE vIdEvento INT;
    DECLARE vStock INT;
    DECLARE vEstadoEvento ENUM('Creado','Publicado','Cancelado');
    DECLARE vCancelado BOOLEAN;
    DECLARE vFechaFuncion DATETIME;
    DECLARE vEstadoTarifa ENUM('Activa','Inactiva','Agotada','Suspendida');


    SELECT 
        O.IdTarifa, F.IdFuncion, F.IdEvento, F.Fecha, F.Cancelado,
        E.Estado, T.Stock, T.Estado
    INTO 
        vIdTarifa, vIdFuncion, vIdEvento, vFechaFuncion, vCancelado,
        vEstadoEvento, vStock, vEstadoTarifa
    FROM Orden O
    JOIN Tarifa T ON O.IdTarifa = T.IdTarifa
    JOIN Funcion F ON T.IdFuncion = F.IdFuncion
    JOIN Evento E ON F.IdEvento = E.IdEvento
    WHERE O.IdOrden = NEW.IdOrden
    LIMIT 1;

   
    IF vCancelado = TRUE THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = ' La función fue cancelada. No se puede comprar la entrada.';
    END IF;


    IF vFechaFuncion < NOW() THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = ' La función ya finalizó. No se puede comprar la entrada.';
    END IF;

    IF vStock <= 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = ' No hay más stock disponible para esta tarifa.';
    END IF;

    IF vEstadoTarifa <> 'Activa' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = ' La tarifa no está activa. No se puede comprar la entrada.';
    END IF;

    IF vEstadoEvento <> 'Publicado' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = ' El evento aún no está publicado. No se puede comprar';
        END IF;
    END $$

CREATE TRIGGER Aft_Upd_Pagado_Orden AFTER UPDATE ON Orden FOR EACH ROW
BEGIN
    IF(old.Estado != 'Cancelado' AND new.Estado = 'Abonado')
    THEN
        INSERT INTO Entrada (IdOrden, Emision, Liquidez)
        VALUES (old.IdOrden, NOW(), DATE_ADD(NOW(), INTERVAL 1 DAY));

        SET @IdEntrada = LAST_INSERT_ID();

        INSERT INTO QR (IdEntrada, Codigo)
        VALUES (@IdEntrada, CONCAT_WS('-', old.IdOrden, old.IdCliente, old.IdTarifa));
    END IF;
END;