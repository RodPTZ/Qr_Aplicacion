DROP USER IF 'admin'@'localhost'; 
CREATE USER 'admin'@'localhost' IDENTIFIED BY 'AdminET12';
GRANT ALL PRIVILEGES ON 5to_SistemaDeBoleteria.* TO 'admin'@'localhost';

DROP USER IF 'Empleado'@'localhost';
CREATE USER 'Empleado'@'localhost' IDENTIFIED BY 'EmpleadoET12';
GRANT SELECT, INSERT, UPDATE ON 5to_SistemaDeBoleteria.Cliente TO 'Empleado'@'localhost';
GRANT SELECT, INSERT ON 5to_SistemaDeBoleteria.Orden TO 'Empleado'@'localhost';
GRANT SELECT, INSERT ON 5to_SistemaDeBoleteria.Entrada TO 'Empleado'@'localhost';
GRANT SELECT ON 5to_SistemaDeBoleteria.* TO 'Empleado'@'localhost';

DROP IF EXISTS 'Organizador'@'localhost';
CREATE USER 'Organizador'@'localhost' IDENTIFIED BY 'OrganizadorET12';
GRANT SELECT ON 5to_SistemaDeBoleteria.* TO 'Organizador'@'localhost';