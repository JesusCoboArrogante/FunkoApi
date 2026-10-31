# FunkoApp

![.NET](https://img.shields.io/badge/.NET-10-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-14-239120?style=flat-square&logo=csharp&logoColor=white)
![Tests](https://img.shields.io/badge/tests-31%20passing-2ea44f?style=flat-square)
![Docker](https://img.shields.io/badge/Docker-multi--stage-2496ED?style=flat-square&logo=docker&logoColor=white)
![Swagger](https://img.shields.io/badge/OpenAPI-Swagger-85EA2D?style=flat-square&logo=swagger&logoColor=black)

Práctica 3 de Desarrollo Web en Entorno Servidor (DWES), unidad 2: una API REST para
gestionar un catálogo de Funkos (figuras de vinilo coleccionables).

**Autores:** Cristian Álvarez · Jesús Cobo (base inicial del proyecto, ver
[De dónde viene este proyecto](#-de-dónde-viene-este-proyecto))

---

## 🎯 Qué hace

CRUD completo sobre un catálogo de Funkos en memoria, con validación en dos capas,
errores de dominio sin excepciones, documentación Swagger y despliegue con Docker.

| Operación | Verbo | Ruta | Éxito |
|---|---|---|---|
| Listar todos los Funkos | `GET` | `/api/Funko` | `200` |
| Consultar un Funko por id | `GET` | `/api/Funko/{id}` | `200` / `404` |
| Buscar Funkos por nombre | `GET` | `/api/Funko/search?nombre=` | `200` |
| Crear un Funko | `POST` | `/api/Funko` | `201` |
| Actualizar un Funko completo | `PUT` | `/api/Funko/{id}` | `200` / `404` |
| Actualizar solo el precio | `PATCH` | `/api/Funko/{id}` | `200` / `404` |
| Eliminar un Funko (borrado lógico) | `DELETE` | `/api/Funko/{id}` | `204` / `404` |

---

## 🏗️ Arquitectura

```
Program.cs → FunkoController → ServiceFunko → FunkoValidator
                                             → RepositoryFunko   (en memoria, Dictionary<long, Funko>)
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

**31 tests**, con NUnit, Moq y FluentAssertions, patrón AAA (Arrange, Act, Assert) y
separados en `CasosPositivos` y `CasosNegativos`.

| Qué se prueba | Nº |
|---|---|
| `DomainError` | 3 |
| `FunkoMapper` | 3 |
| `FunkoValidator` | 7 |
| `RepositoryFunko` (en memoria, sin mocks) | 7 |
| `ServiceFunko` (repositorio y validador mockeados) | 11 |

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

### Por qué el repositorio sigue siendo síncrono

El repositorio es un `Dictionary` en memoria: no hay ninguna E/S que esperar todavía.
Convertirlo a `async`/`Task` ahora sería ceremonia sin beneficio real. Se pasará a
asíncrono cuando llegue EF Core (persistencia real), que es cuando aparece la E/S que
justifica el `async`.

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
- **Fase 9 — Persistencia real:** el repositorio sigue en memoria (`Dictionary`). Falta
  EF Core con PostgreSQL/SQLite, la relación con `Categoria` y las migraciones.
- **Fase 10 — Seguridad:** no hay JWT ni BCrypt; todas las operaciones son públicas.
- **Fase 11 (parcial) — Tests de integración:** hay 31 tests unitarios, pero ninguno
  de integración con `WebApplicationFactory` todavía.
- **Fase 12 (parcial) — Despliegue:** el Dockerfile multi-etapa y el `docker-compose`
  ya funcionan (puerto publicado, verificado en vivo), pero solo levantan la API; no
  hay base de datos que levantar junto a ella porque todavía no existe.
