# FunkoApp

![.NET](https://img.shields.io/badge/.NET-10-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-14-239120?style=flat-square&logo=csharp&logoColor=white)
![Tests](https://img.shields.io/badge/tests-53%20passing-2ea44f?style=flat-square)
![SQLite](https://img.shields.io/badge/EF%20Core-SQLite-003B57?style=flat-square&logo=sqlite&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-multi--stage-2496ED?style=flat-square&logo=docker&logoColor=white)
![Swagger](https://img.shields.io/badge/OpenAPI-Swagger-85EA2D?style=flat-square&logo=swagger&logoColor=black)
![JWT](https://img.shields.io/badge/Auth-JWT%20%2B%20BCrypt-000000?style=flat-square&logo=jsonwebtokens&logoColor=white)

Práctica 3 de Desarrollo Web en Entorno Servidor (DWES), unidad 2: una API REST para
gestionar un catálogo de Funkos (figuras de vinilo coleccionables).

**Autores:** Cristian Álvarez · Jesús Cobo (base inicial del proyecto, ver
[De dónde viene este proyecto](#-de-dónde-viene-este-proyecto))

---

## 🎯 Qué hace

CRUD completo sobre un catálogo de Funkos en memoria, con validación en dos capas,
errores de dominio sin excepciones, documentación Swagger y despliegue con Docker.

| Operación | Verbo | Ruta | Auth | Éxito |
|---|---|---|---|---|
| Listar todos los Funkos | `GET` | `/api/Funko` | pública | `200` |
| Consultar un Funko por id | `GET` | `/api/Funko/{id}` | pública | `200` / `404` |
| Buscar Funkos por nombre | `GET` | `/api/Funko/search?nombre=` | pública | `200` |
| Crear un Funko | `POST` | `/api/Funko` | autenticado | `201` |
| Actualizar un Funko completo | `PUT` | `/api/Funko/{id}` | autenticado | `200` / `404` |
| Actualizar solo el precio | `PATCH` | `/api/Funko/{id}` | autenticado | `200` / `404` |
| Eliminar un Funko (borrado lógico) | `DELETE` | `/api/Funko/{id}` | **solo Admin** | `204` / `404` / `403` |
| Registrarse | `POST` | `/api/auth/signup` | pública | `201` / `409` |
| Iniciar sesión | `POST` | `/api/auth/login` | pública | `200` / `401` |
| Perfil del usuario autenticado | `GET` | `/api/auth/me` | autenticado | `200` / `401` |

---

## 🏗️ Arquitectura

```
Program.cs → FunkoController → ServiceFunko → FunkoValidator
                                             → RepositoryFunko   (EF Core, async)
                                                   → AppDbContext → SQLite (Dev) / PostgreSQL (Prod, pendiente)
```

El controlador no conoce el repositorio ni el modelo de dominio: solo habla con
`IServiceFunko` y solo recibe y devuelve DTOs (`CreateFunkoDto`, `UpdateFunkoDto`,
`FunkoDto`).

Los errores de negocio no se lanzan como excepciones. Cada operación del servicio
devuelve un `Result<T, DomainError>` de CSharpFunctionalExtensions, y el controlador
traduce cada tipo de error a su código HTTP:

| `DomainError` | Código HTTP | Cuándo |
|---|---|---|
| `NotFound` | `404` | El id no existe (o ya estaba borrado) |
| `AlreadyExists` | `409` | Ya hay un Funko activo con ese nombre |
| `Validation` | `400` | El nombre o la categoría están vacíos/en blanco |
| `Unauthorized` | `401` | Credenciales inválidas, o sin token / token inválido |
| `Conflict` | `409` | Email ya registrado (`signup`) |

La persistencia es real (EF Core + SQLite, con migraciones), no en memoria: hay una
relación **Uno a Muchos** entre `Categoria` y `Funko` en la base de datos, y el
borrado es lógico (`IsDeleted`, oculto automáticamente de cualquier consulta con un
`HasQueryFilter`). El modelo de dominio sigue viendo `Categoria` como un simple
`string` — es el repositorio quien resuelve o crea la fila de categoría al guardar.

El repositorio nunca lanza ni devuelve un `Result`: solo valores planos (`null`,
`false`). Es el servicio quien decide qué significa cada uno. Las Data Annotations
de los DTOs (`[Required]`, `[Range]`, `[StringLength]`) cubren el formato de entrada
y las captura automáticamente `[ApiController]` antes de que la petición llegue al
controlador; el `FunkoValidator` cubre lo que las Data Annotations no ven bien (por
ejemplo, una cadena de solo espacios).

Las excepciones que de verdad son inesperadas (no un fallo de negocio, sino un error
de programación) las captura `ExceptionHandlerMiddleware` y las traduce también a
`ProblemDetails`, para que el cliente nunca vea un 500 en blanco.

---

## 🔐 Autenticación

Lectura pública, escritura solo para usuarios autenticados, borrado solo para el rol
`ADMIN`. JWT firmado con HMAC-SHA256 (sin ASP.NET Core Identity: enfoque manual, con
`JwtService` + `AuthService`), contraseñas con BCrypt (`workFactor: 11`).

**Usuarios de prueba** (creados automáticamente al arrancar si la tabla está vacía):

| Email | Contraseña | Rol |
|---|---|---|
| `admin@funko.com` | `admin123` | `ADMIN` |
| `user@funko.com` | `user123` | `USER` |

```bash
curl -X POST http://localhost:5230/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@funko.com","password":"admin123"}'
```

La respuesta trae `accessToken`: se usa como `Authorization: Bearer <token>` en las
peticiones protegidas. En Swagger (`/swagger`), el botón **Authorize** (arriba a la
derecha) pide el token con el mismo formato (`Bearer eyJ...`).

> [!NOTE]
> El token dura 15 minutos (`Jwt:ExpirationMinutes` en `appsettings.json`). La clave
> de firma vive en claro en `appsettings.json` porque esto es una práctica académica
> con SQLite local — en un proyecto real iría a `dotnet user-secrets`, una variable
> de entorno o un gestor de secretos, nunca comiteada.

---

## 📁 Estructura del proyecto

```
.
├── FunkoAppi01.sln
├── Dockerfile                  (dentro de FunkoAppi01/)
├── compose.yaml
├── FunkoAppi01/
│   ├── Program.cs
│   ├── Controller/             FunkoController
│   ├── Service/                IServiceFunko, ServiceFunko
│   ├── Repository/              IRepositoryFunko, RepositoryFunko (en memoria)
│   ├── Validators/              IFunkoValidator, FunkoValidator
│   ├── Dto/                     CreateFunkoDto, UpdateFunkoDto, FunkoDto
│   ├── Mappers/                 FunkoMapper (DTO ↔ modelo)
│   ├── Models/                  Funko
│   ├── Errors/                  DomainError y su factoría FunkoErrors
│   ├── Middleware/               ExceptionHandlerMiddleware
│   └── Infraestructure/         Registro de dependencias (repos, validadores, servicios)
└── FunkoApp.Test/              31 tests con NUnit, Moq y FluentAssertions
```

---

## ▶️ Cómo ejecutarlo

```bash
dotnet run --project FunkoAppi01
```

Arranca en `http://localhost:5230` (o el puerto que asigne `dotnet run`). La
documentación interactiva está en `http://localhost:5230/swagger`.

### Con Docker

```bash
docker compose up --build
```

Publica la API en `http://localhost:8090` (Swagger en `http://localhost:8090/swagger`).
Para pararlo: `Ctrl+C` y después `docker compose down`.

> [!NOTE]
> El puerto del host es **8090**, no 8080: en la máquina de desarrollo el 8080 ya
> estaba ocupado por otro proceso ajeno al proyecto, y de paso probamos que el
> contenedor no respondía porque `compose.yaml` no publicaba ningún puerto. Ver
> [Decisiones de diseño](#-decisiones-de-diseño).

---

## ✅ Tests

```bash
dotnet test
```

**53 tests**, con NUnit, Moq y FluentAssertions, patrón AAA (Arrange, Act, Assert) y
separados en `CasosPositivos` y `CasosNegativos`.

| Qué se prueba | Nº |
|---|---|
| `DomainError` | 3 |
| `FunkoMapper` | 3 |
| `FunkoValidator` | 7 |
| `RepositoryFunko` (SQLite real en memoria, sin mocks) | 8 |
| `ServiceFunko` (repositorio y validador mockeados) | 11 |
| `JwtService` | 2 |
| `AuthService` (repositorio mockeado) | 6 |
| `FunkoController` — integración con `WebApplicationFactory` | 8 |
| `AuthController` — integración con `WebApplicationFactory` | 6 |

Los de integración levantan la API entera (sin mocks en ningún punto) contra una
base de datos EF Core InMemory aislada por test, y no solo miran el código de
estado: también comprueban la **forma** del error (p. ej. que un `400` trae
`errors` agrupados por campo, y que un `404` trae un `detail` legible) — es el
contrato real que vería un cliente.

Los de `ServiceFunko` no solo miran lo que devuelve el método, sino a quién llama:
si la validación falla, o el nombre ya existe, o el precio es negativo, se verifica
con `Times.Never` que el repositorio **no llega a tocarse**. Si alguien rompiera ese
cortocircuito, el valor devuelto podría seguir pareciendo correcto — esa comprobación
es la única que lo detectaría.

---

## 🧠 Decisiones de diseño

### Minimal API vs. controladores

La pauta pide implementar el CRUD primero como Minimal API (fase 3) y después con
controladores (fase 4), comparando ambos enfoques. Partimos del código de un
compañero que ya tenía el CRUD hecho con controladores, así que **no se ha hecho la
versión Minimal API por separado**: decisión consciente, para llegar antes a las
fases avanzadas (validación, errores de dominio, tests) con el tiempo que había.
Si hubiera que elegir solo uno para un proyecto real, los controladores ganan en
cuanto el número de recursos crece: agrupan mejor las acciones relacionadas y se
documentan solos con los atributos `[Http*]` y XMLDoc, mientras que una Minimal API
con muchos endpoints se vuelve un `Program.cs` largo y repetitivo.

### Validación en dos capas, no una

Las Data Annotations de los DTOs cubren lo que se puede expresar declarativamente
(`[Required]`, `[Range]`). El `FunkoValidator` cubre lo que no: comprobamos en vivo
que `[Required]` ya rechaza una cadena de solo espacios (.NET la recorta antes de
comprobar), así que en la práctica el validador de dominio actúa como red de
seguridad para cualquier llamada que no pase por el `ModelState` de HTTP — por
ejemplo, si el servicio se usa algún día desde un test o desde otro punto de entrada.
No es redundancia inútil: es que un camino de entrada (HTTP) ya atrapa el error antes,
pero el servicio sigue estando protegido aunque no sea por ese camino.

### Nombre duplicado es responsabilidad del servicio, no del repositorio

El repositorio solo expone `ExistsByName(nombre, excludeId)` y devuelve `bool`. Es el
`ServiceFunko` quien decide que un nombre duplicado es un `DomainError.AlreadyExists`.
Así todas las reglas de negocio quedan en un único sitio y el repositorio se limita a
guardar y consultar.

### Borrado lógico con un único booleano

El modelo original tenía `IsActivo` (booleano) y `DeleteAt` (fecha) a la vez, pero
solo se usaba el booleano para filtrar. Un único campo `IsDeleted` es suficiente para
lo que pide la pauta; mantener los dos obligaría a sincronizarlos siempre juntos y es
fuente de bugs si alguno se actualiza y el otro no.

### El repositorio pasó a async en cuanto hubo E/S real

Mientras el repositorio era un `Dictionary` en memoria no había ninguna E/S que
esperar, así que se quedó síncrono (ver la decisión equivalente más abajo, cuando
aún no existía la base de datos). En cuanto llegó EF Core con SQLite, todo el camino
—repositorio, servicio y controlador— pasó a `async`/`Task`, que es justo el momento
en el que el `async` empieza a tener sentido real.

### `Categoria` es solo un texto en el dominio, una relación real en la base de datos

Los DTOs de la pauta (`CreateFunkoDto`, `FunkoDto`...) solo piden `Categoria` como
una cadena de texto. La base de datos sí tiene la relación Uno a Muchos real que pide
la pauta (`CategoriaEntity` ← `FunkoEntity`, con clave foránea e índice único). El
repositorio resuelve o crea la fila de categoría por nombre al guardar un Funko, y la
aplana de vuelta a texto al leer. Así no hay que tocar DTOs, validador, mapper ni
tests ya hechos solo para que el dominio conozca una relación que no necesita conocer.

### Migraciones reales, no `EnsureCreated`

La base de datos se crea con `dotnet ef migrations` + `Database.MigrateAsync()` al
arrancar, no con `EnsureCreatedAsync()` (que sí es válido y es lo que usan otras
prácticas de la unidad). Aquí se eligió así porque esta pauta pide explícitamente
"migraciones" por su nombre.

### Autenticación: enfoque manual, no Identity, sin excepciones

El tema de la unidad (`16-autenticacion.md`) trae un "Reto: autenticación con JWT en
FunkoApp" con dos enfoques (manual e Identity) y extras (OAuth2, refresh tokens, rate
limiting). Se implementó solo el **enfoque manual** (`JwtService` + `AuthService`),
que es lo que pide la pauta real de la práctica — Identity, OAuth2 y los extras son
los "puntos extra" del reto, no un requisito de esta fase.

Dos adaptaciones del código tal cual lo trae el tema, ambas para seguir las normas ya
aplicadas al resto del proyecto:
- La entidad `User` del tema lleva `[Required]`/`[MaxLength]` directamente encima.
  Aquí se mantiene limpia (sin atributos) y esas reglas van en Fluent API, igual que
  `FunkoEntity`/`CategoriaEntity`.
- El tema usa `try/catch` con excepciones (`UnauthorizedAccessException`,
  `InvalidOperationException`). Aquí `AuthService` devuelve `Result<T, DomainError>`,
  como el resto de la aplicación — se añadieron `DomainError.Unauthorized` y
  `DomainError.Conflict` para los dos casos nuevos.

### Por qué SQLite y no PostgreSQL (todavía)

La pauta permite "PostgreSQL (o SQLite en desarrollo)". Se implementó SQLite primero,
con migraciones reales y probado también dentro de Docker, para no disparar el
bloque de trabajo de esta fase. El paquete de PostgreSQL (Npgsql) ya está instalado
y el código elige el proveedor según `DatabaseSettings:Provider`; activarlo del todo
(con su propio `docker-compose` y perfil Production) queda para la fase 12.

### El puerto del contenedor no estaba publicado

Al preparar este README comprobamos `docker compose up` de verdad (no solo que
compilase el Dockerfile) y la API no respondía desde el host. El contenedor
arrancaba y escuchaba bien en el 8080 **dentro** de la red de Docker, pero
`compose.yaml` no tenía ninguna sección `ports`, así que ese puerto nunca llegaba al
host. Se añadió `ports: ["8090:8080"]` (8090 porque el 8080 de esta máquina ya estaba
en uso por otro proceso) y se volvió a comprobar: Swagger y la creación de un Funko
responden correctamente desde fuera del contenedor.

---

## 🧰 Tecnologías

| Para qué | Qué se usa |
|---|---|
| API REST | ASP.NET Core con controladores |
| Errores sin excepciones | CSharpFunctionalExtensions (`Result<T, DomainError>`) |
| Documentación | Swashbuckle.AspNetCore (Swagger UI en `/swagger`) |
| Logging | Serilog (consola + fichero con rotación diaria, nivel por perfil) |
| Persistencia | EF Core (SQLite en Dev; PostgreSQL con Npgsql, pendiente de activar) |
| Autenticación | JWT (`Microsoft.AspNetCore.Authentication.JwtBearer`) + BCrypt (`BCrypt.Net-Next`) |
| Inyección de dependencias | `Microsoft.Extensions.DependencyInjection` + Scrutor |
| Contenedores | Dockerfile multi-etapa + Docker Compose |
| Tests | NUnit + Moq + FluentAssertions |

---

## 📜 De dónde viene este proyecto

Este repositorio parte del CRUD inicial (en memoria, con DI básica) que hizo Jesús
Cobo en [`FunkoApi`](https://github.com/JesusCoboArrogante/FunkoApi). Se clonó en
local y se ha ido corrigiendo y ampliando por fases desde ahí, documentado paso a
paso en [`BITACORA.md`](BITACORA.md). De la base original se arreglaron, entre otras
cosas: un error que impedía compilar, un controlador que ignoraba por completo la
inyección de dependencias, dos endpoints con la misma ruta, y la ausencia de
`Create`, de manejo de excepciones y de documentación Swagger — el detalle completo
está en la Entrada 0 de la bitácora.

---

## 🚧 Qué queda fuera (por ahora)

Siguiendo la pauta de la práctica, fases que todavía no se han abordado:

- **Fase 3 (Minimal API):** omitida a propósito, ver
  [Decisiones de diseño](#-decisiones-de-diseño).
- ~~**Fase 8 — Configuración y logs**~~ ✅ Hecha: Serilog a consola y fichero (rotación
  diaria), nivel configurable por perfil (`appsettings.json`/`appsettings.Development.json`)
  y log estructurado de crear/actualizar/borrar en `ServiceFunko`.
- ~~**Fase 9 — Persistencia real**~~ ✅ Hecha con SQLite: EF Core, migraciones reales
  y relación Uno a Muchos con `Categoria`. **Falta PostgreSQL** para el perfil
  Production (decisión de alcance, ver Decisiones de diseño) — aparcado a la fase 12.
- ~~**Fase 10 — Seguridad**~~ ✅ Hecha: JWT + BCrypt, lectura pública, escritura
  autenticada, borrado solo `ADMIN`. Enfoque manual, no Identity (ver Decisiones de
  diseño).
- ~~**Fase 11 — Tests de integración**~~ ✅ Hecha: 14 tests con `WebApplicationFactory`
  contra un servidor real en memoria, incluida la forma de los errores. Falta solo
  un caso de borde opcional: probar un token realmente expirado.
- **Fase 12 (parcial) — Despliegue:** el Dockerfile multi-etapa y el `docker-compose`
  ya funcionan (puerto publicado, verificado en vivo), pero solo levantan la API; no
  hay base de datos que levantar junto a ella porque todavía no existe.
