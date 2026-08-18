# Propuesta de Proyecto: Sistema Integral de Consulta Clínica
### Arquitectura de Microservicios (.NET 9, Arquitectura Hexagonal) con Angular

---

## 1. Visión del Proyecto

Desarrollar un sistema de gestión clínica modular y escalable, usando **C# .NET 9**
para el backend (microservicios independientes, cada uno construido con **Arquitectura
Hexagonal**) y **Angular** para el frontend, asegurando trazabilidad, versionado en
GitHub y calidad de código bajo estándares profesionales.

---

## 2. Delimitación del Alcance (El Prototipo Funcional)

De los 60 módulos del catálogo original, se seleccionan **31 módulos** que garantizan
un sistema de consultas clínicas completo y funcional de principio a fin, descartando
lo que aporta volumen pero no valor inmediato al MVP. Quedan fuera de alcance
Hospitalización, Facturación y Aseguradoras, ya que no tendrán implementación real al no
existir facturación ni gestión de convenios en este prototipo.

Los módulos se agrupan según el microservicio dueño de cada uno (sección 4). 
El número entre paréntesis corresponde al identificador del módulo en
el catálogo original de 60 módulos.

### MedicalAppointments.Security
- Usuarios (8): cuentas de acceso al sistema asociadas a médicos, personal
  administrativo, enfermería, recepción u otros colaboradores.
- Roles (9): perfiles de acceso (administrador, médico, enfermero, recepción, etc.).
- Permisos (10): catálogo de acciones que pueden realizarse en el sistema.
- Rol_Permiso (11): relación entre roles y los permisos que tienen asignados.
- Sesiones_Usuario (59): control de sesiones activas por usuario.
- Auditoría (58): registro de acciones realizadas por los usuarios sobre el sistema.

### MedicalAppointments.Patient
- Pacientes (1): información maestra del paciente (identificación, nombres, fecha de
  nacimiento, sexo, contacto, dirección, estado civil).
- Contactos_Emergencia (2): personas a contactar en caso de emergencia.

> Se omiten Aseguradoras (3) y Paciente_Aseguradora (4): son catálogos de
> facturación/convenios sin implementación real en este MVP (no hay facturación), por
> lo que no aportan valor funcional al prototipo.

### MedicalAppointments.Staff
- Médicos (5): datos personales, licencia, especialidad y contacto.
- Especialidades (6): catálogo de especialidades médicas disponibles.
- Médico_Especialidad (7): relación entre médicos y sus especialidades.
- Consultorios (12): catálogo de espacios físicos de atención.
- Horarios_Médicos (13): disponibilidad de cada médico (días, horarios, consultorio).

### MedicalAppointments.Appointment
- Citas (14): citas programadas entre pacientes y médicos (fecha, hora, especialidad,
  motivo, consultorio, estado).
- Estados_Cita (15): catálogo de estados de una cita (programada, confirmada,
  atendida, cancelada, etc.).
- Turnos (16): control de la fila de pacientes en espera física de atención.

### MedicalAppointments.MedicalRecord
- Atenciones (17): consulta médica realizada; relaciona paciente, médico y cita.
- Expedientes_Clínicos (18): expediente médico principal, punto central del
  historial clínico del paciente.
- Antecedentes_Médicos (19) y Antecedentes_Familiares (20): historial clínico
  relevante del paciente y su familia.
- Alergias (21): alergias conocidas, reacción y severidad.
- Signos_Vitales (22): presión arterial, frecuencia cardíaca, temperatura,
  saturación de oxígeno, peso, talla, entre otros.
- Hábitos (23): actividad física, alimentación, consumo de tabaco, etc.
- Diagnósticos (24) y Atención_Diagnóstico (25): catálogo bajo clasificación CIE-10
  y su relación con cada atención.
- Síntomas (26) y Atención_Síntoma (27): catálogo de síntomas y su relación con cada
  atención.
- Notas_Médicas (28): evolución, valoración, hallazgos y observaciones clínicas.

### MedicalAppointments.Treatment
- Medicamentos (29): catálogo (nombre, principio activo, presentación, concentración).
- Prescripciones (30): cabecera de receta médica asociada a una atención y paciente.
- Prescripción_Detalle (31): detalle de medicamentos prescritos (dosis, frecuencia,
  duración, vía de administración).

---

## 3. Flujo de Trabajo del Prototipo Final

Como defensa y entrega final, el sistema debe ser capaz de completar este recorrido
exacto sin romperse, demostrando que los 6 microservicios funcionan integrados de
principio a fin:

1. **(Security)**: Un usuario con rol "Recepción" inicia sesión y recibe un token JWT.
2. **(Patient)**: El usuario de Recepción registra a un nuevo Paciente.
3. **(Staff + Appointment)**: Revisa el Horario disponible de un Médico y
   le agenda una Cita al paciente (`Appointment`), que valida los IDs de paciente y
   médico contra sus respectivos microservicios.
4. **(Security)**: El usuario "Recepción" cierra sesión y entra el usuario con rol
   "Médico" (nuevo token JWT).
5. **(Appointment)**: El médico ve su cita del día y le cambia el estado a
   "En atención".
6. **(MedicalRecord)**: El médico abre la Atención (vinculada a la Cita por ID),
   registra Signos Vitales, escribe Notas Médicas y asigna un Diagnóstico.
7. **(Treatment)**: Desde la misma Atención, el médico genera una Prescripción con
   uno o varios Medicamentos.
8. **(Appointment)**: La cita pasa a estado "Atendida", cerrando el ciclo.

Este recorrido evidencia la comunicación entre los 6
microservicios (validación de JWT emitido por `Security`, consulta de datos de
`Patient`/`Staff` desde `Appointment`, y la relación `Appointment` → `MedicalRecord`
→ `Treatment` por ID de Atención/Cita), y sirve como criterio objetivo de
"completitud" del prototipo exigido por la consigna.

---

## 4. Arquitectura de Microservicios: Hexagonal (Ports & Adapters) en .NET 9

Cada microservicio se implementa como **una solución .NET 9 independiente**, siguiendo
los principios de **Arquitectura Hexagonal** (Ports & Adapters), separando
responsabilidades en 5 proyectos. Esto permite que el núcleo del negocio (Domain/Core)
no dependa de detalles técnicos (base de datos, HTTP, frameworks), facilitando pruebas
unitarias y el reemplazo de adaptadores sin tocar la lógica de negocio.

### 4.1 Estructura estándar por microservicio

```
{Nombre}.sln
src
│
├── {Nombre}.Domain          → Entidades, Value Objects, reglas de negocio puras,
│                               interfaces de puertos de salida (ej. IPacienteRepository)
│
├── {Nombre}.Core             → Casos de uso / servicios de aplicación, orquestación
│                               de la lógica de negocio, DTOs de aplicación,
│                               interfaces de puertos de entrada
│
├── {Nombre}.Persistence      → Adaptador de salida: implementación de repositorios
│                               con EF Core, DbContext, migraciones
│
├── {Nombre}.Infrastructure   → Adaptadores de salida hacia sistemas externos:
│                               clientes HTTP a otros microservicios, JWT, logging,
│                               mensajería (si aplica)
│
└── {Nombre}.Api              → Adaptador de entrada: Controllers, DTOs de contrato,
                                Swagger/OpenAPI, configuración de autenticación
```

**Regla de dependencias**: `Api` → `Core` → `Domain`. 
`Persistence` e `Infrastructure` implementan interfaces definidas en `Domain`/`Core`, pero
`Domain` y `Core` **nunca** referencian a `Persistence`, `Infrastructure` ni `Api`.

### 4.2 Aplicación de la estructura a cada microservicio

Se aplica el mismo patrón a los 6 microservicios del sistema, bajo el prefijo
`MedicalAppointments.{Contexto}`:

| Microservicio | Responsabilidad | Entidades principales (Domain) |
|---|---|---|
| **MedicalAppointments.Security** | Autenticación y control de acceso | Usuarios, Roles, Permisos, Rol_Permiso, Sesiones_Usuario, Auditoría |
| **MedicalAppointments.Patient** | Datos de pacientes | Pacientes, Contactos_Emergencia |
| **MedicalAppointments.Staff** | Recursos médicos y físicos | Médicos, Especialidades, Medico_Especialidad, Consultorios, Horarios_Médicos |
| **MedicalAppointments.Appointment** | Agenda y turnos | Citas, Estados_Cita, Turnos |
| **MedicalAppointments.MedicalRecord** | Expediente y atención clínica | Expedientes_Clínicos, Antecedentes_Médicos, Antecedentes_Familiares, Alergias, Hábitos, Atenciones, Signos_Vitales, Síntomas, Atención_Síntoma, Diagnósticos, Atención_Diagnóstico, Notas_Médicas |
| **MedicalAppointments.Treatment** | Medicamentos y prescripciones | Medicamentos, Prescripciones, Prescripción_Detalle |

Ejemplo concreto de la solución del microservicio de Agenda:

```
MedicalAppointments.Appointment.sln
src
│
├── MedicalAppointments.Appointment.Domain
├── MedicalAppointments.Appointment.Core
├── MedicalAppointments.Appointment.Persistence
├── MedicalAppointments.Appointment.Infrastructure
└── MedicalAppointments.Appointment.Api
```

### 4.3 Persistencia y comunicación entre microservicios

**Decisión de arquitectura de datos:** cada microservicio tiene **su propia base de
datos** (patrón *database-per-service*), gestionada desde su propio proyecto como
`Persistence`. No se comparte una única base de datos entre servicios. Las referencias
entre entidades de distintos servicios (por ejemplo, una Cita que referencia a un
Paciente) se guardan como **IDs sueltos** (sin FK física entre bases). Esto permite escalar y 
desplegar cada servicio de forma independiente

La resolución de esos datos entre servicios se implementa en la capa
**`Infrastructure`** de cada microservicio (como adaptador de salida), mediante:
- **Clientes HTTP tipados** (`HttpClient` con `IHttpClientFactory`) hacia los demás
  microservicios, expuestos al `Core` a través de una interfaz de puerto (ej.
  `IPatientServiceClient`), para que el `Core` no dependa directamente de detalles de
  HTTP.
- **Endpoints batch** en el microservicio consultado, en vez de
  llamadas una-por-una para evitar el problema n+1.
- Documentación explícita de qué microservicio es "dueño" de cada
  entidad, para evitar duplicación de lógica de negocio.

---

## 5. Frontend: Angular

- **Tipado con Interfaces**: modelos TypeScript que reflejan los DTOs expuestos por
  cada proyecto `Api` de los microservicios.
- **Servicios (HttpClient)**: un servicio Angular inyectable por cada microservicio
  consumido (o por el API Gateway, si Angular pasa a consumir un único punto de
  entrada).
- **Lazy loading en Angular** Carga diferida de **código** (bundles
  JS), no de datos, por lo que reduce el bundle inicial y es buena práctica estándar 
  en Angular.
  - Módulos con lazy loading recomendados: Pacientes, Agenda/Citas, Expediente
    Clínico, Tratamientos.
  - No confundir con la composición de datos entre microservicios (sección 4), que
    se resuelve preferentemente en el backend (`Core`), no en el frontend.

---

## 6. Lineamientos de Calidad y Calificación

- **Repositorio GitHub**: se selecciona el repositorio de **uno de los dos
  integrantes** como base oficial del proyecto y ambos realizan commits 
  descriptivos y funcionales sobre ese repositorio.
- **Documentación**: Swagger/OpenAPI en el proyecto `Api` de cada microservicio.
- **Pruebas**: pruebas unitarias sobre `Core`/`Domain` (lógica de negocio pura, fácil
  de testear al no depender de infraestructura) y pruebas de integración sobre los
  endpoints principales de cada `Api` antes de la entrega final.

---

## 7. Hoja de Ruta

| Parte | Actividades |
|---|---|
| **Parte 1** | Configuración de entorno; creación de la solución .NET 9 base con la estructura hexagonal (Domain/Core/Persistence/Infrastructure/Api) para cada microservicio; estructura de carpetas Angular; definición de bases de datos por servicio; configuración de Docker Compose. | 
| **Parte 2** | Implementación de `MedicalAppointments.Security` y `MedicalAppointments.Patient` (incluye autenticación JWT). | 
| **Parte 3** | Implementación de `MedicalAppointments.Staff` y `MedicalAppointments.Appointment`, incluyendo clientes HTTP tipados en `Infrastructure` con endpoints batch (mitigación N+1). |
| **Parte 4** | Implementación de `MedicalAppointments.MedicalRecord` y `MedicalAppointments.Treatment`, y conexión completa de todos los servicios con Angular (vía API Gateway). |
| **Parte 5** | Pruebas de integración, pulido de interfaces (frontend), documentación final en Swagger y preparación de la sustentación. |
