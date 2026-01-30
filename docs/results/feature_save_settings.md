# Feature: Save Settings

## Overview
The **Save Settings** feature allows administrators or external systems to update the configuration of the Cart Assistant. This configuration drives the questionnaire flow, including the questions asked, their order, and UI texts.

## Implementation Details
- **Pattern**: CQRS (Command)
- **Command**: `SaveSettingsCommand`
- **Handler**: `SaveSettingsCommandHandler`
- **Return Type**: `Result` (Success/Failure)

## Data Flow
1.  **Receive Command**: The `SaveSettingsCommand` is received containing a raw JSON string.
2.  **Validate JSON**:
    -   The handler attempts to deserialize the JSON string into the `SettingsModel` Domain object.
    -   If deserialization fails, a failure result (`Settings.InvalidJson`) is returned immediately.
3.  **Persist**:
    -   The validated `SettingsModel` is passed to the `ISettingsRepository`.
    -   The repository serializes the model back to JSON (ensuring clean formatting) and updates the `Settings` table in the PostgreSQL database.
    -   If the settings record does not exist (ID 1), it is created.
4.  **Confirm**: A Success result is returned to the caller.

## Validation Rules
- The JSON input must be strictly compatible with the `SettingsModel` schema.
- Invalid types or malformed JSON will be rejected.

## Usage (gRPC)
The feature is exposed via the `SaveSettings` RPC method.

```protobuf
service CartAssistantService {
  rpc SaveSettings (SaveSettingsRequest) returns (SaveSettingsResponse);
}
```
