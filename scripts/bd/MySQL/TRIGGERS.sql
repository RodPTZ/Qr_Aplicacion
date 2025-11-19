
USE 5to_SistemaDeBoleteria;

DELIMITER $$

DROP TRIGGER IF EXISTS afsInsertEntradaAutoQR; 
CREATE TRIGGER afsInsertEntradaAutoQR
AFTER INSERT ON Entrada
FOR EACH ROW
BEGIN
    INSERT INTO QR (IdEntrada, Codigo, TipoEstado)
    VALUES (NEW.IdEntrada, CONCAT('QR-', UUID()), 'Ok');
END$$

DROP TRIGGER IF EXISTS befUpdateTarifaAgotar;
CREATE TRIGGER befUpdateTarifaAgotar
BEFORE UPDATE ON Tarifa
FOR EACH ROW
BEGIN
    IF NEW.Stock <= 0 THEN
        SET NEW.Stock = 0;
        SET NEW.Estado = 'Agotada';
    END IF;
END$$

DROP TRIGGER IF EXISTS afsAUpdateEntradaInvalidarQR; 
CREATE TRIGGER afsUpdateEntradaInvalidarQR
AFTER UPDATE ON Entrada
FOR EACH ROW
BEGIN
    IF NEW.Estado = 'Anulado' AND OLD.Estado <> 'Anulado' THEN
        UPDATE QR
        SET TipoEstado = 'NoExiste'
        WHERE IdEntrada = NEW.IdEntrada;
    END IF;
END$$

DROP TRIGGER IF EXISTS afsUpdateOrdenAsegurarEntrada;
CREATE TRIGGER afsUpdateOrdenAsegurarEntrada
AFTER UPDATE ON Orden
FOR EACH ROW
BEGIN
    IF NEW.Estado = 'Abonado' AND OLD.Estado <> 'Abonado' THEN
    
        -- Si no existen entradas, crearlas
        IF NOT EXISTS (SELECT 1 FROM Entrada WHERE IdOrden = NEW.IdOrden) THEN
            INSERT INTO Entrada (IdOrden, TipoEntrada, Emision, Liquidez, Estado)
            VALUES (
                NEW.IdOrden,
                (SELECT TipoEntrada FROM Tarifa WHERE IdTarifa = NEW.IdTarifa),
                NOW(),
                DATE_ADD(NOW(), INTERVAL 1 DAY),
                'Pagado'
            );
        END IF;
    END IF;
END$$

DROP TRIGGER IF EXISTS afsUpdateFuncionCancelar;
CREATE TRIGGER afsUpdateFuncionCancelar
AFTER UPDATE ON Funcion
FOR EACH ROW
BEGIN
    IF NEW.Cancelado = TRUE AND OLD.Cancelado = FALSE THEN
        
        -- Anular entradas vinculadas a sus órdenes
        UPDATE Entrada
        SET Estado = 'Anulado'
        WHERE IdOrden IN (
            SELECT IdOrden FROM Orden WHERE IdFuncion = NEW.IdFuncion
        );

        UPDATE QR
        SET TipoEstado = 'NoExiste'
        WHERE IdEntrada IN (
            SELECT IdEntrada FROM Entrada 
            WHERE IdOrden IN (
                SELECT IdOrden FROM Orden WHERE IdFuncion = NEW.IdFuncion
            )
        );
    END IF;
END$$