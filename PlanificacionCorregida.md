# Propuesta de Proyecto: Sistema Integral de Consulta Clínica

### Prototipo funcional con .NET 9, Arquitectura Hexagonal y Angular

\---

## 1\. Visión del Proyecto

Desarrollar un sistema de gestión de consultas clínicas orientado a un **prototipo funcional y demostrable**, utilizando **C# .NET 9** con microservicios para el backend, **Angular** para el frontend y **Arquitectura Hexagonal** como criterio de organización del código.

El alcance se reduce intencionalmente a **5 módulos funcionales**, priorizando el flujo principal del sistema: autenticarse, registrar pacientes, gestionar médicos y horarios, agendar una cita, atender al paciente y registrar la información clínica y el tratamiento.

La reducción busca que el equipo pueda implementar, integrar, probar y sustentar el sistema completo sin que la cantidad de módulos y tablas convierta el proyecto en una implementación excesivamente grande para un prototipo académico.

\---

## 2\. Delimitación del Alcance

La propuesta original contemplaba 6 microservicios y 31 entidades/tablas. Para el prototipo se establece una delimitación más estricta:

* **5 módulos funcionales**.
* **15 tablas** como máximo para la implementación inicial.
* Se mantiene **MedicalAppointments.Patient sin modificar su funcionalidad ya implementada**.
* Se priorizan las operaciones necesarias para completar un flujo clínico de principio a fin.
* Los elementos que agregan complejidad administrativa, pero no son indispensables para demostrar el sistema, quedan fuera del prototipo.

### Fuera de alcance

No se implementarán en esta etapa:

* Hospitalización.
* Facturación.
* Aseguradoras y convenios.
* Gestión avanzada de permisos.
* Auditoría detallada.
* Control de sesiones persistentes.
* Turnos de espera física.
* Antecedentes médicos/familiares como módulos independientes.
* Alergias, hábitos y síntomas como catálogos independientes.
* Catálogo CIE-10 y relación Atención-Diagnóstico.
* Catálogo de síntomas y relación Atención-Síntoma.
* Relación muchos-a-muchos Médico\_Especialidad.
* Otros módulos administrativos que no intervienen en el flujo principal.

Estas funcionalidades pueden quedar como extensiones futuras sin formar parte de los criterios de aceptación del prototipo.

\---

# 3\. Los 5 módulos del prototipo:

### En esta versión se está usando un módulo por cada microservicio.

## Módulo 1 — MedicalAppointments.Patient

**Estado:** ya implementado.

**Regla:** no modificar la funcionalidad existente de `MedicalAppointments.Patient`. La planificación de los demás módulos debe adaptarse al contrato y a los identificadores que este módulo ya utiliza.

### Base de datos actual

* **Base de datos:** `MedicalAppointments\_Patient`
* **Motor:** Microsoft SQL Server
* **Tablas implementadas:** `dbo.Pacientes` y `dbo.Contactos\_Emergencia`

### Tabla `dbo.Pacientes`

La tabla actual contiene los siguientes campos:

|Campo|Tipo|Restricciones / descripción|
|-|-|-|
|`PacienteID`|`BIGINT IDENTITY(1,1)`|PK. Identificador único del paciente.|
|`CodigoPaciente`|`VARCHAR(50)`|NOT NULL, UNIQUE. Código interno del paciente.|
|`TipoDocumento`|`VARCHAR(30)`|NOT NULL. Tipo de documento de identificación.|
|`NumeroDocumento`|`VARCHAR(50)`|NOT NULL. Número de documento.|
|`Nombres`|`NVARCHAR(100)`|NOT NULL. Nombres del paciente.|
|`Apellidos`|`NVARCHAR(100)`|NOT NULL. Apellidos del paciente.|
|`FechaNacimiento`|`DATE`|NOT NULL. Fecha de nacimiento.|
|`Sexo`|`CHAR(1)`|NOT NULL. Valores permitidos: `M`, `F`, `O`.|
|`EstadoCivil`|`VARCHAR(30)`|NULL. Estado civil.|
|`Telefono`|`VARCHAR(30)`|NULL. Teléfono principal.|
|`TelefonoSecundario`|`VARCHAR(30)`|NULL. Teléfono alternativo.|
|`Email`|`VARCHAR(150)`|NULL. Correo electrónico.|
|`Direccion`|`NVARCHAR(300)`|NULL. Dirección del paciente.|
|`Ciudad`|`NVARCHAR(100)`|NULL. Ciudad de residencia.|
|`Pais`|`NVARCHAR(100)`|NULL. País de residencia.|
|`Ocupacion`|`NVARCHAR(150)`|NULL. Ocupación del paciente.|
|`TipoSangre`|`VARCHAR(10)`|NULL. Tipo de sangre.|
|`Activo`|`BIT`|NOT NULL, DEFAULT `1`. Permite activar/desactivar al paciente.|
|`FechaRegistro`|`DATETIME2`|NOT NULL, DEFAULT `SYSDATETIME()`. Fecha de registro.|

### Restricciones e índices de `Pacientes`

* `PK\_Pacientes` sobre `PacienteID`.
* `UQ\_Pacientes\_Codigo` sobre `CodigoPaciente`.
* `UQ\_Pacientes\_Documento` sobre `(TipoDocumento, NumeroDocumento)`.
* `CK\_Pacientes\_Sexo` restringe `Sexo` a `M`, `F` u `O`.
* `IX\_Pacientes\_Nombre` sobre `(Apellidos, Nombres)` para facilitar búsquedas por nombre.

### Tabla `dbo.Contactos\_Emergencia`

|Campo|Tipo|Restricciones / descripción|
|-|-|-|
|`ContactoEmergenciaID`|`BIGINT IDENTITY(1,1)`|PK. Identificador del contacto.|
|`PacienteID`|`BIGINT`|NOT NULL, FK hacia `dbo.Pacientes(PacienteID)`.|
|`NombreCompleto`|`NVARCHAR(200)`|NOT NULL. Nombre del contacto de emergencia.|
|`Parentesco`|`NVARCHAR(100)`|NULL. Relación con el paciente.|
|`Telefono`|`VARCHAR(30)`|NOT NULL. Teléfono principal del contacto.|
|`TelefonoSecundario`|`VARCHAR(30)`|NULL. Teléfono alternativo.|
|`Email`|`VARCHAR(150)`|NULL. Correo electrónico.|
|`Prioridad`|`INT`|NOT NULL, DEFAULT `1`, debe ser mayor que `0`.|
|`Activo`|`BIT`|NOT NULL, DEFAULT `1`. Estado del contacto.|

### Restricciones e índices de `Contactos\_Emergencia`

* `PK\_Contactos\_Emergencia` sobre `ContactoEmergenciaID`.
* `FK\_Contactos\_Pacientes` hacia `dbo.Pacientes(PacienteID)`.
* `CK\_Contacto\_Prioridad` exige que `Prioridad > 0`.
* `IX\_ContactosEmergencia\_Paciente` sobre `PacienteID`.

### Relación

```text
Pacientes (1)
     │
     │ PacienteID
     │
     └──────────────< Contactos\_Emergencia (N)
```

Un paciente puede tener uno o varios contactos de emergencia y cada contacto pertenece a un único paciente.

### Integración con los demás módulos

Los demás módulos **no deben crear copias de la tabla `Pacientes`**. Cuando `Appointment` o `ClinicalCare` necesiten identificar a un paciente, utilizarán el `PacienteID` proporcionado por `MedicalAppointments.Patient`.

Al tratarse de microservicios independientes, estas referencias se manejarán como identificadores externos y se validarán mediante comunicación entre APIs. No se crearán claves foráneas físicas entre las bases de datos de distintos módulos.

\---

## Módulo 2 — MedicalAppointments.Security

**Responsabilidad:** autenticación y rol básico de acceso.

Se reduce Security a lo indispensable para permitir iniciar sesión como **Recepción** o **Médico** y emitir un JWT.

### Tablas

#### Usuarios

|Campo|Tipo sugerido|Descripción|
|-|-|-|
|Id|int / GUID|Identificador|
|Username|varchar|Usuario de acceso|
|PasswordHash|varchar|Contraseña almacenada como hash|
|Nombre|varchar|Nombre del usuario|
|RolId|int|Rol asignado|
|MedicoId|int / GUID, nullable|Médico relacionado si el usuario es médico|
|Activo|bool|Estado de la cuenta|

#### Roles

|Campo|Tipo sugerido|Descripción|
|-|-|-|
|Id|int|Identificador|
|Nombre|varchar|Recepcion, Medico, Administrador|

### Se elimina del prototipo

* Permisos.
* Rol\_Permiso.
* Sesiones\_Usuario.
* Auditoría.

El control de acceso se realizará mediante **roles incluidos en el JWT**.

\---

## Módulo 3 — MedicalAppointments.Staff

**Responsabilidad:** administrar los médicos, especialidades, consultorios y disponibilidad necesaria para agendar citas.

### Tablas

#### Medicos

|Campo|Tipo sugerido|Descripción|
|-|-|-|
|Id|int / GUID|Identificador|
|Nombres|varchar|Nombres|
|Apellidos|varchar|Apellidos|
|Licencia|varchar|Número de licencia|
|EspecialidadId|int|Especialidad principal|
|Telefono|varchar|Teléfono|
|Email|varchar|Correo|
|Activo|bool|Estado|

#### Especialidades

|Campo|Tipo sugerido|Descripción|
|-|-|-|
|Id|int|Identificador|
|Nombre|varchar|Cardiología, Pediatría, etc.|
|Activo|bool|Estado|

#### Consultorios

|Campo|Tipo sugerido|Descripción|
|-|-|-|
|Id|int|Identificador|
|Nombre|varchar|Nombre/número del consultorio|
|Ubicacion|varchar|Ubicación|
|Activo|bool|Estado|

#### HorariosMedicos

|Campo|Tipo sugerido|Descripción|
|-|-|-|
|Id|int / GUID|Identificador|
|MedicoId|int / GUID|Médico|
|DiaSemana|int|Día de la semana|
|HoraInicio|time|Inicio de disponibilidad|
|HoraFin|time|Fin de disponibilidad|
|ConsultorioId|int|Consultorio|
|Activo|bool|Estado|

### Simplificación aplicada

Se elimina `Medico\_Especialidad` y se establece **una especialidad principal por médico**. Esto es suficiente para el prototipo y evita una tabla de relación muchos-a-muchos que no aporta valor al flujo principal.

\---

## Módulo 4 — MedicalAppointments.Appointment

**Responsabilidad:** gestionar la agenda y el estado de las citas.

### Tabla

#### Citas

|Campo|Tipo sugerido|Descripción|
|-|-|-|
|Id|int / GUID|Identificador|
|PacienteId|int / GUID|ID del paciente|
|MedicoId|int / GUID|ID del médico|
|ConsultorioId|int|Consultorio asignado|
|FechaHora|datetime|Fecha y hora|
|Motivo|varchar/text|Motivo de consulta|
|Estado|varchar|Programada, Confirmada, EnAtencion, Atendida, Cancelada|
|Observacion|varchar/text|Observación de agenda|

### Simplificación aplicada

No se crea la tabla `Estados\_Cita`; el estado se manejará como **enum/string controlado** dentro de la aplicación.

No se implementa `Turnos`, ya que la cola física de pacientes no es necesaria para demostrar el objetivo principal.

### Comunicación con otros módulos

`Appointment` almacena solamente los IDs de:

* Paciente.
* Médico.
* Consultorio.

La existencia de esos registros se valida mediante comunicación con `Patient` y `Staff`. No se crean claves foráneas físicas entre bases de datos.

\---

## Módulo 5 — MedicalAppointments.ClinicalCare

Este módulo combina las funcionalidades que originalmente estaban separadas en:

* `MedicalRecord`.
* `Treatment`.

**Responsabilidad:** registrar la atención médica y el tratamiento asociado.

### Tablas

#### Atenciones

|Campo|Tipo sugerido|Descripción|
|-|-|-|
|Id|int / GUID|Identificador|
|CitaId|int / GUID|Cita atendida|
|PacienteId|int / GUID|Paciente|
|MedicoId|int / GUID|Médico|
|FechaHora|datetime|Fecha de atención|
|Diagnostico|varchar/text|Diagnóstico principal|
|Estado|varchar|Abierta/Cerrada|

#### SignosVitales

|Campo|Tipo sugerido|Descripción|
|-|-|-|
|Id|int / GUID|Identificador|
|AtencionId|int / GUID|Atención|
|PresionArterial|varchar|Ej. 120/80|
|FrecuenciaCardiaca|decimal|Latidos por minuto|
|Temperatura|decimal|Temperatura|
|SaturacionOxigeno|decimal|Saturación|
|Peso|decimal|Peso|
|Talla|decimal|Talla|

#### NotasMedicas

|Campo|Tipo sugerido|Descripción|
|-|-|-|
|Id|int / GUID|Identificador|
|AtencionId|int / GUID|Atención|
|Nota|text|Evolución, valoración, hallazgos y observaciones|
|FechaHora|datetime|Fecha de registro|

#### Medicamentos

|Campo|Tipo sugerido|Descripción|
|-|-|-|
|Id|int / GUID|Identificador|
|Nombre|varchar|Nombre|
|PrincipioActivo|varchar|Principio activo|
|Presentacion|varchar|Tableta, jarabe, etc.|
|Concentracion|varchar|Concentración|
|Activo|bool|Estado|

#### Prescripciones

|Campo|Tipo sugerido|Descripción|
|-|-|-|
|Id|int / GUID|Identificador|
|AtencionId|int / GUID|Atención|
|PacienteId|int / GUID|Paciente|
|Fecha|datetime|Fecha de prescripción|
|IndicacionesGenerales|text|Indicaciones|

#### PrescripcionDetalle

|Campo|Tipo sugerido|Descripción|
|-|-|-|
|Id|int / GUID|Identificador|
|PrescripcionId|int / GUID|Prescripción|
|MedicamentoId|int / GUID|Medicamento|
|Dosis|varchar|Dosis|
|Frecuencia|varchar|Cada cuánto|
|Duracion|varchar|Duración|
|ViaAdministracion|varchar|Oral, intravenosa, etc.|

### Simplificación aplicada

Se elimina la separación entre `Expedientes\_Clínicos` y `Atenciones`. Para este prototipo, el **historial clínico se obtiene a partir de las atenciones registradas del paciente**.

También se evita implementar como tablas independientes:

* Antecedentes\_Médicos.
* Antecedentes\_Familiares.
* Alergias.
* Hábitos.
* Síntomas.
* Diagnósticos.
* Atención\_Diagnóstico.
* Atención\_Síntoma.

El diagnóstico principal se registra directamente en `Atenciones`.

\---

# 4\. Resumen de tablas

|Módulo|Tablas|Cantidad|
|-|-|-:|
|Patient|Pacientes, ContactosEmergencia|2|
|Security|Usuarios, Roles|2|
|Staff|Medicos, Especialidades, Consultorios, HorariosMedicos|4|
|Appointment|Citas|1|
|ClinicalCare|Atenciones, SignosVitales, NotasMedicas, Medicamentos, Prescripciones, PrescripcionDetalle|6|
|**Total**||**15**|

La reducción de **31 a 15 tablas** disminuye considerablemente el trabajo de modelado, migraciones, endpoints, validaciones y pruebas, sin perder el flujo principal del sistema.

\---

# 5\. Flujo funcional del prototipo

El criterio de completitud será que el sistema pueda ejecutar correctamente este recorrido:

1. **Security:** un usuario con rol `Recepcion` inicia sesión y obtiene un JWT.
2. **Patient:** Recepción registra un nuevo paciente.
3. **Staff:** Recepción consulta médicos, especialidades, consultorios y horarios disponibles.
4. **Appointment:** Recepción agenda una cita para el paciente.
5. **Appointment:** el médico inicia sesión y consulta sus citas.
6. **Appointment:** el médico cambia la cita a `EnAtencion`.
7. **ClinicalCare:** el médico registra la atención asociada a la cita.
8. **ClinicalCare:** registra signos vitales, diagnóstico y nota médica.
9. **ClinicalCare:** genera una prescripción con uno o varios medicamentos.
10. **Appointment:** la cita cambia a `Atendida`.
11. **ClinicalCare:** el historial del paciente puede consultarse mediante sus atenciones registradas.

Este recorrido mantiene la idea central del planteamiento original: **Paciente → Agenda → Atención → Tratamiento**, pero con una cantidad de componentes manejable.

\---

# 6\. Arquitectura Hexagonal

Cada módulo puede conservar la estructura hexagonal definida en la propuesta original:

```text
MedicalAppointments.{Module}
│
├── Domain
│   └── Entidades, reglas de negocio y puertos
│
├── Core
│   └── Casos de uso, servicios de aplicación, DTOs y puertos de entrada
│
├── Persistence
│   └── EF Core, DbContext, repositorios y migraciones
│
├── Infrastructure
│   └── Clientes HTTP, JWT y comunicación externa
│
└── Api
    └── Controllers, contratos, Swagger y configuración
```

Regla de dependencia:

```text
Api → Core → Domain

Persistence → Domain/Core
Infrastructure → Domain/Core

Domain NO depende de Persistence, Infrastructure ni Api.
```

\---

# 7\. Persistencia y comunicación

Para conservar el enfoque de microservicios:

* Cada módulo mantiene su propia base de datos.
* No se comparten tablas entre módulos.
* Las referencias a entidades externas se almacenan como IDs.
* La comunicación entre módulos se realiza mediante APIs HTTP.
* `Core` consume interfaces/puertos, evitando depender directamente de HTTP.

### Relaciones principales

```text
Patient
   │
   └──── PacienteId ────> Appointment

Staff
   │
   ├──── MedicoId ──────> Appointment
   └──── ConsultorioId ─> Appointment

Appointment
   │
   └──── CitaId ────────> ClinicalCare

ClinicalCare
   │
   └──── AtencionId ────> Prescripciones / SignosVitales / NotasMedicas
```

\---

# 8\. Angular

El frontend se organizará por las mismas funcionalidades principales:

```text
src/app/
├── auth/
├── patients/
├── staff/
├── appointments/
└── clinical-care/
```

Se recomienda lazy loading para:

* Pacientes.
* Agenda/Citas.
* Atención clínica.
* Tratamientos.

El frontend debe consumir los APIs mediante servicios Angular y mantener los DTOs alineados con los contratos publicados por cada módulo.

\---

# 9\. Ruta de implementación revisada

|Parte|Actividades|
|-|-|
|**Parte 1**|Configuración del repositorio, solución .NET 9, estructura hexagonal de los 5 módulos, configuración de Angular y definición de bases de datos.|
|**Parte 2**|Finalizar/verificar `Patient` sin alterar su funcionalidad. Implementar `Security` con usuarios, roles y JWT.|
|**Parte 3**|Implementar `Staff` y `Appointment`, incluyendo médicos, especialidades, horarios, consultorios y agenda.|
|**Parte 4**|Implementar `ClinicalCare`, incluyendo atención, signos vitales, notas, medicamentos y prescripciones.|
|**Parte 5**|Integración completa con Angular, pruebas de extremo a extremo, Swagger, validaciones, manejo de errores, documentación y preparación de la sustentación.|

\---

# 10\. Criterio final de éxito

El proyecto se considerará funcional cuando un evaluador pueda realizar una demostración completa:

```text
LOGIN
  ↓
REGISTRAR PACIENTE
  ↓
CONSULTAR MÉDICO Y HORARIO
  ↓
AGENDAR CITA
  ↓
MÉDICO CONSULTA CITA
  ↓
INICIAR ATENCIÓN
  ↓
REGISTRAR SIGNOS VITALES
  ↓
REGISTRAR DIAGNÓSTICO Y NOTA
  ↓
GENERAR PRESCRIPCIÓN
  ↓
MARCAR CITA COMO ATENDIDA
  ↓
CONSULTAR HISTORIAL DEL PACIENTE
```

La finalidad no es implementar la mayor cantidad posible de módulos, sino entregar un **prototipo integrado, coherente, demostrable y técnicamente defendible**, manteniendo la arquitectura hexagonal y la separación de responsabilidades.

