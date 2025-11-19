USE 5to_SistemaDeBoleteria;
-- USUARIOS
INSERT INTO Usuario (NombreUsuario, Email, Contraseña, Rol)
VALUES
('lucasAdmin', 'admin@test.com', 'pass', 'Admin'),
('sofiaCliente', 'sofia@test.com', 'pass', 'Cliente'),
('marcosCliente', 'marcos@test.com', 'pass', 'Cliente'),
('lauraCliente', 'laura@test.com', 'pass', 'Cliente'),
('diegoCliente', 'diego@test.com', 'pass', 'Cliente');

-- CLIENTES
INSERT INTO Cliente (IdUsuario, Nombre, Apellido, DNI, Telefono, Localidad, Edad)
VALUES
(2, 'Sofia', 'Mendez', 11111111, '1111-1111', 'CABA', 28),
(3, 'Marcos', 'Lopez', 22222222, '2222-2222', 'Cordoba', 32),
(4, 'Laura', 'Gomez', 33333333, '3333-3333', 'Rosario', 25),
(5, 'Diego', 'Perez', 44444444, '4444-4444', 'Mendoza', 29);

-- LOCALES
INSERT INTO Local (Nombre, Ubicacion)
VALUES
('Teatro Central', 'CABA'),
('Estadio Norte', 'Cordoba'),
('Auditorio Central', 'CABA');

-- SECTORES
INSERT INTO Sector (IdLocal, Capacidad)
VALUES
(1, 150),
(1, 50),
(2, 500),
(3, 120);

-- EVENTOS
INSERT INTO Evento (IdLocal, Nombre, Tipo, Estado)
VALUES
(1, 'Obra Romeo y Julieta', 'Teatro', 'Publicado'),
(2, 'Concierto RockFest', 'Concierto', 'Publicado'),
(1, 'Charla de Tecnología IA', 'Convencion', 'Creado');

-- FUNCIONES
INSERT INTO Funcion (IdEvento, IdSector, Apertura, Cierre, Cancelado)
VALUES
(1, 1, '2025-12-01 18:00:00', '2025-12-01 20:00:00', FALSE),
(1, 2, '2025-12-02 18:00:00', '2025-12-02 20:00:00', FALSE),
(2, 3, '2025-12-05 21:00:00', '2025-12-05 23:30:00', FALSE),
(3, 4, '2025-12-10 18:00:00', '2025-12-10 20:00:00', FALSE);

-- TARIFAS
INSERT INTO Tarifa (IdFuncion, TipoEntrada, Precio, Stock, Estado)
VALUES
(1, 'General', 5000, 100, 'Activa'),
(2, 'VIP', 8000, 40, 'Activa'),
(3, 'General', 12000, 300, 'Activa'),
(4, 'General', 12000, 300, 'Activa');
