
DROP USER IF EXISTS 'Admin'@'localhost'; 
CREATE USER 'Admin'@'localhost' IDENTIFIED BY 'AdminET12';
GRANT ALL PRIVILEGES ON 5to_SistemaDeBoleteria.* TO 'Admin'@'localhost';

DROP USER IF EXISTS 'Empleado'@'localhost';
CREATE USER 'Empleado'@'localhost' IDENTIFIED BY 'EmpleadoET12';
GRANT SELECT, INSERT, UPDATE ON 5to_SistemaDeBoleteria.Cliente TO 'Empleado'@'localhost';
GRANT SELECT, INSERT ON 5to_SistemaDeBoleteria.Orden TO 'Empleado'@'localhost';
GRANT SELECT, INSERT ON 5to_SistemaDeBoleteria.Entrada TO 'Empleado'@'localhost';
GRANT SELECT ON 5to_SistemaDeBoleteria.* TO 'Empleado'@'localhost';

DROP USER IF EXISTS 'Organizador'@'localhost';
CREATE USER 'Organizador'@'localhost' IDENTIFIED BY 'OrganizadorET12';
GRANT SELECT ON 5to_SistemaDeBoleteria.* TO 'Organizador'@'localhost';

DROP USER IF EXISTS 'Cliente'@'%';
CREATE USER 'Cliente'@'%' IDENTIFIED BY 'ClienteET12';
GRANT SELECT ON Usuario TO 'Cliente'@'%';
GRANT INSERT ON AuthTokens TO 'Cliente'@'%';