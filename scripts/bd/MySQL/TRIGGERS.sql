
USE 5to_sistemadeboleteria

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
    DECLARE vAperturaFuncion DATETIME;
    DECLARE vEstadoTarifa ENUM('Activa','Inactiva','Agotada','Suspendida');


    SELECT 
        O.IdTarifa, F.IdFuncion, F.IdEvento, F.Apertura, F.Cancelado,
        E.Estado, T.Stock, T.Estado
    INTO 
        vIdTarifa, vIdFuncion, vIdEvento, vAperturaFuncion, vCancelado,
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


    IF vAperturaFuncion < NOW() THEN
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

-- CREATE TRIGGER trgCancelarOrden
-- BEFORE UPDATE ON Orden
-- FOR EACH ROW
-- BEGIN
--     IF NEW.Estado = 'Cancelado' AND OLD.Estado <> 'Cancelado' THEN
--         UPDATE Tarifa
--         SET Stock = Stock + 1
--         WHERE IdFuncion = OLD.IdFuncion;
--     END IF;
-- END;

CREATE TRIGGER BefInsTarifa BEFORE INSERT ON Tarifa FOR EACH ROW 
BEGIN
    IF NOT EXISTS(SELECT * FROM Funcion WHERE  IdFuncion = new.IdFuncion)
    THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'La funcion ingresada no existe';
    END IF;

    IF((SELECT Cancelado FROM Funcion WHERE IdFuncion = new.IdFuncion) = TRUE)
    THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'La función ingresada se encuentra cancelada.';
    END IF;
END;

-- ========================= EVENTO =====================================
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
END$$

-- =======================================================================

-- ======================= FUNCION ==================================

CREATE TRIGGER BefInsFuncion BEFORE INSERT ON Funcion FOR EACH ROW
BEGIN
    IF(SELECT E.Estado FROM Evento E WHERE E.IdEvento = new.IdEvento) = 'Cancelado'
    THEN
        SET new.Cancelado = TRUE;
    END IF;
END$$

-- ==================================================================