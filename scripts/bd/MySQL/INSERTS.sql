
INSERT INTO Usuario (NombreUsuario, Email, Contraseña) VALUES
('martinl', 'martinl@example.com', '1234'),
('sofia_g', 'sofia_g@example.com', 'abcd'),
('natalia_r', 'natalia_r@example.com', 'qwerty'),
('agus99', 'agus99@example.com', 'pass123'),
('tomasv', 'tomasv@example.com', 'clave321');

INSERT INTO Cliente (IdUsuario, Nombre, Apellido, DNI, Telefono, Localidad, Edad) VALUES
(1, 'Martín', 'López', 45322111, '1132456789', 'Palermo', 25),
(2, 'Sofía', 'García', 39876543, '1156677788', 'Recoleta', 28),
(3, 'Natalia', 'Ramos', 41223987, '1165564433', 'Belgrano', 31),
(4, 'Agustín', 'Pérez', 40222456, '1145678912', 'Caballito', 27),
(5, 'Tomás', 'Vega', 44111888, '1178990011', 'San Telmo', 33);

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

INSERT INTO Sesion (IdEvento, Cupos, Fecha, Apertura, Cierre) VALUES
(1, 500, '2025-11-05', '19:00:00', '22:00:00'),
(2, 8000, '2025-12-10', '18:00:00', '23:59:00'),
(3, 2000, '2025-10-28', '10:00:00', '18:00:00'),
(4, 700, '2025-10-31', '23:00:00', '05:00:00'),
(5, 40000, '2025-11-20', '16:00:00', '21:00:00');

INSERT INTO Funcion (IdEvento, IdSector, IdSesion, Fecha, Duracion, Cancelado) VALUES
(1, 1, 1, '2025-11-05 20:00:00', '03:00:00', FALSE),
(2, 2, 2, '2025-12-10 19:30:00', '05:00:00', FALSE),
(3, 5, 3, '2025-10-28 11:00:00', '07:00:00', FALSE),
(4, 4, 4, '2025-10-31 23:30:00', '06:00:00', FALSE),
(5, 3, 5, '2025-11-20 17:00:00', '03:00:00', TRUE);

INSERT INTO Tarifa (IdFuncion, Precio, Stock, Estado) VALUES
(1, 25000.00, 500, TRUE),
(2, 18000.00, 8000, TRUE),
(3, 12000.00, 2000, FALSE),
(4, 8000.00, 700, TRUE),
(5, 35000.00, 40000, FALSE);

INSERT INTO Orden (IdTarifa, IdSesion, IdCliente, Estado, Emision, Cierre, MedioDePago) VALUES
(1, 1, 1, 'Abonado', '2025-10-10 15:00:00', '2025-10-10 15:05:00', 'Credito'),
(2, 2, 2, 'Abonado', '2025-10-12 18:20:00', '2025-10-12 18:25:00', 'Debito'),
(3, 3, 3, 'Cancelado', '2025-10-14 10:00:00', '2025-10-14 10:10:00', 'Transferencia'),
(4, 4, 4, 'Abonado', '2025-10-16 22:00:00', '2025-10-16 22:05:00', 'Efectivo'),
(5, 5, 5, 'Abonado', '2025-10-18 12:30:00', '2025-10-18 12:35:00', 'Credito');

INSERT INTO Entrada (IdOrden, TipoEntrada, Emision, Liquidez, Estado) VALUES
(1, 'VIP', '2025-10-10 15:10:00', '2025-11-05 19:30:00', 'Pagado'),
(2, 'General', '2025-10-12 18:30:00', '2025-12-10 18:30:00', 'Pagado'),
(3, 'PLUS', '2025-10-14 10:15:00', '2025-10-28 09:00:00', 'Pendiente'),
(4, 'General', '2025-10-16 22:10:00', '2025-10-31 22:30:00', 'Pagado'),
(5, 'VIP', '2025-10-18 12:40:00', '2025-11-20 15:00:00', 'Anulado');

/* INSERT INTO QR (IdEntrada, TipoEstado, Codigo) VALUES
(1, 'Ok', 'QR123ABC'),
(2, 'YaUsada', 'QR456DEF'),
(3, 'NoExiste', 'QR789GHI'),
(4, 'Ok', 'QR321JKL'),
(5, 'Expirada', 'QR654MNO'); */