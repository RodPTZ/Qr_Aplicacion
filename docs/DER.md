``` mermaid
erDiagram
    Usuario {
        int IdUsuario PK
        string NombreUsuario
        string Email
        string Contraseña
        enum Rol
    }

    Local {
        int IdLocal PK
        string Nombre
        string Ubicacion
    }

    Cliente {
        int IdCliente PK
        int IdUsuario FK
        string Nombre
        string Apellido
        int DNI
        string Telefono
        string Localidad
        tinyint Edad
    }

    Sector {
        int IdSector PK
        int IdLocal FK
        smallint Capacidad
    }

    Evento {
        int IdEvento PK
        int IdLocal FK
        string Nombre
        enum Tipo
        enum Estado
    }

    Funcion {
        int IdFuncion PK
        int IdEvento FK
        int IdSector FK
        datetime Apertura
        datetime Cierre
        boolean Cancelado
    }

    Tarifa {
        int IdTarifa PK
        int IdFuncion FK
        enum TipoEntrada
        decimal Precio
        int Stock
        enum Estado
    }

    Orden {
        int IdOrden PK
        int IdTarifa FK
        int IdCliente FK
        enum Estado
        datetime Emision
        datetime Cierre
        enum MedioDePago
    }

    Entrada {
        int IdEntrada PK
        int IdOrden FK
        enum TipoEntrada
        datetime Emision
        datetime Liquidez
        boolean Anulado
    }

    QR {
        int IdQR PK
        int IdEntrada FK
        enum TipoEstado
        string Codigo
    }

    AuthTokens {
        int IdToken PK
        int IdUsuario FK
        string Token
        datetime Expiracion
        boolean Revocado
    }

    Usuario ||--o{ Cliente : "tiene"
    Usuario ||--o{ AuthTokens : "tiene"
    Local ||--o{ Sector : "contiene"
    Local ||--o{ Evento : "alberga"
    Evento ||--o{ Funcion : "tiene"
    Sector ||--o{ Funcion : "contiene"
    Funcion ||--o{ Tarifa : "define"
    Cliente ||--o{ Orden : "realiza"
    Tarifa ||--o{ Orden : "aplica"
    Orden ||--o{ Entrada : "genera"
    Entrada ||--o{ QR : "genera"

```
