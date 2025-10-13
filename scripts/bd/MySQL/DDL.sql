CREATE DATABASE 5to_SistemaDeBoleteria;

USE 5to_SistemaDeBoleteria;

CREATE TABLE Cliente (
    IdCliente INT UNSIGNED NOT NULL AUTO_INCREMENT,
    Nombre VARCHAR(30) NOT NULL,
    Apellido VARCHAR(30) NOT NULL,
    DNI INT UNSIGNED NOT NULL,
    Email VARCHAR(60) NOT NULL,
    Telefono INT NOT NULL,
    Localidad VARCHAR(30) NOT NULL,
    Edad TINYINT NOT NULL,
    CONSTRAINT PK_Cliente PRIMARY KEY (IdCliente),
    CONSTRAINT UK_Cliente UNIQUE KEY (DNI)
);

CREATE TABLE Local (
    IdLocal INT UNSIGNED AUTO_INCREMENT NOT NULL,
    Ubicacion VARCHAR(30) NOT NULL,
    CONSTRAINT PK_Local PRIMARY KEY (ID)
);

CREATE TABLE Sector (
    IdSector INT UNSIGNED NOT NULL AUTO_INCREMENT,
    IdLocal INT UNSIGNED NOT NULL,
    TipoSector VARCHAR(30) NOT NULL,
    CONSTRAINT PK_Sector PRIMARY KEY (IdSector),
    CONSTRAINT FK_Sector_Local FOREIGN KEY (IdLocal) REFERENCES Local (ID)
);

CREATE TABLE Evento (
    IdEvento INT UNSIGNED NOT NULL AUTO_INCREMENT,
    IdLocal INT UNSIGNED NOT NULL,
    Nombre VARCHAR(30) NOT NULL,
    Tipo ENUM(
        'Concierto',
        'Convencion',
        'Opera',
        'Teatro',
        'Deportes',
        'Boliches',
        'Música'
    ) NOT NULL,
    publicado BOOLEAN DEFAULT FALSE NOT NULL,
    CONSTRAINT PK_Evento PRIMARY KEY (IdEvento),
    constraint FK_Evento_Local FOREIGN KEY (IdLocal) REFERENCES Local (IdLocal)
);

CREATE TABLE Sesion (
    IdSesion INT UNSIGNED AUTO_INCREMENT NOT NULL,
    IdEvento INT UNSIGNED NOT NULL,
    cupos SMALLINT UNSIGNED NOT NULL,
    Fecha DATE NOT NULL,
    Apertura TIME NOT NULL,
    Cierre TIME NOT NULL,
    CONSTRAINT PK_Sesion PRIMARY KEY (IdSesion),
    CONSTRAINT FK_Sesion_Evento FOREIGN KEY (IdEvento) REFERENCES Evento (IdEvento)
);

CREATE TABLE Funcion (
    IdFuncion INT UNSIGNED AUTO_INCREMENT NOT NULL,
    IdEvento INT UNSIGNED NOT NULL,
    IdSector INT UNSIGNED NOT NULL,
    IdSesion INT UNSIGNED NOT NULL,
    Fecha DATETIME NOT NULL,
    Duracion TIME NOT NULL,
    Cancelado BOOLEAN DEFAULT FALSE,
    CONSTRAINT PK_Funcion PRIMARY KEY (IdFuncion),
    CONSTRAINT FK_Funcion_Evento FOREIGN KEY (IdEvento) REFERENCES Evento (IdEvento),
    CONSTRAINT FK_Funcion_Sector FOREIGN KEY (IdSector) REFERENCES Sector (IdSector)
);

CREATE TABLE Tarifa (
    IdTarifa INT UNSIGNED AUTO_INCREMENT NOT NULL,
    IdFuncion INT UNSIGNED NOT NULL,
    Precio DECIMAL(10, 2) NOT NULL,
    Stock INT UNSIGNED NOT NULL,
    Estado BOOL DEFAULT FALSE,
    CONSTRAINT PK_Tarifa PRIMARY KEY (IdTarifa),
    CONSTRAINT FK_Tarifa_Funcion FOREIGN KEY (IdFuncion) REFERENCES Funcion (IdFuncion)
);

CREATE TABLE Orden (
    IdOrden INT UNSIGNED AUTO_INCREMENT NOT NULL,
    idSesion INT UNSIGNED NOT NULL,
    IdCliente INT UNSIGNED NOT NULL,
    TipoEntrada ENUM('General', 'VIP', 'Plus'),
    Abonado BOOLEAN DEFAULT FALSE,
    Cancelado BOOLEAN DEFAULT FALSE,
    Emision DATETIME NOT NULL,
    Cierre DATETIME NOT NULL,
    MedioDePago VARCHAR(30),
    CONSTRAINT PK_Orden PRIMARY KEY (IdOrden),
    CONSTRAINT FK_Orden_Sesion FOREIGN KEY (IdSesion) REFERENCES Sesion (IdSesion),
    CONSTRAINT FK_Orden_Cliente FOREIGN KEY (IdCliente) REFERENCES Cliente (IdCliente)
);

CREATE TABLE Entrada (
    IdEntrada INT UNSIGNED AUTO_INCREMENT NOT NULL,
    IdOrden INT UNSIGNED NOT NULL,
    TipoEntrada ENUM('General', 'VIP', 'PLUS'),
    Emision DATETIME NOT NULL,
    Liquidez DATETIME NOT NULL,
    Anulada BOOLEAN DEFAULT FALSE,
    CONSTRAINT FK_Entrada_Orden FOREIGN KEY (IdOrden) REFERENCES Orden (IdOrden),
    CONSTRAINT PK_Entrada PRIMARY KEY (IdEntrada)
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
    ) NOT NULL,
    Código VARCHAR(60) NOT NULL,
    CONSTRAINT PK_Qr PRIMARY KEY (IdQr),
    CONSTRAINT FK_Qr_Entrada FOREIGN KEY (IdEntrada) REFERENCES Entrada (IdEntrada)
);
