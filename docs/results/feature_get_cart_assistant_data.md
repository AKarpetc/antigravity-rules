# Feature: Get Cart Assistant Data

## Overview
The **Get Cart Assistant Data** feature is the core functionality of the service. It aggregates data from multiple sources to present a structured set of questions and answers to the client, enabling a personalized product selection flow.

## Implementation Details
- **Pattern**: CQRS (Query)
- **Query**: `GetCartAssistantDataQuery`
- **Handler**: `GetCartAssistantDataQueryHandler`
- **Return Type**: `Result<CartAssistantModel>`

## Data Sources
1.  **Settings Database (PostgreSQL)**:
    -   Source of truth for the structure of the questionnaire (Questions, Steps, UI logic).
    -   Defines limits (MaxCount) and types.
2.  **External Service (Mocked)**:
    -   Provides the available `Options` for each question type (e.g., list of specific flavors or categories).
    -   Filtered based on the limits defined in Settings.
3.  **Image Service (File System)**:
    -   Provides image URLs for each option.
    -   Uses **Fuzzy Matching** (Levenshtein distance) to match option names from the External Service to image entries in `images.json`.

## Data Flow
1.  **Receive Query**: The `GetCartAssistantDataQuery` is received with a `StoreId`.
2.  **Fetch Settings**: The handler retrieves the current `SettingsModel` from the database.
3.  **Prepare External Request**: Limits are extracted from the settings (e.g., "Category" has MaxCount 5).
4.  **Fetch External Data**: The handler calls `ICartAssistantExternalService` with the `StoreId` and limits.
5.  **Merge & Map**:
    -   The handler iterates through the questions defined in **Settings**.
    -   For "Range" or "Strength" types, answers are generated algorithmically based on `Min`/`Max` settings.
    -   For "Selectable" types (Category, Effect, etc.), options are matched from the **External Data**.
    -   **Image Resolution**: Each option title is passed to `IImageService` to find the best matching image URL.
6.  **Return**: A fully populated `CartAssistantModel` is returned.

## Usage (gRPC)
The feature is exposed via the `GetCartAssistantData` RPC method.

```protobuf
service CartAssistantService {
  rpc GetCartAssistantData (GetCartAssistantDataRequest) returns (GetCartAssistantDataResponse);
}
```
