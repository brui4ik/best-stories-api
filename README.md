# best-stories-api

**BestStoriesApi** is a .NET Minimal API that retrieves the first **N "best stories"** from the official **Hacker News API**, sorts them by score (descending), and returns them in a simplified response format.

The solution is built using:

- **.NET 10 Minimal API**
- **Carter Modules** (endpoint routing)
- **MediatR** (CQRS-style request handling)
- **FluentValidation** (request validation)
- **AutoMapper** (DTO → response model mapping)
- **Memory Cache** (to reduce load on Hacker News API)
- **Rate Limiting** (per-IP fixed window)
- **Polly Retry Policy** (for transient upstream failures)
- **Global throttling** to protect Hacker News API from overload

---

## What the API Does

The API calls the Hacker News endpoint:

- `https://hacker-news.firebaseio.com/v0/beststories.json`

This returns a list of IDs for the current best stories.

Then the API fetches the details for the first **N** IDs:

- `https://hacker-news.firebaseio.com/v0/item/{id}.json`

Finally, it returns the stories sorted by **score** in descending order.

---

## Endpoint

### `GET /api/best-stories?n={N}`

#### Query Parameters

| Name | Type | Required | Description |
|------|------|----------|-------------|
| n    | int  | Yes      | Number of best stories to return (n > 0) |

#### Example Request
```http
GET https://localhost:7161/api/best-stories?n=10
```

## Example Response

```json
[
  {
    "id": 21233041,
    "title": "A uBlock Origin update was rejected from the Chrome Web Store",
    "uri": "https://github.com/uBlockOrigin/uBlock-issues/issues/745",
    "postedBy": "ismaildonmez",
    "time": "2019-10-12T13:43:01+00:00",
    "score": 1716,
    "commentCount": 572
  }
]
```

How to Run the Application
--------------------------

### Prerequisites

*   **.NET 10 SDK**
    
*   (Optional) Postman / curl / browser
    

### Run from CLI

From the project folder:

```bash
dotnet restore
dotnet run --launch-profile https
```

The API will then be available at:

*   https://localhost:7161
    
*   http://localhost:5075
    

Testing the API
---------------

Example using curl (HTTPS):

```bash
curl "https://localhost:7161/api/best-stories?n=10"
```

Assumptions
-----------

*   descendants from the Hacker News API is used as commentCount because Hacker News defines it as the total number of comments for a story.
    
*   Story items are cached in memory because Hacker News story metadata does not change frequently.
    
*   In-memory cache is sufficient for a single-instance deployment.
    
*   The API protects the Hacker News API from overload using:
    
    *   global throttling (SemaphoreSlim)
        
    *   caching
        
    *   retry policy
        
    *   per-IP rate limiting
        

Error Handling
--------------

Validation errors return HTTP 400 in JSON format:

```json
{
  "statusCode": 400,
  "message": "Validation failed.",
  "details": {
    "N": [
      "N must be must be greater than 0"
    ]
  },
  "traceId": "0HNJ64U2BVJK8:00000003"
}
```

Future Improvements
-------------------

Given more time, the following enhancements would be implemented:

*   **API Versioning:** Add proper API versioning (e.g. /api/v1/best-stories) using Asp.Versioning to support backward compatibility.
    
*   **Swagger / OpenAPI Documentation:** Add Swagger UI and OpenAPI definitions to improve discoverability and allow easier testing.
    
*   **Distributed Cache (Redis):** Replace IMemoryCache with Redis (IDistributedCache) to support horizontal scaling and shared caching across multiple API instances.
    
*   **Code Quality Tooling (Sonar / Style Rules):** Add SonarQube (or SonarCloud) and enforce consistent coding style using .editorconfig + analyzers.
    
*   **Metrics and Logging:** Add structured logging (Serilog) and metrics (OpenTelemetry / Prometheus) for better observability and performance monitoring.

