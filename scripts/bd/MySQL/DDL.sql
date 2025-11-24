
DROP DATABASE IF EXISTS 5to_SistemaDeBoleteria;

CREATE DATABASE 5to_SistemaDeBoleteria;
USE 5to_SistemaDeBoleteria;

CREATE TABLE Usuario(
    IdUsuario INT UNSIGNED NOT NULL AUTO_INCREMENT,
    NombreUsuario VARCHAR(60),
    Email VARCHAR(60) NOT NULL,
    Contraseña VARCHAR(255),
    Rol ENUM('Admin','Empleado','Organizador','Cliente') NOT NULL DEFAULT 'Cliente',
    CONSTRAINT PK_Usuario PRIMARY KEY (IdUsuario)
);

CREATE TABLE Local (
    IdLocal INT UNSIGNED AUTO_INCREMENT NOT NULL,
    Nombre VARCHAR(255) NOT NULL,
    Ubicacion VARCHAR(30) NOT NULL,
    CONSTRAINT PK_Local PRIMARY KEY (IdLocal)
);

CREATE TABLE Cliente (
    IdCliente INT UNSIGNED NOT NULL AUTO_INCREMENT,
    IdUsuario INT UNSIGNED NOT NULL,
    Nombre VARCHAR(30) NOT NULL,
    Apellido VARCHAR(30) NOT NULL,
    DNI INT UNSIGNED NOT NULL,
    Telefono VARCHAR(20) NOT NULL,
    Localidad VARCHAR(30) NOT NULL,
    Edad TINYINT NOT NULL,
    CONSTRAINT PK_Cliente PRIMARY KEY (IdCliente),
    CONSTRAINT UK_Cliente UNIQUE (DNI),
    CONSTRAINT FK_Usuario FOREIGN KEY (IdUsuario) REFERENCES Usuario (IdUsuario)
);

CREATE TABLE Sector (
    IdSector INT UNSIGNED NOT NULL AUTO_INCREMENT,
    IdLocal INT UNSIGNED NOT NULL,
    Capacidad SMALLINT UNSIGNED NOT NULL,
    CONSTRAINT PK_Sector PRIMARY KEY (IdSector),
    CONSTRAINT FK_Sector_Local FOREIGN KEY (IdLocal) REFERENCES Local (IdLocal)
);

CREATE TABLE Evento (
    IdEvento INT UNSIGNED NOT NULL AUTO_INCREMENT,
    IdLocal INT UNSIGNED NOT NULL,
    Nombre VARCHAR(60) NOT NULL,
    Tipo ENUM(
        'Concierto',
        'Convencion',
        'Opera',
        'Teatro',
        'Deportes',
        'Boliches',
        'Música'
    ) NOT NULL,
    Estado ENUM('Publicado','Cancelado','Creado') NOT NULL DEFAULT 'Creado',
    CONSTRAINT PK_Evento PRIMARY KEY (IdEvento),
    CONSTRAINT FK_Evento_Local FOREIGN KEY (IdLocal) REFERENCES Local (IdLocal)
);

CREATE TABLE Funcion (
    IdFuncion INT UNSIGNED AUTO_INCREMENT NOT NULL,
    IdEvento INT UNSIGNED NOT NULL,
    IdSector INT UNSIGNED NOT NULL,
    Apertura DATETIME NOT NULL,
    Cierre DATETIME NOT NULL,
    Cancelado BOOLEAN DEFAULT FALSE,
    CONSTRAINT PK_Funcion PRIMARY KEY (IdFuncion),
    CONSTRAINT FK_Funcion_Evento FOREIGN KEY (IdEvento) REFERENCES Evento (IdEvento),
    CONSTRAINT FK_Funcion_Sector FOREIGN KEY (IdSector) REFERENCES Sector (IdSector)
);

CREATE TABLE Tarifa (
    IdTarifa INT UNSIGNED AUTO_INCREMENT NOT NULL,
    IdFuncion INT UNSIGNED NOT NULL,
    TipoEntrada ENUM('General', 'VIP', 'PLUS'),
    Precio DECIMAL(10, 2) NOT NULL,
    Stock INT UNSIGNED NOT NULL,
    Estado ENUM('Activa', 'Inactiva', 'Agotada', 'Suspendida') DEFAULT 'Inactiva',
    CONSTRAINT PK_Tarifa PRIMARY KEY (IdTarifa),
    CONSTRAINT FK_Tarifa_Funcion FOREIGN KEY (IdFuncion) REFERENCES Funcion (IdFuncion)
);

CREATE TABLE Orden (
    IdOrden INT UNSIGNED AUTO_INCREMENT NOT NULL,
    IdTarifa INT UNSIGNED NOT NULL,
    IdCliente INT UNSIGNED NOT NULL,
    Estado ENUM('Abonado','Cancelado','Expirado', 'Creado') DEFAULT 'Creado',
    Emision DATETIME NOT NULL,
    Cierre DATETIME NOT NULL,
    MedioDePago ENUM('Efectivo','Transferencia','Debito','Credito') NOT NULL,
    CONSTRAINT PK_Orden PRIMARY KEY (IdOrden),
    CONSTRAINT FK_Orden_Cliente FOREIGN KEY (IdCliente) REFERENCES Cliente (IdCliente),
    CONSTRAINT FK_Orden_Tarifa FOREIGN KEY (IdTarifa) REFERENCES Tarifa (IdTarifa)
);

CREATE TABLE Entrada (
    IdEntrada INT UNSIGNED AUTO_INCREMENT NOT NULL,
    IdOrden INT UNSIGNED NOT NULL,
    TipoEntrada ENUM('General', 'VIP', 'PLUS'),
    Emision DATETIME NOT NULL,
    Liquidez DATETIME NOT NULL,
    Anulado BOOLEAN DEFAULT FALSE,
    CONSTRAINT PK_Entrada PRIMARY KEY (IdEntrada),
    CONSTRAINT FK_Entrada_Orden FOREIGN KEY (IdOrden) REFERENCES Orden (IdOrden)
);

CREATE TABLE QR (
    IdQR INT UNSIGNED AUTO_INCREMENT NOT NULL,
    IdEntrada INT UNSIGNED NOT NULL,
    TipoEstado ENUM(
        'Ok',
        'YaUsada',
        'Expirada',
        'FirmaInvalida',
        'NoExiste'
    ) DEFAULT 'NoExiste' NOT NULL,
    Codigo VARCHAR(255) NOT NULL,
    CONSTRAINT PK_Qr PRIMARY KEY (IdQR),
    CONSTRAINT FK_Qr_Entrada FOREIGN KEY (IdEntrada) REFERENCES Entrada (IdEntrada)
);

CREATE TABLE AuthTokens (
    IdToken INT UNSIGNED AUTO_INCREMENT NOT NULL,
    IdUsuario INT UNSIGNED NOT NULL,
    Token VARCHAR(500) NOT NULL,
    Expiracion DATETIME NOT NULL,
    Revocado BOOLEAN DEFAULT FALSE,
    CONSTRAINT PK_TokenRevoked PRIMARY KEY (IdToken),
    CONSTRAINT FK_TokenRevoked_Usuario FOREIGN KEY (IdUsuario) REFERENCES Usuario (IdUsuario)
);