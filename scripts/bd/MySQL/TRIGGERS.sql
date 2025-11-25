
USE 5to_SistemaDeBoleteria;

DELIMITER $$

-- DROP TRIGGER IF EXISTS BefInsUsuario $$
-- CREATE TRIGGER BefInsUsuario BEFORE INSERT ON Usuario FOR EACH ROW
-- BEGIN
--     SET NEW.Contraseña = SHA2(NEW.Contraseña, 256);
-- END $$
-- DROP TRIGGER IF EXISTS BefInsFuncion $$
-- CREATE TRIGGER BefInsFuncion BEFORE INSERT ON Funcion FOR EACH ROW
-- BEGIN
--     IF(SELECT E.Estado FROM Evento E WHERE E.IdEvento = new.IdEvento) = 'Cancelado'
--     THEN
--         SET new.Cancelado = TRUE;
--     END IF;
-- END$$