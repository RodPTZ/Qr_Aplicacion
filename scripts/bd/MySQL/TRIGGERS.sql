CREATE TRIGGER RestarStockDespuesCompra
AFTER INSERT ON Entrada
FOR EACH ROW
BEGIN
    UPDATE Tarifa
    SET Stock = Stock - 1
    WHERE IdFuncion = (
        SELECT F.IdFuncion
        FROM Funcion F
        JOIN Orden O ON O.IdSesion = F.IdSesion
        WHERE O.IdOrden = NEW.IdOrden
        LIMIT 1
    );
END;
DELIMITER $$

CREATE TRIGGER VerificarCompraEntrada
BEFORE INSERT ON Entrada
FOR EACH ROW
BEGIN
    DECLARE vIdTarifa INT;
    DECLARE vIdFuncion INT;
    DECLARE vIdEvento INT;
    DECLARE vStock INT;
    DECLARE vPublicado BOOLEAN;
    DECLARE vCancelado BOOLEAN;
    DECLARE vFechaFuncion DATETIME;

    SELECT O.IdTarifa, F.IdFuncion, F.IdEvento, F.Fecha, F.Cancelado
    INTO vIdTarifa, vIdFuncion, vIdEvento, vFechaFuncion, vCancelado
    FROM Orden O
    JOIN Tarifa T ON O.IdTarifa = T.IdTarifa
    JOIN Funcion F ON T.IdFuncion = F.IdFuncion
    WHERE O.IdOrden = NEW.IdOrden
    LIMIT 1;

    IF vCancelado = TRUE THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'La función fue cancelada. No se puede comprar la entrada.';
    END IF;

   
    IF vFechaFuncion < NOW() THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'La función ya finalizó. No se puede comprar la entrada.';
    END IF;

    
    SELECT Stock INTO vStock
    FROM Tarifa
    WHERE IdTarifa = vIdTarifa;

    IF vStock <= 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'No hay más stock disponible para esta tarifa.';
    END IF;

    SELECT Publicado INTO vPublicado
    FROM Evento
    WHERE IdEvento = vIdEvento;

    IF vPublicado = FALSE THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El evento aún no está publicado. No se puede comprar la entrada.';
    END IF;

END$$

DELIMITER ;
CREATE TRIGGER VerificarStockAntesCompra
BEFORE INSERT ON Entrada
FOR EACH ROW
BEGIN
    DECLARE stockActual INT;

    SELECT T.Stock INTO stockActual
    FROM Tarifa T
    JOIN Funcion F ON T.IdFuncion = F.IdFuncion
    JOIN Orden O ON O.IdSesion = F.IdSesion
    WHERE O.IdOrden = NEW.IdOrden
    LIMIT 1;

    IF stockActual <= 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'No hay stock disponible para esta función.';
    END IF;

END;
