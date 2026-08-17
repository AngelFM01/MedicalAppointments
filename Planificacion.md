# Propuesta de Proyecto: Sistema Integral de Consulta Clínica
### Arquitectura de Microservicios con .NET y Angular.

---

## 1. Visión del Proyecto

Desarrollar un sistema de gestión clínica modular y escalable, usando **C# .NET Core**
para el backend (arquitectura de microservicios) y **Angular** para el frontend,
asegurando trazabilidad, versionado en GitHub y calidad de código bajo estándares
profesionales.

---

## 2. Alcance y Delimitación de Módulos (MVP)

Se delimita el sistema a los módulos esenciales que cubren el ciclo operativo completo
de una clínica, garantizando un prototipo funcional. **Se omite intencionalmente el
módulo de Facturación y Pagos**, ya que no tiene implementación real.

### A. Módulo de Seguridad y Usuarios
- Usuarios (médicos, personal administrativo, enfermería, recepción).
- Roles y Permisos (Administrador, Médico, Recepcionista, etc.).
- Autenticación con JWT para sesiones seguras entre Angular y los microservicios.
- Auditoría de operaciones sobre el sistema.

### B. Módulo de Catálogos Maestros
- Pacientes: información maestra (identificación, nombres, fecha de nacimiento, sexo,
  contacto, dirección, estado civil).
- Contactos de Emergencia.
- Médicos: datos personales, licencia, especialidad, contacto.
- Especialidades y Consultorios.
- Aseguradoras y relación Paciente-Aseguradora (pólizas, vigencia, cobertura).

### C. Módulo de Agenda y Atención (Core Operativo)
- Horarios Médicos.
- Citas y Estados de Cita.
- Turnos (control de fila de espera física).
- Atenciones (registro clínico central: paciente/médico/cita).
- Signos Vitales.
- Síntomas y Diagnósticos (bajo clasificación CIE-10).
- Notas Médicas.

### D. Módulo de Expediente Clínico Ampliado
- Expedientes Clínicos.
- Antecedentes Médicos y Antecedentes Familiares.
- Alergias.
- Hábitos.

### E. Módulo de Tratamientos y Medicamentos
- Medicamentos (catálogo).
- Prescripciones y Prescripción_Detalle (dosis, frecuencia, duración, vía de
  administración).

---

## 3. Arquitectura de Microservicios (.NET Core)

**Decisión de arquitectura de datos:** cada microservicio tiene **su propia base de
datos** (patrón *database-per-service*). No se comparte una única base de datos entre
servicios. Las referencias entre entidades de distintos servicios (por ejemplo, una
Cita que referencia a un Paciente) se guardan como **IDs sueltos** (sin FK física entre
bases), y la resolución de esos datos se hace mediante:

- **Llamadas HTTP síncronas** entre microservicios cuando un servicio necesita
  validar/enriquecer datos de otro (ej. `Appointment.API` llama a `Patient.API` para
  validar que el `PacienteID` existe).
- **Composición en el frontend (Angular)**: para vistas que combinan datos de varios
  servicios (ej. listado de citas con nombre de paciente y médico), Angular puede
  hacer las llamadas a cada API y combinar la información en el cliente, evitando
  acoplar los microservicios entre sí.
- Se documentará explícitamente qué microservicio es "dueño" (source of truth) de
  cada entidad para evitar duplicación de lógica.

| Microservicio | Responsabilidad | Tablas / Entidades |
|---|---|---|
| **Clinic.Security.API** | Autenticación y control de acceso | Usuarios, Roles, Permisos, Rol_Permiso, Sesiones_Usuario, Auditoría |
| **Clinic.Patient.API** | Datos de pacientes y seguros | Pacientes, Contactos_Emergencia, Aseguradoras, Paciente_Aseguradora |
| **Clinic.Staff.API** | Recursos médicos y físicos | Médicos, Especialidades, Medico_Especialidad, Consultorios, Horarios_Médicos |
| **Clinic.Appointment.API** | Agenda y turnos | Citas, Estados_Cita, Turnos |
| **Clinic.MedicalRecord.API** | Expediente y atención clínica | Expedientes_Clínicos, Antecedentes_Médicos, Antecedentes_Familiares, Alergias, Hábitos, Atenciones, Signos_Vitales, Síntomas, Atención_Síntoma, Diagnósticos, Atención_Diagnóstico, Notas_Médicas |
| **Clinic.Treatment.API** | Medicamentos y prescripciones | Medicamentos, Prescripciones, Prescripción_Detalle |

---

## 4. Frontend: Angular

- **Tipado con Interfaces**: modelos TypeScript que reflejan los DTOs expuestos por
  cada microservicio.
- **Servicios (HttpClient)**: un servicio Angular inyectable por cada microservicio
  consumido.
- **Arquitectura Modular con Lazy Loading**: módulos independientes para pacientes,
  agenda, expediente clínico y tratamientos, cargados solo cuando se navega a ellos.
- **Composición de datos en cliente**: para vistas que cruzan información de varios
  microservicios, se combinan las respuestas en el componente/servicio
  Angular correspondiente.

---

## 5. Lineamientos de Calidad y Calificación

- **Trazabilidad en GitHub**: Repositorio único y público. Ambos integrantes deben realizar 
  commits descriptivos y funcionales.
- **IA como Soporte**: el uso de IA es válido para agilizar el desarrollo, pero ambos
  integrantes deben poder explicar cualquier seccion de código durante la revisión
  docente; de lo contrario la nota preliminar será penalizada según la consigna.
- **Documentación**: Swagger/OpenAPI en cada microservicio.

---

## 6. Hoja de Ruta

| Parte | Actividades |
|---|---|
| **Parte 1** | Configuración de entorno, creación de la solución .NET (estructura de microservicios), estructura de carpetas Angular, definición de bases de datos por servicio y configuración de Docker Compose. |
| **Parte 2** | Implementación de MedicalAppointments.Security y MedicalAppointments.Patient |
| **Parte 3** | Implementación de `Clinic.Staff.API` y `Clinic.Appointment.API`, incluyendo la comunicación HTTP entre servicios para validación de IDs. |
| **Parte 4** | Implementación de `Clinic.MedicalRecord.API` y `Clinic.Treatment.API`, y conexión completa de todos los servicios con Angular (vía API Gateway). |
| **Parte 5** | Pruebas de integración, pulido de interfaces (frontend), documentación final en Swagger y preparación de la sustentación. |