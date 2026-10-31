# Bitácora de trabajo — API de Funkos

Documento vivo. Una entrada por paso de trabajo, en orden cronológico (la más reciente abajo). Pensado para que Cristian pueda repasarlo y exponerlo en clase: cada entrada dice **qué se hizo, por qué, qué se descartó y qué queda pendiente**.

Normas de esta bitácora (ver `CLAUDE.md` en `Entorno-Servidor/`, puntos 7 y 9):
- Antes de cada comando con efecto (crear rama, modificar fichero, commit), se explica qué hace y se espera confirmación.
- Cada entrada queda cerrada con lo que falta, para no tener que releer todo el historial del chat.
- Nada se sube a GitHub por ahora: todo el trabajo es local hasta que Cristian decida lo contrario.

---

## Entrada 0 — Contexto y punto de partida

**Fecha:** 2026-10-31

**Qué se hizo:**
- Se revisó la pauta oficial de la práctica (`DesarrolloWebEntornosServidor-02-2026-2027/practicas/03-practica-api-funkos.md`, de José Luis García), que define 12 fases: recurso → diseño en papel → Minimal API → controladores → DI → `Result<T>` → DTOs/validadores → config/logs → EF Core → JWT/BCrypt → Swagger/tests → Docker.
- Se clonó temporalmente el repo de un compañero, Jesús Cobo (`https://github.com/JesusCoboArrogante/FunkoApi`), solo para inspección (se borró después), y se auditó contra la pauta y contra nuestras normas de `CLAUDE.md`.
- Se redactó el informe `informe-funko-api.md` (en `Entorno-Servidor/`) con el hallazgo completo. Resumen de lo crítico encontrado en el código de Jesús:
  1. **No compila**: `RepositoryFunko.Update()` usa `funko.Imagen`, propiedad que no existe en el modelo `Funko`.
  2. **El controlador no usa la inyección de dependencias**: crea `new RepositoryFunko()` a mano en un campo `static`, así que el `ServiceFunko` (que tiene las validaciones) nunca se ejecuta.
  3. **Ruta duplicada**: `Update` y `Patch` mapeados los dos a `POST {id}` → error en tiempo de ejecución.
  4. **Falta el `Create`** — el CRUD está incompleto.
  5. **Verbos HTTP equivocados** según la propia tabla de la pauta (debería ser `PUT`/`PATCH`/`GET` según el caso, no todo `POST`).
  6. Falta la propiedad **`Stock`**, exigida por la pauta.
  7. `ExceptionHandlerMiddleware` existe pero está **vacío y sin registrar**.
  8. **Swagger no está configurado** pese a tener el paquete referenciado.
  9. Problemas de entrega: sin tests, sin `.gitignore` (hay `bin/`/`obj/`/`.idea/` comiteados), README roto por encoding, solo 2 commits llamados "first commit".

**Decisión tomada:** partir del código de Jesús en vez de reescribir desde cero.
- **Por qué:** los fallos son localizados y mecánicos (una propiedad, una inyección de dependencias, dos rutas, un middleware vacío) — se arreglan en horas, no en días. El esqueleto de carpetas, el `Dockerfile` multi-etapa y el registro de DI con Scrutor ya están bien planteados y se reaprovechan.
- **Alternativa descartada:** empezar de cero. Se descartó porque tirar el esqueleto que ya funciona (estructura de carpetas, Dockerfile, DI) costaría más tiempo que corregir los bugs puntuales, y además metería más presión sobre la fecha de entrega sin beneficio real.

**Decisión tomada:** clonar en local sin hacer fork ni tocar GitHub.
- **Por qué:** Cristian pidió explícitamente no subir nada a GitHub por ahora y trabajar solo en local. El repo se clonó como `Entorno-Servidor/FunkoApp` (carpeta renombrada respecto al original `FunkoApi`, para ya acercarse al nombre `FunkoApp` que exige el formato de entrega). El único remoto configurado es `origin` → el repo de Jesús, pero no se va a hacer push ahí.
- **Alternativa descartada (de momento):** hacer fork en GitHub inmediatamente. Sigue siendo la recomendación para cuando se decida trabajar en remoto (deja constancia pública de que se parte del trabajo de Jesús), pero se aparca hasta que Cristian lo indique.

**Qué queda pendiente:**
- Arreglar el build (fase 1 de la hoja de ruta: rama `feature/fix-build`).
- Decidir si y cuándo se hace el fork/push a GitHub.
- Seguir documentando cada paso en esta bitácora antes de avanzar al siguiente.

---

## Entrada 1 — `feature/fix-build`: arreglar lo que no compilaba

**Fecha:** 2026-10-31

**Qué se hizo:**
1. Ramas: creada `dev` a partir de `main` (rama de trabajo por defecto, según `CLAUDE.md`), y `feature/fix-build` a partir de `dev`.
2. `Models/Funko.cs`: añadida la propiedad `Stock` (exigida por la pauta y ausente); renombradas `CreateAt`→`CreatedAt`, `UpdateAt`→`UpdatedAt` (coinciden ahora con el nombre exacto de la pauta); sustituida `IsActivo` por `IsDeleted` (invirtiendo la condición en todo el código que la usaba); eliminada `DeleteAt` (no la pide la pauta y quedaba redundante con `IsDeleted`); añadidos valores por defecto a `Nombre`/`Categoria` para evitar warnings de nulabilidad.
3. `Repository/RepositoryFunko.cs`: quitada la asignación a `Imagen` (la propiedad que no existía y rompía la compilación); actualizados todos los usos a los nuevos nombres; `SoftDelete` ahora marca `IsDeleted = true` en vez de rellenar una fecha que nadie leía; `Update` ahora también copia `Stock`.
4. Verificado con `dotnet build`: **compila correctamente** (0 errores). Aparecen 2 warnings `ASP0023` por la colisión de rutas `Update`/`Patch` — es el bug nº3 del informe, ya identificado, pendiente de la fase de rutas.
5. Al compilar se generaron `bin/`/`obj/` y se vio en vivo el problema nº10 del informe (sin `.gitignore`, binarios trackeados). Se resolvió en el mismo bloque: creado `.gitignore` (`bin/`, `obj/`, `.vs/`, `.idea/`, ficheros de usuario) y `git rm --cached` sobre lo que ya estaba versionado.
6. Commits atómicos en `feature/fix-build`: `fix: corregir propiedades del modelo Funko y referencias rotas en el repositorio`, `chore: anadir gitignore y dejar de versionar binarios e idea`, `docs: anadir bitacora de trabajo`. Fusionados en `dev` con `--no-ff` (deja constancia del merge en el historial) y rama `feature/fix-build` borrada tras el merge.

**Decisión tomada:** eliminar `DeleteAt` en vez de mantenerlo junto a `IsDeleted`.
- **Por qué:** la pauta solo exige `IsDeleted`; mantener los dos campos obligaría a sincronizarlos siempre a la vez y es una fuente de bugs (que uno se actualice y el otro no). Un único booleano es suficiente para el borrado lógico que pide la práctica.
- **Alternativa descartada:** conservar `DeleteAt` como fecha de auditoría. Se aparca — si en la fase de EF Core hiciera falta auditoría más fina, se puede reintroducir como campo de solo lectura, no como sustituto de `IsDeleted`.

**Qué queda pendiente:**
- El bug nº2 (controlador sin inyectar el servicio) y nº3 (rutas duplicadas, confirmado ahora también por warning del compilador) — siguiente bloque.
- Bug nº4 (falta `Create`) y nº5 (verbos HTTP) — mismo bloque de CRUD/rutas.
- Middleware de excepciones vacío y Swagger sin configurar — bloques posteriores.
- `.idea/` sigue en el working directory en disco (es normal, lo genera Rider); ya no se versiona.

## Entrada 2 — `feature/controller-rest`: DI real, CRUD completo, verbos correctos

**Fecha:** 2026-10-31

**Qué se hizo:**
1. Rama `feature/controller-rest` desde `dev`.
2. Reescrito `Controller/FunkoController.cs` de cero:
   - Recibe `IServiceFunko` por **primary constructor** (antes: `new RepositoryFunko()` en un campo `static`, que ignoraba por completo la DI y las validaciones del servicio). Ahora toda petición pasa por `ServiceFunko`.
   - Añadido el **`Create`** que faltaba (`[HttpPost]`, devuelve `201 Created` con `CreatedAtAction`).
   - `Update` completo ahora es **`PUT`** (antes `POST`, duplicada con el patch).
   - Actualización de precio ahora es **`PATCH`** (antes `POST`, misma ruta que `Update` → colisión).
   - `Search` ahora es **`GET /api/Funko/search?nombre=...`** (antes `POST`, no tenía sentido como búsqueda de solo lectura).
3. Verificación en tres capas:
   - **Compila**: `dotnet build` → 0 errores, 0 warnings (han desaparecido los `ASP0023` de rutas duplicadas).
   - **Arranca**: `dotnet run` levanta en `http://localhost:5050` sin errores.
   - **Funciona de verdad**: probado con `curl` el ciclo completo — crear (201), listar, consultar por id, actualizar completo (PUT), actualizar precio (PATCH), buscar por nombre, borrar (204, borrado lógico) y comprobar que tras el borrado el GET devuelve 404. Todo correcto.
4. Un commit atómico (`fix: inyectar IServiceFunko en el controlador y completar el CRUD con los verbos correctos`), fusionado en `dev` con `--no-ff`, rama borrada.

**Decisión tomada:** el body de `UpdatePrice` (PATCH) se recibe como `decimal` plano en el body, no como un diccionario genérico (`Dictionary<string, object>`, que era como lo tenía Jesús).
- **Por qué:** el diccionario genérico obligaba a mirar claves por string (`"Precio"`) y castear `JsonElement` a mano — frágil y sin tipado. Al ser un único campo, un `decimal` en el body es más simple y sigue el principio de "si él no usa una técnica más compleja sin necesidad, nosotros tampoco la metemos".
- **Alternativa descartada:** mantener el patch genérico tipo JSON Patch (RFC 6902). Se descarta por ahora porque la pauta no lo pide explícitamente y añadiría complejidad que no toca en esta fase.

**Qué queda pendiente:**
- El `Service.Add`/`Update` siguen lanzando `ArgumentException` en validaciones (nombre vacío, precio negativo) y **no hay middleware que las capture** — de momento esas peticiones devuelven un 500 crudo en vez de un `ProblemDetails` 400. Es el bug nº7 del informe, siguiente bloque.
- Swagger sigue sin configurar (bug nº8).
- DTOs de entrada/salida (fase 7 de la pauta) — por ahora el controlador expone el propio `Funko` de dominio, aceptable en esta fase pero a corregir antes de EF Core.

## Entrada 3 — `feature/middleware-swagger`: manejo de errores y documentación

**Fecha:** 2026-10-31

**Qué se hizo:**
1. Rama `feature/middleware-swagger` desde `dev`.
2. Implementado `Middleware/ExceptionHandlerMiddleware.cs` de verdad (antes solo tenía los `using`, ninguna clase):
   - Captura `ArgumentException` (las validaciones de `ServiceFunko`) → `400 Bad Request` con `ProblemDetails`.
   - Captura cualquier otra excepción → `500 Internal Server Error` con `ProblemDetails` genérico (sin filtrar el mensaje interno al cliente) y log con `ILogger`.
   - Registrado en `Program.cs` con `app.UseMiddleware<ExceptionHandlerMiddleware>()`, antes de `MapControllers()`.
3. Swagger: el paquete que ya traía el proyecto (`Microsoft.AspNetCore.OpenApi`) **no sirve una UI**, solo genera el documento OpenAPI nativo de .NET. Para cumplir el requisito de entrega ("debe arrancar y servir la documentación en `/swagger`") se instaló `Swashbuckle.AspNetCore` (10.3.1) y se añadió `AddSwaggerGen()` + `UseSwagger()` + `UseSwaggerUI()`.
4. Verificación en tres capas:
   - **Compila**: 0 errores (tuvimos que matar antes un proceso `FunkoAppi01.exe` que había quedado corriendo de la verificación del bloque anterior y bloqueaba el `.exe` — nada relacionado con el código, solo higiene del entorno).
   - **Arranca**: `dotnet run` sin errores.
   - **Funciona de verdad**: `GET /swagger/index.html` → 200, `GET /swagger/v1/swagger.json` → 200. `POST /api/Funko` con nombre vacío o precio negativo → ahora devuelve `400` con `{"title":"Solicitud invalida","detail":"..."}` en vez del 500 crudo de antes.
5. Dos commits en la rama (`feat: implementar el middleware...`, `chore: anadir Swashbuckle...`), fusionados en `dev` con `--no-ff`, rama borrada.

**Decisión tomada:** usar `Swashbuckle.AspNetCore` en vez del soporte nativo de OpenAPI que ya traía el proyecto.
- **Por qué:** el paquete nativo (`Microsoft.AspNetCore.OpenApi` + `MapOpenApi()`) solo expone el JSON, no una interfaz visual en `/swagger`; la entrega pide explícitamente esa ruta con documentación servida. Swashbuckle es además la opción estándar que se usa en el resto de ejemplos de la asignatura.
- **Alternativa descartada:** quedarnos con `MapOpenApi()` nativo y montar nosotros una UI estática. Más trabajo para el mismo resultado, sin ninguna ventaja para esta fase.

**Nota de transparencia:** el commit `feat: implementar el middleware...` incluye también las dos líneas de `Program.cs` que registran los servicios de Swagger (`AddEndpointsApiExplorer`/`AddSwaggerGen`), porque tocaban el mismo fichero en la misma edición y separarlas habría significado dos ediciones manuales del mismo archivo sin beneficio real; el `UseSwagger()`/`UseSwaggerUI()` y el paquete en sí quedaron en el commit `chore` aparte. No es una violación grave de "un propósito por commit", pero se deja constancia aquí tal como pide el punto 9 de `CLAUDE.md`.

**Qué queda pendiente:**
- DTOs de entrada/salida y validador de dominio (fase 7 de la pauta) — el controlador sigue aceptando/devolviendo el `Funko` de dominio directamente.
- Las validaciones del servicio siguen siendo `ArgumentException`, no `Result<T, DomainError>` — lo correcto según nuestra arquitectura, pendiente para cuando ataquemos esa fase.
- README real con instrucciones + justificación de decisiones (bug nº11 del informe) — sigue sin hacerse.
- Historial de commits: ya llevamos 8 commits atómicos de fases propias desde que se retomó el proyecto, pero los 2 "first commit" de Jesús siguen en la base del historial (es correcto dejarlos, documentan de dónde partimos).

## Entrada 4 — `feature/dtos-validators`: DTOs, `Result<T, DomainError>`, validador y mapper

**Fecha:** 2026-10-31

**Antes de tocar código:** se revisó `RepositorioRemoto/Errors/DomainError.cs`, `Validators/UserValidator.cs`, `Mappers/UserMapper.cs` y `Services/UserService.cs` para copiar exactamente su patrón (regla 1 de `CLAUDE.md`: mirar el material antes de proponer). Se confirmó: `abstract record DomainError(string Message)` con records anidados + factoría estática fuera; validador que devuelve `Result<T, DomainError>` con estilo if/return (nunca `Bind`/`Map`); mapeadores como métodos de extensión; servicio que llama al validador, comprueba duplicados contra el repositorio y traduce todo a `Result`.

**Qué se hizo:**
1. Rama `feature/dtos-validators` desde `dev`. Paquete `CSharpFunctionalExtensions` 3.7.0 (misma versión que `RepositorioRemoto`).
2. `Errors/DomainError.cs`: `DomainError.NotFound`, `.Validation`, `.AlreadyExists` + factoría `FunkoErrors`.
3. `Dto/CreateFunkoDto.cs`, `UpdateFunkoDto.cs`, `FunkoDto.cs`: con Data Annotations (`[Required]`, `[StringLength]`, `[Range]`), tal como pide la pauta ("Valida con Data Annotations y un validador de dominio").
4. `Validators/IFunkoValidator.cs` + `FunkoValidator.cs`: comprueba lo que las Data Annotations no cubren bien (nombre/categoría solo con espacios).
5. `Mappers/FunkoMapper.cs`: `ToFunko()` (desde los DTOs de entrada) y `ToDto()` (hacia el DTO de salida).
6. `Repository`: añadido `ExistsByName(nombre, excludeId)` — sigue devolviendo un valor plano (`bool`), la regla de negocio de "nombre duplicado" la decide el servicio, no el repositorio.
7. `ServiceFunko`/`IServiceFunko`: reescritos para trabajar con DTOs de entrada/salida y devolver `Result<T, DomainError>` en vez de lanzar excepciones. `Add`/`Update` llaman primero al validador, luego comprueban duplicados, y si todo va bien delegan en el repositorio.
8. `FunkoController`: ya no expone el modelo de dominio, habla en DTOs; un único método privado `ToActionResult` traduce cada tipo de `DomainError` a su código HTTP (`NotFound`→404, `AlreadyExists`→409, `Validation`→400), todo como `ProblemDetails`.
9. `Infraestructure/ValidatorsConfig.cs`: nuevo, registra `IFunkoValidator` en el contenedor DI, igual que ya existía para repositorios y servicios.
10. Verificación en tres capas: compila (0 errores, 0 warnings); arranca sin errores; probado en vivo con `curl` — creación (201), **duplicado de nombre (409)**, nombre en blanco (400), precio negativo (400), no encontrado (404) en `GetById` y `Update`, patch de precio, borrado lógico y búsqueda.
11. Dos commits (`feat: anadir DomainError, DTOs...` y `refactor: usar Result<T, DomainError>...`), fusionados en `dev` con `--no-ff`.

**Hallazgo durante la prueba en vivo:** al mandar un nombre con solo espacios (`"   "`), la respuesta 400 vino del `ModelState` automático de `[ApiController]` (por el `[Required]`), **no** de nuestro `FunkoValidator` — resulta que `RequiredAttribute` de .NET recorta espacios antes de comprobar. Esto significa que, llamando por HTTP, el validador de dominio queda casi siempre "tapado" por las Data Annotations para ese caso concreto.
- **Decisión tomada:** dejar las dos capas de todos modos. El validador de dominio sigue sirviendo de red de seguridad si en el futuro se llama al servicio sin pasar por el `ModelState` (por ejemplo, desde un test unitario que construye el DTO a mano, o desde otro punto de entrada que no sea HTTP). No es redundancia inútil, es defensa en profundidad — y la pauta pide explícitamente las dos técnicas.
- **Alternativa descartada:** quitar la comprobación de blancos del validador de dominio ya que "nunca se ejecuta vía HTTP". Se descarta porque sería optimizar para un único punto de entrada y dejaría el servicio desprotegido si se usa desde otro sitio.

**Decisión tomada (recordatorio de la pregunta de antes del bloque):** servicio y repositorio de Funko se quedan síncronos por ahora.
- **Por qué:** decisión de Cristian — seguir el estilo de José Luis sin salirse del temario; como la pauta no especifica async en esta fase y el repositorio sigue sin E/S real, no hay ninguna instrucción del temario que obligue a ello todavía. Se retomará en la fase 9 (EF Core), cuando aparezca E/S real, igual que hace `RepositorioRemoto`.

**Qué queda pendiente:**
- README real con instrucciones, justificación de decisiones (bug nº11 del informe original).
- Tests unitarios (`NUnit` + `Moq` + `FluentAssertions`, `CasosPositivos`/`CasosNegativos`) — ninguno todavía.
- Configuración en `appsettings` + Serilog (fase 8 de la pauta).
- Persistencia real con EF Core (fase 9) — aquí es donde se decidirá si el repositorio pasa a ser async.
- Seguridad JWT + BCrypt (fase 10).
- El modelo `Funko` sigue siendo mutable (`{ get; set; }`) en vez de un `record` posicional inmutable como `User` en `RepositorioRemoto` — item de estilo nº15 del informe, pendiente de limpieza cuando toquemos el modelo otra vez (probablemente en la fase de EF Core).

## Entrada 5 — `feature/tests`: proyecto de tests `FunkoApp.Test`

**Fecha:** 2026-10-31

**Antes de tocar código:** se revisó `RepositorioRemoto.Tests` (estructura de carpetas, `UserValidatorTests.cs`, `DomainErrorTests.cs`, `UserServiceTests.cs`) para copiar exactamente su estilo: mismas versiones de paquetes (`NUnit` 4.3.2, `Moq` 4.20.72, `FluentAssertions` 6.12.2), `[TestFixture]` + clases anidadas `CasosPositivos`/`CasosNegativos`, comentarios `// Arrange` `// Act` `// Assert`, datos de prueba propios (Pikachu/Charmander/Bulbasaur, no copiados de los ejemplos del profesor).

**Qué se hizo:**
1. Rama `feature/tests` desde `dev`. Proyecto `FunkoApp.Test` creado con `dotnet new nunit`, referencia a `FunkoAppi01`, paquetes `FluentAssertions`, `Moq` y `CSharpFunctionalExtensions` en las mismas versiones que usa `RepositorioRemoto`. Añadido a `FunkoAppi01.sln`.
2. `Errors/DomainErrorTests.cs`: comprueba que cada tipo de `DomainError` construye el mensaje correcto (3 tests).
3. `Validators/FunkoValidatorTests.cs`: `CasosPositivos` (datos válidos) y `CasosNegativos` (nombre/categoría en blanco, varios campos a la vez) (7 tests).
4. `Mappers/FunkoMapperTests.cs`: las conversiones DTO↔Funko copian todos los campos (3 tests).
5. `Repository/RepositoryFunkoTests.cs`: contra el repositorio real en memoria (sin mocks, es lo que se está probando): borrado lógico oculta el Funko de `GetAll`/`GetById`, `ExistsByName` se ignora a sí mismo al excluir su propio Id, `Update`/`SoftDelete` sobre un Id inexistente devuelven `null`/`false` (7 tests).
6. `Services/ServiceFunkoTests.cs`: repositorio y validador **mockeados** con Moq. Aquí es donde se verifica no solo el resultado sino el cortocircuito, tal como pide el punto 4 de `CLAUDE.md`:
   - `Add_ValidacionFalla_NoDeberiaLlamarAlRepositorioAdd` → `_mockRepository.Verify(r => r.Add(...), Times.Never)`: si el validador falla, el repositorio **no debe tocarse**.
   - `Add_NombreDuplicado_NoDeberiaLlamarAlRepositorioAdd` → mismo `Times.Never`, ahora por nombre duplicado.
   - `UpdatePrice_PrecioNegativo_NoDeberiaLlamarAlRepositorio` → `Times.Never` sobre `UpdatePrecio`.
   - Más los casos positivos (`Times.Once` implícito al verificar que sí se llama) y los `NotFound` de `GetById`/`Update`/`Delete` (11 tests).
7. Verificación: `dotnet build` de toda la solución → 0 errores, 0 warnings. **`dotnet test` → 31/31 en verde.**
8. 6 commits atómicos (el proyecto en sí, y un commit por carpeta de tests), fusionados en `dev` con `--no-ff`.

**Decisión tomada:** testear `RepositoryFunko` directamente (sin mocks) en vez de mockear algo dentro de él.
- **Por qué:** es un repositorio en memoria, no hay nada que simular — simularlo sería probar el mock, no el código. Se mockea en `ServiceFunkoTests` porque ahí sí hay una dependencia externa real que aislar.
- **Alternativa descartada:** usar TestContainers ya para el repositorio. Se descarta porque TestContainers es para una base de datos real (regla 4 de `CLAUDE.md`); aquí no hay ninguna todavía — llegará en la fase 9 con EF Core, momento en el que este test se sustituirá por uno de integración contra PostgreSQL/SQLite real.

**Qué queda pendiente:**
- Tests de integración del endpoint con `WebApplicationFactory` (fase 11 de la pauta) — de momento solo hay tests unitarios.
- README real con instrucciones, cómo ejecutar los tests y justificación de decisiones.
- Configuración en `appsettings` + Serilog (fase 8).
- Persistencia EF Core (fase 9) — cuando llegue, `RepositoryFunkoTests` pasará a usar TestContainers con una base de datos real, como exige la norma.

<!-- La siguiente entrada se añade aquí, con el mismo formato: Qué se hizo / Decisión y porqué / Alternativas descartadas / Pendiente -->
