# Base técnica de persistencia con patrón genérico (Repository + Specification)

> Módulo: `MedicalAppointments.Patient`
> Fecha: 2026-09-03
> Objetivo: proveer la base de algoritmos para la gestión de persistencia donde **todas las
> operaciones CRUD se realizan sobre objetos genéricos**, evitando reescribir el mismo
> acceso a datos para cada entidad.

---

## 1. Resumen de la solución

Se implementó una **biblioteca de persistencia genérica** distribuida en dos capas ya
existentes de la arquitectura limpia del módulo:

| Capa | Proyecto | Contenido nuevo |
|------|----------|-----------------|
| Dominio | `src/Domain` | Contrato `IEntity<TKey>` (marca de entidad genérica). |
| Core (aplicación) | `src/Core` | Contratos `IGenericRepository<TEntity,TKey>`, `ISpecification<TEntity>` y `BaseSpecification<TEntity>`. |
| Persistencia | `src/Persistence` | Implementaciones `GenericRepository<TEntity,TKey>`, `SpecificationEvaluator<TEntity>` y registro en DI. |

Con esto, dar de alta un repositorio para una entidad nueva **no requiere escribir código**:
basta con que la entidad implemente `IEntity<TKey>` y esté mapeada en `AppDbContext`.

---

## 2. Clases e interfaces agregadas

### 2.1 `Domain.Abstractions.IEntity<TKey>`
`src/Domain/Abstractions/IEntity.cs`

```csharp
public interface IEntity<out TKey> where TKey : notnull
{
    TKey Id { get; }
}
```

- Marca genérica que identifica a cualquier entidad persistible por su clave primaria.
- Se usa como **restricción de tipo** (`where TEntity : class, IEntity<TKey>`) en el
  repositorio genérico.
- `TKey` es libre: `long`, `int`, `Guid`, `string`, etc.

### 2.2 `Core.Interfaces.Persistence.IGenericRepository<TEntity, TKey>`
`src/Core/Interfaces/Persistence/IGenericRepository.cs`

Contrato CRUD completo sobre objetos genéricos. Métodos:

| Grupo | Firma | Descripción |
|-------|-------|-------------|
| Read | `Task<TEntity?> GetByIdAsync(TKey id, CancellationToken)` | Busca por clave primaria, sin *tracking*. |
| Read | `Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity,bool>>, CancellationToken)` | Primer elemento que cumple el predicado. |
| Read | `Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken)` | Todas las entidades. |
| Read | `Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity,bool>>, CancellationToken)` | Filtrado por predicado. |
| Read | `Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity>, CancellationToken)` | Filtrado + includes + orden + paginación declarativos. |
| Read | `Task<bool> ExistsAsync(Expression<Func<TEntity,bool>>, CancellationToken)` | Existencia (`ANY`). |
| Read | `Task<int> CountAsync(Expression<Func<TEntity,bool>>?, CancellationToken)` | Conteo total o filtrado. |
| Create | `Task<TEntity> AddAsync(TEntity, CancellationToken)` | Marca para inserción. |
| Create | `Task AddRangeAsync(IEnumerable<TEntity>, CancellationToken)` | Inserción masiva. |
| Update | `TEntity Update(TEntity)` | Marca como modificada. |
| Delete | `void Remove(TEntity)` / `void RemoveRange(IEnumerable<TEntity>)` | Marca para eliminación. |
| Delete | `Task<bool> RemoveByIdAsync(TKey id, CancellationToken)` | Carga por clave y marca para eliminación; `false` si no existe. |
| Persist | `Task<int> SaveChangesAsync(CancellationToken)` | Confirma los cambios pendientes. |

> Las operaciones de escritura **no** persisten hasta `SaveChangesAsync`. Como el `AppDbContext`
> y el repositorio genérico se registran con ciclo de vida *Scoped*, dentro de una misma
> petición todos los repositorios comparten el mismo contexto: basta un `SaveChangesAsync`
> para confirmar en bloque los cambios de varias entidades.

### 2.3 `Core.Interfaces.Persistence.ISpecification<TEntity>` + `BaseSpecification<TEntity>`
`src/Core/Interfaces/Persistence/ISpecification.cs`, `BaseSpecification.cs`

Patrón *Specification*: describe una consulta reutilizable (criterio, `Includes`,
`OrderBy` / `OrderByDescending`, `Skip` / `Take`, `AsNoTracking`).
`BaseSpecification<TEntity>` es la clase base; las especificaciones concretas heredan y
componen la consulta con métodos protegidos: `Where`, `AddInclude`, `ApplyOrderBy`,
`ApplyOrderByDescending`, `ApplyPaging`, `EnableTracking`.

### 2.4 `Persistence.Repositories.Generic.GenericRepository<TEntity, TKey>`
`src/Persistence/Repositories/Generic/GenericRepository.cs`

- Implementación **única** de `IGenericRepository<TEntity,TKey>` sobre EF Core.
- Obtiene el `DbSet<TEntity>` con `context.Set<TEntity>()`, por lo que sirve para
  cualquier entidad sin código adicional.
- Todos los métodos son `virtual` para que un repositorio concreto pueda ajustar un
  comportamiento puntual (p. ej. el orden por defecto).
- Algoritmo destacado — **resolución dinámica de la clave primaria**
  (`private Expression<Func<TEntity,bool>> KeyEquals(TKey id)`):
  1. En el constructor se lee el nombre real de la propiedad clave desde los metadatos
     del modelo: `context.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties[0].Name`.
  2. `KeyEquals` construye en tiempo de ejecución el árbol de expresión
     `e => EF.Property<TKey>(e, keyName) == id`.
  3. Así `GetByIdAsync` / `RemoveByIdAsync` funcionan aunque cada entidad nombre su
     clave distinto (`PacienteId`, `ContactoEmergenciaId`, …) y sin exigir una columna `Id`.

### 2.5 `Persistence.Repositories.Generic.SpecificationEvaluator<TEntity>`
`src/Persistence/Repositories/Generic/SpecificationEvaluator.cs`

Traductor `ISpecification<TEntity>` → `IQueryable<TEntity>`. Aplica en orden:
`AsNoTracking` → `Where` → `Include`(s) → `OrderBy`/`OrderByDescending` → `Skip` → `Take`.

---

## 3. Cambios en clases existentes

| Archivo | Cambio |
|---------|--------|
| `src/Domain/Model/Paciente.cs` | Implementa `IEntity<long>`; agrega `[NotMapped] public long Id => PacienteId;`. |
| `src/Domain/Model/ContactoEmergencia.cs` | Implementa `IEntity<long>`; agrega `[NotMapped] public long Id => ContactoEmergenciaId;`. |
| `src/Core/Interfaces/Repository/IPatients.cs` | `IPatientsRepository` ahora hereda de `IGenericRepository<Paciente,long>`; sólo conserva `ExistsByDocumentAsync`, `UpdateAsync`, `DeleteAsync`. |
| `src/Core/Interfaces/Repository/IContactosEmergenciaRepository.cs` | `IContactosEmergenciaRepository` hereda de `IGenericRepository<ContactoEmergencia,long>`; sólo conserva `GetByPacienteIdAsync`, `UpdateAsync`, `DeleteAsync`. |
| `src/Persistence/Repositories/PatientsRepository.cs` | Ahora deriva de `GenericRepository<Paciente,long>`. Sólo redefine `GetAllAsync` (orden por apellidos/nombres), implementa `ExistsByDocumentAsync` (vía `ExistsAsync` genérico) y envuelve `Add/Update/Delete` con `SaveChangesAsync` inmediato. Pasó de ~38 a métodos mínimos. |
| `src/Persistence/Repositories/ContactosEmergenciaRepository.cs` | Igual criterio: deriva del genérico, sólo redefine `GetAllAsync`, agrega `GetByPacienteIdAsync` y el guardado inmediato. |
| `src/Persistence/Extension.cs` | Registra el genérico abierto `AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>))` y deduplica las dos sobrecargas de `AddPersistence`. |

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

## 4. Uso

### 4.1 CRUD genérico directo (inyectar el repositorio genérico)

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

### 4.2 Varias entidades en un mismo guardado

Los repositorios genéricos comparten el `AppDbContext` de la petición (Scoped), así que un
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

### 4.3 Consulta reutilizable con Specification

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

## 5. Verificación

```bash
cd MedicalAppointments.Patient
dotnet build          # Compilación correcta. 0 Advertencia(s) 0 Errores
dotnet ef migrations has-pending-model-changes --project src/Persistence --startup-project src/Api
# -> No changes have been made to the model since the last migration.
```

Los *endpoints* existentes (`/patients`, contactos de emergencia) siguen funcionando sin
cambios porque los repositorios concretos mantienen su contrato público.

---

## 6. Cómo agregar persistencia para una entidad nueva

1. Crear la entidad en `src/Domain/Model` e implementar `IEntity<TKey>`
   (`public TKey Id => MiEntidadId;` con `[NotMapped]`).
2. Mapearla en `AppDbContext` (`DbSet` + `IEntityTypeConfiguration`).
3. Inyectar `IGenericRepository<MiEntidad, TKey>` donde se necesite. **No hay más código.**
4. (Opcional) Crear una interfaz `IMiEntidadRepository : IGenericRepository<MiEntidad, TKey>`
   con consultas específicas y una clase que derive de `GenericRepository<MiEntidad, TKey>`;
   registrarla en `Extension.cs`.
