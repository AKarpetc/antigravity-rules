---
trigger: always_on
---

# Code Conventions

> [!NOTE]
> Tests conventions are listed separately: [Tests conventions](mdc:tests-conventions.md)

## Table of Contents

- [General Conventions 💅](#general-conventions-)
- [Git 🛣️](#git-️)
- [API & GRPC](#api--grpc)
- [Result Pattern](#result-pattern)
- [Data Structures & Collections](#data-structures--collections)
- [Configuration](#configuration)
- [Logging](#logging)
- [Code Style](#code-style)

---

## General Conventions 💅

### Access Modifiers
- Use **most strict** access modifiers as possible: `private`/`internal` instead of `public`.
- Use the `internal` modifier for all services within a single project if possible.
- Use `public` for interfaces and `internal` for their implementations if applicable.

### Constructors
- **No Primary Constructors** for classes resolved from DI.
  - **Why?**
    - Loss of naming conventions (`_service` vs `service`).
    - Parameters become mutable.
    - Scope visibility issues.
  - **Bad**: `internal class Service(IFooService foo) { ... }`
  - **Good**: `internal class Service { private readonly IFooService _foo; Service(IFooService foo) { _foo = foo; } }`
- **Null Checks**: Do **not** check services for null using `ArgumentNullException` in the constructor.
  - **Bad**: `_unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));`
  - **Good**: `_unitOfWork = unitOfWork;`

### Double Negation
- Avoid **double negation**. Use positive naming (e.g., `Enabled` instead of `!Disabled`).

### Async & CancellationToken
**🔴 CancellationToken is mandatory in every async method.**

- It should be the last parameter in the parameter list.
- **Internal methods**: Do **not** use a default value. Force the caller to pass the token.
  - **Bad**: `public Task ExecuteAsync(CancellationToken cancellationToken = default)`
  - **Good**: `public Task ExecuteAsync(CancellationToken cancellationToken)`
- **Public API / Legacy Code**: `CancellationToken = default` is acceptable to avoid breaking changes or if the call stack is extremely difficult to refactor immediately (but try to avoid if possible).

---

## API & GRPC

### General API
- **Internal contracts**: Use GRPC.
- **External API**: Use HTTP REST-like approach.
- **Errors**: Use [Problem Details (RFC 7807)](https://tools.ietf.org/html/rfc7807) (`Microsoft.AspNetCore.Mvc.ProblemDetails`).
- **DTOs**: Use `record` for all DTOs in the projects.

### GRPC Guidelines

#### Definitions & Attributes
- Use `[ProtoContract]` and `[ProtoMember]`.
- **Compatibility**: Mark assembly or DTOs with `[CompatibilityLevel(CompatibilityLevel.Level300)]`.
  - This allows `Guid` to be serialized as a string (better cross-platform compatibility).
  - *Note*: Changing compatibility level on existing types can break backward compatibility.

#### Request/Response Pattern
- Always use wrappers for requests and responses. Avoid `void` or empty `Task` returns.
  - **Bad**: `Task<int> GetAsync(...)`
  - **Good**: `Task<GetResponse> GetAsync(GetRequest request, ...)`
- **Naming**: Use `VerbObjectRequest` / `VerbObjectResponse`.
  - Example: `GetPromoWizardStateRequest`, `GetPromoWizardStateResponse`.

#### Collections in Contracts
- **Initialize Collections**: Always initialize collections with a default value.
  - Example: `public int[] Ids { get; init; } = [];`
- **Prohibited Types**:
  - `Dictionary<K, V>`: Not supported in Proto standard. Use a list of key-value pairs or a custom map class (e.g., `Dictionary<int, BarList>`).
  - `IReadOnlyCollection<T>`: Issues with serialization (Wait for protobuf-net v3.2+). Use `T[]` or `IEnumerable<T>`.

---

## Result Pattern

Try to use the **Result pattern** where possible instead of throwing exceptions for logic flow.
Use CSharpFunctionalExtensions for ready implementation

### Layer-by-Layer Guidelines

| Layer           | Validation/Check     | Action Execution                | Returns                    | Throws                                           |
| :-------------- | :------------------- | :------------------------------ | :------------------------- | :----------------------------------------------- |
| **Domain**      | `CanExecute()`       | `Execute()`                     | `Result`                   | **Never** <br> (or only on invariant violations) |
| **Application** | Implicit or Explicit | Use domain logic or app service | `Result<T>`                | **Never** <br> (wrap all errors)                 |
| **API**         | Model validation     | Call application layer          | `IResult` / `ActionResult` | Maybe <br> (caught by middleware)                |

### Implementation Details

#### Domain Layer
- Methods (e.g., `ChangeName`) should return `Result` or `Result<T>`.
- Use `CanExecute` pattern for validation which returns `Result` or valid state.
- **Do not** throw exceptions for failing business rules.

#### Application Layer
- **Always check the result**: `if (result.IsFailure) { return ... }`.
- Return `Result<T>` to the API layer.
- Wrap all logic in `try-catch` blocks (if needed) or let Result handlings flow naturally.

#### API Layer
- Map `Result.Error` to HTTP status codes.
- Example:
  ```csharp
  if (result.IsFailure)
  {
      return result.Error switch
      {
          NotFoundAppError => NotFound(result.Error.Message),
          _ => BadRequest(result.Error.Message)
      };
  }
  return Ok(result.Value);
  ```

---

## Data Structures & Collections

### Usage
- Use **read-only** collection interfaces (`IReadOnlyCollection<T>`, `IReadOnlySet<T>`) for parameters and return types where possible.
- Use `IEnumerable<T>` for lazy evaluation.
- Use `T[]` (Arrays) for GRPC DTOs.

### Nullability
- **Collections**: Do not use nullable collections (e.g., `List<int>?`). Use an empty collection instead.
- **Booleans**: Avoid `bool?`. Use an `enum` (e.g., `State.Success`, `State.Failed`, `State.Ignored`) or a wrapper if "not set" is a valid state distinct from false.

---

## Configuration

### IOptions
- **❌ Do Not Use `IOptions`**: It does not support dynamic updates.
- **✅ Scoped/Transient**: Use `IOptionsSnapshot`.
- **✅ Singleton**: Use `IOptionsMonitor`.

### Feature Flags
- Must be **disabled by default** (`false`).
- Use positive naming: `NewFeatureEnabled` (not `OldFeatureDisabled`).

### Secrets & Etcd
- **Etcd**: Do **not** store secrets or sensitive data in etcd. It is not a secret store.
- **Vault**: Store all secrets in Vault.

---

## Logging

- **Template Parameters**: Use `UpperCamelCase`.
  - **Bad**: `_logger.LogError("Error in {storeId}", storeId);`
  - **Good**: `_logger.LogError("Error in {StoreId}", storeId);`
- **Null Checks**: Never check logger for null. `_logger = logger;`.

---

## Code Style

### Formatting
- **Line Breaks**: Use line breaks for long lines.
- **Chain Methods**: Place **every** chained method on a new line starting with a dot.
  ```csharp
  var result = items
      .Where(x => x.Active)
      .Select(x => x.Name)
      .ToArray();
  ```
- **Attributes**: Place each attribute on a separate line.
  ```csharp
  [Required]
  [JsonProperty("foo")]
  public string Foo { get; set; }
  ```

### Control Flow
- **Braces**: Use braces `{}` for all `if` statements, even one-liners.
- **Else**: Avoid `else` keyword where possible. Use early returns (Guard Clauses) implementation pattern.
  ```csharp
  // Bad
  if (valid) { ... } else { return error; }
  
  // Good
  if (!valid) { return error; }
  ...
  ```

### Naming
- **Context Prefixes**: Do **not** use context prefixes in property names within a class.
  - **Bad**: `class Product { public int ProductId { get; set; } }`
  - **Good**: `class Product { public int Id { get; set; } }`
- **Async Suffix**: Maintain consistency with the existing style in the file/project.

### Classes & Methods
- **Partial Classes**: Avoid `partial` classes (except for code generation).
- **Expression Bodies**: Encouraged for short methods/properties.

### Invocations
- **Arguments**: Place arguments on separate lines if there is more than one (or if they are long).
- **Named Arguments**: Encouraged for literals or unclear boolean flags.
  - `Foo(bar: 1, isRecursive: true);`

### Syntax Preferences
- **Default Keyword**:
  - Avoid `variable = default` for primitives or classes. Use implicit types or explicit values.
  - Exception: Structs/Generics.
  - **Bad**: `TimeSpan t = new();`
  - **Good**: `TimeSpan t = default;`
- **Ternary Operator**: Encouraged for assignments.
- **Null-Coalescing (`??`)**: Encouraged.
- **Constants**: Use `const` over variables.