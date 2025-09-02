# Documento Técnico: Arquitectura por Capas - Acudir.Test.Apis

##  1. Capa Domain (Dominios y Contratos)

### Implementado

* Entidad `Persona` con propiedades básicas.
* Interfaz `IPersonaRepository` como contrato de persistencia.

###  Por qué se implementó así

* Seguimos el principio **Dependency Inversion** del SOLID: las dependencias están definidas por interfaces, no por implementaciones.
* El dominio es la capa más pura y estable de la arquitectura, sin dependencias de infraestructura o frameworks.

### Pros

* Bajo acoplamiento.
* Alta reusabilidad.
* Independiente de tecnología (testable, portable).

### Contras

* Necesidad de más código repetitivo para mantener la separación de responsabilidades.

### ⚡ Mejoras posibles

* Incorporar ValueObjects y reglas de negocio inmutables.
* Aplicar DDD si el dominio lo justifica.

---

##  2. Capa Application (CQRS + Dtos + Validaciones)

### Implementado

* Handlers CQRS (`CreatePersonaCommand`, `UpdatePersonaCommand`, etc.)
* `PersonaDto` como DTO plano.
* `PagedResult<T>` para paginación.
* `ValidationBehavior` con FluentValidation como pipeline de validación.

###  Por qué se implementó así

* Se aplicó **CQRS (Command Query Responsibility Segregation)** para separar claramente escritura (Commands) de lectura (Queries).
* `FluentValidation` permite reglas declarativas, reutilizables y limpias.

### Pros

* Escalable y testable por separado.
* Evita mezclas de lógica de negocio con lógica de entrada.
* Facilita evolución hacia DomainEvents / Pipelines más complejos.

### Contras

* Overkill para CRUDs simples.
* Más archivos y capas para tareas simples.

### Mejoras posibles

* Agregar logging, metrics y caching en behaviors.
* Usar `Result<T>` para errores tipados.

---

## 3. Capa Infrastructure (Persistencia en JSON)

### Implementado

* `PersonaRepository` implementa `IPersonaRepository` leyendo/escribiendo `Test.json`.
* Inyectado en `Startup` mediante `AddInfrastructure()`.

### Por qué se implementó así

* Uso de archivo plano `Test.json` para simplificar el entorno sin base de datos.
* Apoya principios de **Inversion of Control** (IOC) y permite mocks en tests.

### Pros

* Fácil de correr sin entorno externo.
* Aislado y mockeable.

### Contras

* No escalable.
* No tiene concurrencia, locking ni rendimiento aceptable.

### Mejoras posibles

* Reemplazar por EF Core + SQLite InMemory o MySQL en proyectos reales.
* Validar el path de `Test.json` al iniciar.

---

## 4. Capa Web (API REST + Middleware + Seguridad)

###  Implementado

* Controller `PersonasController` usa solo MediatR (sin servicios acoplados).
* Validaciones centralizadas en `ValidationBehavior`.
* Seguridad con JWT + Swagger configurado.
* Middlewares de:

  * Excepciones (ProblemDetails RFC 7807)
  * Logging (Serilog)
  * CorrelationId

### Por qué se implementó así

* Se buscó mantener el controlador, orientado a contratos REST.
* Se integró seguridad JWT y documentación con Swagger para facilitar pruebas.
* Los middlewares aseguran consistencia de logs y errores para observabilidad.

### Pros

* Compatible con OpenAPI.
* Bajo acoplamiento con infraestructura.

### Contras

* Swagger no incluye login automático ni refresh token.

### Mejoras posibles

* Agregar CORS, RateLimiting y SwaggerAuth para entornos reales.
* Versionado más granular (v1.0, v2.0...)

---

## 5. Testing (UnitTest con Moq + xUnit)

### Implementado

* Tests a `PersonasController` mockeando `IMediator`.
* Verifica flujos: GET, POST, PUT, DELETE.

### Por qué se implementó así

* Aislamos el controller y validamos sus respuestas REST sin necesidad de back.
* xUnit y Moq son livianos y ampliamente usados en .NET.

### Pros

* Independientes de infraestructura.
* Ejecutables en CI/CD.

### Contras

* No testea la lógica de negocio real (solo capa Web).

### Mejoras posibles

* Agregar tests de integración contra Infra con SQLite In-Memory.
* Usar TestContainers en entornos más realistas.

---

## 6. Docker + Deploy (Local dev)

### Implementado

* Dockerfile multistage (build + runtime).
* docker-compose expone el puerto 5001.
* Variables de entorno para JWT.

### Por qué se implementó así

* Facilita la ejecución sin instalar SDK.
* Prepara el proyecto para ser portable.

### Pros

* Rápido para levantar entornos demo.
* Compatible con CI/CD, Azure, etc.

### Contras

* No incluye healthchecks ni volumen persistente.

### Mejoras posibles

* Agregar healthcheck HTTP.
* Montar logs o archivos como volumen.
* Preparar imagen para cloud.

---

## Conclusión general

Este challenge fue implementado con principios de:

* Clean Architecture
* CQRS + MediatR
* SOLID y separación por capas
* JWT para auth + Swagger para DX
* Logging estructurado + Healthchecks

Si bien es un entorno demo, sienta bases reales y escalables para proyectos más complejos. La estructura modular, las pruebas, y la seguridad son aspectos bien cubiertos.


Consideraciones en relación a los commits y al entorno en el que se desarrolló la solución

A lo largo del desarrollo de esta solución, es posible que algunos de los commits presenten una cantidad significativa de archivos modificados o de modificaciones agrupadas (es decir, commits sucios). Todo esto ocurrió porque estuve trabajando todo el desarrollo en Visual Studio Code en macOS, y había ciertos archivos que eran generados automáticamente por el entorno (.DS_Store, .vscode, .pdb, obj, etc.) que dificultaban obtener la exclusión correcta a través de .gitignore conservar el staging de forma limpia.

Y aunque pasé parte del tiempo buscando dar respuesta a esta situación, llegar a una solución de forma que afectara poco o nada al flujo del desarrollo del proyecto fue algo en lo que no pude solucionar. En un entorno productivo o bien en un entorno de CI/CD, con respecto a estos archivos se puede operar mediante configuraciones de .gitignore y mediante workflows, razón por la cual no resulta en absoluto una dificultad técnica ni crítica; sí es algo que definitivamente tiene margen de mejora.

Git no elimina automáticamente archivos ya versionados, incluso si luego se agregan al .gitignore.

Esto generó que ciertos commits incluyeran archivos no deseados como .dll, .pdb, .cache, o archivos dentro de obj/ y bin/, lo cual no representa buenas prácticas de versionado
