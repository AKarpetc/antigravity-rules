# Feature: Image Service (Fuzzy Matching)

## Overview
The **Image Service** is responsible for resolving image URLs for product options (e.g., specific flavors or categories). Since the names of options in the External Service might not exactly match the keys in the local image registry, this service employs **Fuzzy Matching** to find the best candidate.

## Implementation Details
- **Interface**: `IImageService`
- **Implementation**: `ImageService`
- **Algorithm**: Levenshtein Distance (via `FuzzySharp`)
- **Return Type**: `string` (URL or empty if no match found)

## Configuration
- **Source File**: `docs/images.json` (or similar path in parent directories).
- **Threshold**: Only matches with a score above **80** are accepted (configurable in code).

## Data Flow
1.  **Load**: On startup (Singleton), the service loads and caches the `images.json` map.
2.  **Request**: Application layers call `GetImageUrlAsync(term, questionType)`.
3.  **Search**:
    -   Exact match check first.
    -   If no exact match, it iterates through keys in the corresponding `questionType` section.
    -   Calculates similarity score between the search term and the key.
4.  **Result**: Returns the URL of the highest scoring match if it exceeds the threshold.

## Usage
Used internally by the `GetCartAssistantDataQueryHandler` to enrich the response.
