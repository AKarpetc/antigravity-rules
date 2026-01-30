---
trigger: always_on
---


# Test Conventions

These conventions apply to all unit and integration tests across the solution. The goal is to write clear, maintainable tests that validate behavior through outcomes rather than implementation details.

## Frameworks & Tools

- **Test Framework**: `xUnit` (Preferred).
  - *Note*: `nUnit` is allowed **only** in legacy code (e.g., `server-api`). Use `xUnit` for all new tests.
- **Mocking**: `NSubstitute`.
- **Assertions**: `FluentAssertions` (Version 7+).

## Naming & Organization

### Naming Patterns
- **Test Project**: `[ProjectUnderTest].Tests.(Unit|Integration)`
- **Test File**: `{SystemUnderTest}Tests.cs` (e.g., `CartSynchronizerTests.cs`)
- **Test Method**: `{TestAction}_When_{Condition(s)}_Should_{ExpectedBehavior}`
  - Example: `Add_WhenItemExists_ShouldUpdateQuantity`

### Variable Naming
- **System Under Test**: `_sut` (field) or `sut` (local).
- **Result**: `subject`, `result`, `actual`.
- **Inputs**: Use descriptive names like `query`, `parameters`, `expected`.

### Visibility
- Use `[assembly: InternalsVisibleTo(...)]` for assemblies that need to be tested, rather than making internal members public.

## Anatomy of a Test (AAA)

Divide tests into 3 distinct blocks separated by comments:

1.  **Arrange**: Setup mocks, create entities, initialize dependencies.
2.  **Act**: Call the method under test.
    - **Rule**: The `Act` section should ideally be a **single line**.
3.  **Assert**: Verify the result.

```csharp
[Fact]
public void Add_ShouldAlwaysReturnSum()
{
    // Arrange
    var calculator = new Calculator();
    int a = 2;
    int b = 3;

    // Act
    var result = calculator.Add(a, b);

    // Assert
    result.Should().Be(5);
}
```

## Mocking & Stubbing Guidelines

### Core Principle: Subject-Focused
- **Assert the subject** (result of the SUT), not the internals of collaborators.
- **Do not** verify the exact internal sequence of calls unless the behavior depends on it.

### Argument Matching (`Arg.*`) policy
1.  **`Arg.Is<T>(predicate)`**: **Preferred**. Use this to constrain inputs during setup.
    - Example: `_client.Get(Arg.Is<Params>(p => p.Id == 1)).Returns(expected);`
2.  **`Arg.Any<T>()`**: Use when the specific argument value is irrelevant or for `DidNotReceive` checks.
3.  **`Arg.Do`**: **Avoid**. Do not use `Arg.Do` to capture parameters for later assertion.
    - *Why?* It leaks implementation details into the test and makes the test harder to read. constraint the input in the setup instead.

### Examples

**Good (Constrain inputs):**
```csharp
var expected = new CartDto { Total = 3 };
_client.GetCart(Arg.Is<CartParams>(p => p.StoreId == storeId), token)
       .Returns(expected);

var subject = await _sut.Handle(query, token);

subject.Should().BeEquivalentTo(expected);
```

**Bad (Parameter Capture):**
```csharp
CartParams? captured = null;
_client.GetCart(Arg.Do<CartParams>(x => captured = x), token) // ⛔ Avoid this
       .Returns(expected);

await _sut.Handle(query, token);

captured!.StoreId.Should().Be(storeId); // ⛔ Asserting implementation detail
```

## Assertions

### Object Comparison
- **Do not** assert specific fields individually.
- **Use `BeEquivalentTo`** for object comparisons. This ensures that if new fields are added, tests will automatically validate them (or fail if they don't match), keeping tests robust.

**Correct:**
```csharp
cart.Should().BeEquivalentTo(new CartAggregate(storeId, ...), 
    opts => opts
        .Excluding(z => z.Created)
        .Excluding(z => z.Updated));
```

**Wrong:**
```csharp
cart.Id.Should().Be(123);
cart.StoreId.Should().Be(456);
// Misses any other properties!
```

### No Discard Assignments
- **Do**: Assign the result of an async call to a local variable (e.g., `subject`) and assert it.
- **Don't**: Use discard assignments (e.g., `_ = await ...`).

```csharp
// correct
var subject = await _sut.Handle(query, token);
subject.Should().NotBeNull();
```

## Coding Style for Tests

- **Implicit Typing**: Prefer `var` when the type is obvious from the right-hand side.
  - `var subject = await ...`
  - `var expected = new Dto { ... }`
- **Value Objects**: Try to use Value Objects instead of primitives/references where possible to make mocking and assertions easier.
