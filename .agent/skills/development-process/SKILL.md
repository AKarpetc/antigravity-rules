---
name: develop-with-continuous-testing
description: Strict iterative development flow with mandatory unit-testing and regression checks for every atomic task.
---
# Unit Tests & Development Skill

## Core Principles
- **No code without tests:** Every new function, class, or logic change must be accompanied by unit tests.
- **Atomic Iterations:** Break down the task into small sub-tasks. Perform the full "Code-Test-Fix" cycle for EACH sub-task.
- **Zero Tolerance:** Do not move to the next task if any test (new or existing) is failing.

## Development Loop (Repeat for each sub-task)

1. **Analyze & Plan**: 
   - Identify the specific logic to implement.
   - Define the expected behavior and edge cases for testing.
2. **Implementation**:
   - Write the minimum code necessary to fulfill the sub-task.
   - Follow conventions in `.agent/rules/code-convention.md` and common rule `.agent/rules/common.md`
3. **Test Creation**:
   - Create/update unit tests in the `tests/` folder according to `.agent/rules/test-convention.md`.
   - Ensure tests cover both happy paths and error handling.
4. **Execution & Verification**:
   - Run **all** unit tests in the project (not just the new ones).
   - [Action]: Execute the test runner tool.
5. **Self-Healing Loop**:
   - IF tests fail: Analyze the output, identify if the bug is in the code or the test, fix it, and return to Step 4.
   - IF tests pass: Proceed to the next sub-task or finalization.

## Finalization
- Once all sub-tasks are complete, perform a final full test suite run.
- Add md document with description of the task and how to use it