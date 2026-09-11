# Base técnica de persistencia con patrón genérico (Repository + Specification)

> Módulo: `MedicalAppointments.Patient`
> Fecha: 2026-09-11
> Objetivo: proveer, como **proyecto de biblioteca de clases independiente**, la base de
> algoritmos para la gestión de persistencia donde **todas las operaciones CRUD se realizan
> sobre objetos genéricos**, evitando reescribir el mismo acceso a datos para cada entidad.

---

## 1. Resumen de la solución

Se agregó un **proyecto de tipo biblioteca de clases** nuevo, `Persistence.Generic`
(`src/Persistence.Generic/Persistence.Generic.csproj`), que contiene el algoritmo de
persistencia genérica sobre EF Core. Es un proyecto propio (no una carpeta dentro de
`Persistence`): tiene su propio `.csproj`, su propia entrada en el `.slnx` y sólo depende de
`Core` (por los contratos) y del paquete `Microsoft.EntityFrameworkCore` — **no** depende de
`AppDbContext` ni de SQL Server, por lo que es reutilizable desde cualquier proyecto que tenga
su propio `DbContext`.

| Capa / Proyecto | Contenido |
|------|-----------|
| Dominio — `src/Domain` | Contrato `IEntity<TKey>` (marca de entidad genérica). |
| Core (aplicación) — `src/Core` | Contratos `IGenericRepository<TEntity,TKey>`, `ISpecification<TEntity>` y `BaseSpecification<TEntity>`. |
| **Persistence.Generic (biblioteca de clases nueva)** — `src/Persistence.Generic` | Algoritmo: `GenericRepository<TEntity,TKey>` y `SpecificationEvaluator<TEntity>`. |
| Persistencia de la app — `src/Persistence` | Referencia a `Persistence.Generic`; sólo aporta `AppDbContext`, configuraciones EF, migraciones, repositorios concretos y el registro en DI. |

Diagrama de dependencias (flechas = "referencia a"):

```
Api ──> Persistence ──> Persistence.Generic ──> Core ──> Domain
                 └────────────────────────────────┘
        (Persistence también referencia Core directamente)
```

Con esto, dar de alta un repositorio para una entidad nueva **no requiere escribir código**
de acceso a datos: basta con que la entidad implemente `IEntity<TKey>` y esté mapeada en el
`DbContext` de turno.

---

## 2. El proyecto de biblioteca `Persistence.Generic`

`src/Persistence.Generic/Persistence.Generic.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\Core\Core.csproj" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.19" />
  </ItemGroup>
</Project>
```

Añadido al solution file `MedicalAppointments.Patient.slnx` dentro de la carpeta
`/C.Persistence/`, junto al proyecto `Persistence`.

### 2.1 `Persistence.Generic.GenericRepository<TEntity, TKey>`
`src/Persistence.Generic/GenericRepository.cs`

- Implementación **única** de `IGenericRepository<TEntity,TKey>` sobre EF Core.
- Recibe un `Microsoft.EntityFrameworkCore.DbContext` **genérico** (no `AppDbContext`), lo
  que hace la biblioteca independiente de cualquier aplicación concreta.
- Obtiene el `DbSet<TEntity>` con `context.Set<TEntity>()`, por lo que sirve para cualquier
  entidad `IEntity<TKey>` sin escribir código adicional.
- Todos los métodos son `virtual` para que un repositorio concreto pueda ajustar un
  comportamiento puntual (p. ej. el orden por defecto de `GetAllAsync`).
- Algoritmo destacado — **resolución dinámica de la clave primaria**
  (`private Expression<Func<TEntity,bool>> KeyEquals(TKey id)`):
  1. En el constructor se lee el nombre real de la propiedad clave desde los metadatos
     del modelo: `context.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties[0].Name`.
  2. `KeyEquals` construye en tiempo de ejecución el árbol de expresión
     `e => EF.Property<TKey>(e, keyName) == id`.
  3. Así `GetByIdAsync` / `RemoveByIdAsync` funcionan aunque cada entidad nombre su
     clave distinto (`PacienteId`, `ContactoEmergenciaId`, …) y sin exigir una columna `Id`.

Funciones CRUD implementadas:

| Grupo | Firma | Descripción |
|-------|-------|-------------|
| Read | `GetByIdAsync(TKey id, CancellationToken)` | Busca por clave primaria, sin *tracking*. |
| Read | `FirstOrDefaultAsync(Expression<Func<TEntity,bool>>, CancellationToken)` | Primer elemento que cumple el predicado. |
| Read | `GetAllAsync(CancellationToken)` | Todas las entidades. |
| Read | `ListAsync(Expression<Func<TEntity,bool>>, CancellationToken)` | Filtrado por predicado. |
| Read | `ListAsync(ISpecification<TEntity>, CancellationToken)` | Filtrado + includes + orden + paginación declarativos. |
| Read | `ExistsAsync(Expression<Func<TEntity,bool>>, CancellationToken)` | Existencia (`ANY`). |
| Read | `CountAsync(Expression<Func<TEntity,bool>>?, CancellationToken)` | Conteo total o filtrado. |
| Create | `AddAsync(TEntity, CancellationToken)` | Marca para inserción. |
| Create | `AddRangeAsync(IEnumerable<TEntity>, CancellationToken)` | Inserción masiva. |
| Update | `Update(TEntity)` | Marca como modificada. |
| Delete | `Remove(TEntity)` / `RemoveRange(IEnumerable<TEntity>)` | Marca para eliminación. |
| Delete | `RemoveByIdAsync(TKey id, CancellationToken)` | Carga por clave y marca para eliminación; `false` si no existe. |
| Persist | `SaveChangesAsync(CancellationToken)` | Confirma los cambios pendientes. |

### 2.2 `Persistence.Generic.SpecificationEvaluator<TEntity>`
`src/Persistence.Generic/SpecificationEvaluator.cs`

Traductor `ISpecification<TEntity>` → `IQueryable<TEntity>`. Aplica en orden:
`AsNoTracking` → `Where` → `Include`(s) → `OrderBy`/`OrderByDescending` → `Skip` → `Take`.

---

## 3. Contratos en `Core` (sin cambios de ubicación)

Se mantienen en `src/Core/Interfaces/Persistence/` porque son **contratos de aplicación**
(puertos), no implementación:

- `IGenericRepository<TEntity, TKey>` — contrato CRUD genérico (implementado por
  `Persistence.Generic.GenericRepository<,>`).
- `ISpecification<TEntity>` / `BaseSpecification<TEntity>` — patrón *Specification* para
  consultas reutilizables (criterio, `Includes`, `OrderBy`/`OrderByDescending`, `Skip`/`Take`,
  `AsNoTracking`).

> Las operaciones de escritura **no** persisten hasta `SaveChangesAsync`. Como el `DbContext`
> y el repositorio genérico se registran con ciclo de vida *Scoped*, dentro de una misma
> petición todos los repositorios comparten el mismo contexto: basta un `SaveChangesAsync`
> para confirmar en bloque los cambios de varias entidades.

`Domain.Abstractions.IEntity<TKey>` (`src/Domain/Abstractions/IEntity.cs`) tampoco se movió:
es una marca sin dependencias, correcta en la capa de dominio.

```csharp
public interface IEntity<out TKey> where TKey : notnull
{
    TKey Id { get; }
}
```

---

## 4. Cambios en el proyecto `Persistence` (app-específico)

| Archivo | Cambio |
|---------|--------|
| `src/Persistence/Persistence.csproj` | Agrega `<ProjectReference Include="..\Persistence.Generic\Persistence.Generic.csproj" />`. |
| `src/Persistence/Repositories/PatientsRepository.cs` | Deriva de `Persistence.Generic.GenericRepository<Paciente,long>`. Sólo redefine `GetAllAsync` (orden por apellidos/nombres), implementa `ExistsByDocumentAsync` (vía `ExistsAsync` genérico) y envuelve `Add/Update/Delete` con `SaveChangesAsync` inmediato. |
| `src/Persistence/Repositories/ContactosEmergenciaRepository.cs` | Igual criterio: deriva del genérico, sólo redefine `GetAllAsync`, agrega `GetByPacienteIdAsync` y el guardado inmediato. |
| `src/Persistence/Extension.cs` | Registra `DbContext` → mismo `AppDbContext` (`AddScoped<DbContext>(sp => sp.GetRequiredService<AppDbContext>())`, necesario porque `GenericRepository<,>` pide `DbContext`, no `AppDbContext`), el genérico abierto `AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>))` y deduplica las dos sobrecargas de `AddPersistence`. |
| `src/Core/Interfaces/Repository/IPatients.cs` | `IPatientsRepository` hereda de `IGenericRepository<Paciente,long>`; sólo conserva `ExistsByDocumentAsync`, `UpdateAsync`, `DeleteAsync`. |
| `src/Core/Interfaces/Repository/IContactosEmergenciaRepository.cs` | `IContactosEmergenciaRepository` hereda de `IGenericRepository<ContactoEmergencia,long>`; sólo conserva `GetByPacienteIdAsync`, `UpdateAsync`, `DeleteAsync`. |
| `src/Domain/Model/Paciente.cs` | Implementa `IEntity<long>`; agrega `[NotMapped] public long Id => PacienteId;`. |
| `src/Domain/Model/ContactoEmergencia.cs` | Implementa `IEntity<long>`; agrega `[NotMapped] public long Id => ContactoEmergenciaId;`. |

> **Compatibilidad**: los repositorios concretos `PatientsRepository` y
> `ContactosEmergenciaRepository` conservan la semántica de *guardar de inmediato* en
> `Add/Update/Delete` para no alterar el comportamiento de los *handlers* MediatR ni de los
> controladores actuales. El código nuevo debería preferir `IGenericRepository<,>` y
> controlar el `SaveChangesAsync` explícitamente.
>
> El alias `Id` va marcado con `[NotMapped]`: EF Core lo ignora y `dotnet ef migrations
> has-pending-model-changes` confirma que **no hay cambios de esquema** (no se requiere
> nueva migración).

---

## 5. Uso

### 5.1 CRUD genérico directo (inyectar el repositorio genérico)

```csharp
public sealed class EjemploHandler(IGenericRepository<Paciente, long> repo)
{
    public async Task Run(CancellationToken ct)
    {
        Paciente? p = await repo.GetByIdAsync(1, ct);

        await repo.AddAsync(new Paciente { /* ... */ }, ct);
        await repo.SaveChangesAsync(ct);                 // la escritura se confirma aquí

        bool hay = await repo.ExistsAsync(x => x.Activo, ct);
        int total = await repo.CountAsync(x => x.Ciudad == "San Salvador", ct);
    }
}
```

### 5.2 Varias entidades en un mismo guardado

Los repositorios genéricos comparten el `DbContext` de la petición (Scoped), así que un
único `SaveChangesAsync` confirma en bloque los cambios de todas las entidades:

```csharp
public sealed class AltaPacienteConContacto(
    IGenericRepository<Paciente, long> pacientes,
    IGenericRepository<ContactoEmergencia, long> contactos)
{
    public async Task Run(Paciente paciente, ContactoEmergencia contacto, CancellationToken ct)
    {
        await pacientes.AddAsync(paciente, ct);
        await contactos.AddAsync(contacto, ct);
        await pacientes.SaveChangesAsync(ct);   // guarda paciente + contacto en una sola operación
    }
}
```

Si se necesita una transacción explícita con `Rollback`, se puede inyectar el `AppDbContext`
y usar `context.Database.BeginTransactionAsync(...)` en el caso de uso concreto.

### 5.3 Consulta reutilizable con Specification

```csharp
public sealed class ContactosActivosDePacienteSpec : BaseSpecification<ContactoEmergencia>
{
    public ContactosActivosDePacienteSpec(long pacienteId, int page, int size)
        : base(c => c.PacienteId == pacienteId && c.Activo)
    {
        AddInclude(c => c.Paciente);
        ApplyOrderBy(c => c.Prioridad);
        ApplyPaging((page - 1) * size, size);
    }
}

// ...
var contactos = await repo.ListAsync(new ContactosActivosDePacienteSpec(10, 1, 20), ct);
```

---

## 6. Verificación

```bash
cd MedicalAppointments.Patient
dotnet build          # Compilación correcta. 0 Advertencia(s) 0 Errores
dotnet ef migrations has-pending-model-changes --project src/Persistence --startup-project src/Api
# -> No changes have been made to the model since the last migration.
```

Los *endpoints* existentes (`/patients`, contactos de emergencia) siguen funcionando sin
cambios porque los repositorios concretos mantienen su contrato público.

---

## 7. Cómo agregar persistencia para una entidad nueva

1. Crear la entidad en `src/Domain/Model` e implementar `IEntity<TKey>`
   (`public TKey Id => MiEntidadId;` con `[NotMapped]`).
2. Mapearla en `AppDbContext` (`DbSet` + `IEntityTypeConfiguration`).
3. Inyectar `IGenericRepository<MiEntidad, TKey>` donde se necesite. **No hay más código.**
4. (Opcional) Crear una interfaz `IMiEntidadRepository : IGenericRepository<MiEntidad, TKey>`
   con consultas específicas y una clase que derive de
   `Persistence.Generic.GenericRepository<MiEntidad, TKey>`; registrarla en `Extension.cs`.

## 8. Reutilización en otros módulos

Como `Persistence.Generic` sólo depende de `Core` (contratos) y de EF Core, cualquier otro
módulo de la solución (por ejemplo un futuro `MedicalAppointments.Appointments`) puede
referenciar este mismo proyecto y reutilizar `GenericRepository<TEntity,TKey>` sin duplicar
código, siempre que sus entidades implementen `IEntity<TKey>` y su propio `DbContext` las
tenga mapeadas.
