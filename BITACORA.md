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

<!-- La siguiente entrada se añade aquí, con el mismo formato: Qué se hizo / Decisión y porqué / Alternativas descartadas / Pendiente -->
