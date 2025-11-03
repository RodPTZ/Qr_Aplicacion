::: mermaid
erDiagram
    Usuario {
        INT IdUsuario PK
        VARCHAR NombreUsuario
        VARCHAR Email
        VARCHAR Contraseña
    }

    Cliente {
        INT IdCliente PK
        INT IdUsuario FK
        VARCHAR Nombre
        VARCHAR Apellido
        INT DNI
        VARCHAR Telefono
        VARCHAR Localidad
        TINYINT Edad
    }

    Local {
        INT IdLocal PK
        VARCHAR Nombre
        VARCHAR Ubicacion
    }

    Sector {
        INT IdSector PK
        INT IdLocal FK
        SMALLINT Capacidad
    }

    Evento {
        INT IdEvento PK
        INT IdLocal FK
        VARCHAR Nombre
        ENUM Tipo
        ENUM Estado
    }

    Sesion {
        INT IdSesion PK
        INT IdEvento FK
        SMALLINT Cupos
        DATE Fecha
        TIME Apertura
        TIME Cierre
    }

    Funcion {
        INT IdFuncion PK
        INT IdEvento FK
        INT IdSector FK
        INT IdSesion FK
        DATETIME Fecha
        TIME Duracion
        BOOLEAN Cancelado
    }

    Tarifa {
        INT IdTarifa PK
        INT IdFuncion FK
        DECIMAL Precio
        INT Stock
        ENUM Estado
    }

    Orden {
        INT IdOrden PK
        INT IdTarifa FK
        INT IdSesion FK
        INT IdCliente FK
        ENUM Estado
        DATETIME Emision
        DATETIME Cierre
        ENUM MedioDePago
    }

    Entrada {
        INT IdEntrada PK
        INT IdOrden FK
        ENUM TipoEntrada
        DATETIME Emision
        DATETIME Liquidez
        ENUM Estado
    }

    QR {
        INT IdQR PK
        INT IdEntrada FK
        ENUM TipoEstado
        VARCHAR Codigo
    }

   Usuario ||--o| Cliente : ""
    Cliente ||--o| Orden : ""
    Orden ||--o| Entrada : ""
    Entrada ||--o| QR : ""
    Tarifa ||--o{ Orden : ""

    Local ||--o{ Sector : ""
    Local ||--o{ Evento : ""
    
    Evento ||--o{ Sesion : ""
    Evento ||--o{ Funcion : ""
    
    Funcion ||--o{ Tarifa : ""
    Sector ||--o{ Funcion : ""
    Sesion ||--o{ Funcion : ""

:::