
USE 5to_SistemaDeBoleteria;

INSERT INTO Usuario (NombreUsuario, Email, Contraseña, Rol) VALUES
('admin', 'admin@gmail.com', SHA2('123', 256), 'Admin'),
('empleado','empleado@gmail.com',SHA2('qwe', 256),'Empleado'),
('Organizador','organizador@gmail.com',SHA2('asd', 256),'Organizador');

INSERT INTO Usuario (NombreUsuario, Email, Contraseña, Rol) VALUES
('martinl', 'martinl@example.com', SHA2('1234', 256), 'Organizador'),
('sofia_g', 'sofia_g@example.com', SHA2('abcd', 256), 'Organizador'),
('natalia_r', 'natalia_r@example.com', SHA2('qwerty', 256), 'Empleado'),
('agus99', 'agus99@example.com', SHA2('pass123', 256), 'Empleado'),
('tomasv', 'tomasv@example.com', SHA2('clave321', 256), 'Empleado');
INSERT INTO Cliente (IdUsuario, Nombre, Apellido, DNI, Telefono, Localidad, Edad) VALUES
(4, 'Martín', 'López', 45322111, '1132456789', 'Palermo', 25),
(5, 'Sofía', 'García', 39876543, '1156677788', 'Recoleta', 28),
(6, 'Natalia', 'Ramos', 41223987, '1165564433', 'Belgrano', 31),
(7, 'Agustín', 'Pérez', 40222456, '1145678912', 'Caballito', 27),
(8, 'Tomás', 'Vega', 44111888, '1178990011', 'San Telmo', 33);

INSERT INTO Local (Nombre, Ubicacion) VALUES
('Teatro Colón', 'CABA'),
('Luna Park', 'CABA'),
('Estadio Único', 'La Plata'),
('Club Museum', 'San Telmo'),
('Centro de Convenciones', 'Puerto Madero');

INSERT INTO Sector (IdLocal, Capacidad) VALUES
(1, 1200),
(2, 9000),
(3, 45000),
(4, 800),
(5, 3000);

INSERT INTO Evento (IdLocal, Nombre, Tipo, Estado) VALUES
(1, 'La Traviata', 'Opera', 'Publicado'),
(2, 'Duki en Vivo', 'Concierto', 'Publicado'),
(3, 'ComicCon 2025', 'Convencion', 'Creado'),
(4, 'Noche Retro', 'Boliches', 'Publicado'),
(5, 'Boca vs River', 'Deportes', 'Cancelado');

INSERT INTO Funcion (IdEvento, IdSector, Apertura, Cierre, Cancelado) VALUES
(1, 1, '2025-11-05 19:00:00', '2025-11-05 22:00:00', FALSE),
(2, 2, '2025-12-10 18:00:00', '2025-12-10 23:59:00', FALSE),
(3, 5, '2025-10-28 10:00:00', '2025-10-28 18:00:00', FALSE),
(4, 4, '2025-10-31 23:00:00', '2025-11-01 05:00:00', FALSE),
(5, 3, '2025-11-20 16:00:00', '2025-11-20 21:00:00', TRUE);


INSERT INTO Tarifa (IdFuncion, TipoEntrada, Precio, Stock, Estado) VALUES
(1, 'VIP',25000.00, 500, 'Activa'),
(2, 'General',18000.00, 8000, 'Activa'),
(3, 'PLUS',12000.00, 2000, 'Inactiva'),
(4, 'General',8000.00, 700, 'Activa'),
(5, 'VIP',35000.00, 40000, 'Inactiva');

INSERT INTO Orden (IdTarifa, IdCliente, Estado, Emision, Cierre, MedioDePago) VALUES
(1, 1, 'Abonado', '2025-10-10 15:00:00', '2025-10-10 15:05:00', 'Credito'),
(2, 2, 'Abonado', '2025-10-12 18:20:00', '2025-10-12 18:25:00', 'Debito'),
(3, 3, 'Cancelado', '2025-10-14 10:00:00', '2025-10-14 10:10:00', 'Transferencia'),
(4, 4, 'Abonado', '2025-10-16 22:00:00', '2025-10-16 22:05:00', 'Efectivo'),
(5, 5, 'Abonado', '2025-10-18 12:30:00', '2025-10-18 12:35:00', 'Credito');

INSERT INTO Entrada (IdOrden, TipoEntrada, Emision, Liquidez, Anulado) VALUES
(1, 'VIP', '2025-10-10 15:10:00', '2025-11-05 19:30:00', FALSE),
(2, 'General', '2025-10-12 18:30:00', '2025-12-10 18:30:00', FALSE),
(3, 'PLUS', '2025-10-14 10:15:00', '2025-10-28 09:00:00', FALSE),
(4, 'General', '2025-10-16 22:10:00', '2025-10-31 22:30:00', FALSE),
(5, 'VIP', '2025-10-18 12:40:00', '2025-11-20 15:00:00', TRUE);

/* INSERT INTO QR (IdEntrada, TipoEstado, Codigo) VALUES
(1, 'Ok', 'QR123ABC'),
(2, 'YaUsada', 'QR456DEF'),
(3, 'NoExiste', 'QR789GHI'),
(4, 'Ok', 'QR321JKL'),
(5, 'Expirada', 'QR654MNO'); */