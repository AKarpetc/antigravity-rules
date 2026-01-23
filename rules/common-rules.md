---
description: Guidelines on which conventions to apply based on the task type
globs:
  - "**/*.cs"
alwaysApply: true
---

# Convention Application Rules

## Universal Principles
**ALL** C# code (Production & Test) must adhere to the high-level principles:
- Reference: [Common Principles](common-principles.md)

## General Code (Production)
When writing production code, you must also follow:
- Reference: [Code Conventions](code-convention.md)

## Tests
When writing tests (Unit or Integration), you must follow:
1. **Common Principles**
2. **Code Conventions** (Style, naming, general C# rules)
3. **Test Conventions** (Structure, assertions, mocking specifics)
   - Reference: [Test Conventions](test-convention.md)
