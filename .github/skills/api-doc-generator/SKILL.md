---
name: api-doc-generator
description: "Generate and verify API documentation for ASP.NET Core services using Swagger/OpenAPI. Use when endpoints, DTOs, validation, or response contracts change and updated API docs are required for release or frontend integration."
argument-hint: "goal=<generate-openapi|release-summary> output=<path> summaryFile=<path>"
---

# API Doc Generator

## When To Use
- You changed controllers, routes, DTOs, or validation rules.
- You need an updated OpenAPI contract file for frontend or QA.
- You need a concise API change summary for release notes.

## Inputs
- `goal`: one of `generate-openapi`, `release-summary`
- `output`: optional, defaults to `Backend/Invoice_Generator/docs/openapi.json`
- `summaryFile`: optional, defaults to `Backend/Invoice_Generator/docs/api-change-summary.md`
- `swaggerUrl`: optional, defaults to `http://localhost:5000/swagger/v1/swagger.json`

## Workflow
1. Verify API compiles and Swagger endpoint is available in development mode.
2. Generate latest OpenAPI JSON with [generate-openapi.sh](./scripts/generate-openapi.sh).
3. Summarize and review API impact with [summarize-api-doc.sh](./scripts/summarize-api-doc.sh).
4. Refine the final release summary using [api-doc-release-template.md](./references/api-doc-release-template.md).

## Decision Rules
- If generation fails because app cannot start, stop and report startup or config issue.
- If OpenAPI succeeds, produce both console and markdown summaries.
- If only descriptions/examples changed, mark as documentation-only change.

## Completion Criteria
- OpenAPI JSON exists and is non-empty at configured output path.
- Markdown summary exists at configured summary path.
- Console summary is printed with quick endpoint/schema stats.
- Release summary contains endpoint, request/response, and client-impact notes.

## Commands
- Generate latest OpenAPI:
  - `bash ./.github/skills/api-doc-generator/scripts/generate-openapi.sh`
- Create markdown + console summary:
  - `bash ./.github/skills/api-doc-generator/scripts/summarize-api-doc.sh`
- Generate to a custom path:
  - `OUTPUT=Backend/Invoice_Generator/docs/openapi.latest.json bash ./.github/skills/api-doc-generator/scripts/generate-openapi.sh`
- Generate summary to a custom path:
  - `OPENAPI_FILE=Backend/Invoice_Generator/docs/openapi.latest.json SUMMARY_FILE=Backend/Invoice_Generator/docs/api-change-summary.md bash ./.github/skills/api-doc-generator/scripts/summarize-api-doc.sh`

## References
- API release summary template: [api-doc-release-template.md](./references/api-doc-release-template.md)
