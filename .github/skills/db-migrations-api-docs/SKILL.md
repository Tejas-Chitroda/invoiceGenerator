---
name: db-migrations-api-docs
description: "Plan and execute EF Core database migrations and generate API documentation (OpenAPI/Swagger) for ASP.NET Core services. Use when adding schema changes, releasing API updates, writing migration notes, or validating backend contract changes."
argument-hint: "goal=<add-migration|update-db|generate-openapi|full-workflow> migrationName=<Name>"
---

# DB Migrations and API Docs

## When To Use
- You changed entity models, DbContext mappings, or configuration and need a migration.
- You are preparing a backend release and need updated OpenAPI output.
- You need a repeatable checklist for schema + contract consistency.

## Inputs
- `goal`: one of `add-migration`, `update-db`, `generate-openapi`, `full-workflow`
- `migrationName`: required for `add-migration` and `full-workflow`
- `startupProject`: optional, defaults to `Backend/Invoice_Generator/InvoiceGenerator.csproj`
- `project`: optional, defaults to `Backend/Invoice_Generator/InvoiceGenerator.csproj`

## Workflow
1. Validate scope and identify if changes are:
   - Schema only
   - API contract only
   - Both schema and API contract
2. If schema changed, create migration with [new-migration.sh](./scripts/new-migration.sh).
3. Apply migration locally for verification using `dotnet ef database update`.
4. If endpoints/DTOs changed, generate OpenAPI with [generate-openapi.sh](./scripts/generate-openapi.sh).
5. Produce a release note summary using [migration-doc-instructions.md](./references/migration-doc-instructions.md).

## Decision Rules
- If model/config changed and there is no new migration, create one.
- If migration is created but app build fails, stop and fix compile issues before update.
- If migration applies but startup fails, stop and report DB/config mismatch.
- If OpenAPI generation fails, keep migration result and report unresolved API doc blockers.

## Completion Criteria
- Migration file created and checked into `Backend/Invoice_Generator/Migrations`.
- Local database update command succeeds.
- OpenAPI JSON generated at `Backend/Invoice_Generator/docs/openapi.json` when API changes exist.
- Change note includes:
  - schema changes
  - endpoints/DTO changes
  - backward compatibility notes
  - rollback considerations

## Commands
- Add migration:
  - `bash ./.github/skills/db-migrations-api-docs/scripts/new-migration.sh AddTaxField`
- Generate OpenAPI:
  - `bash ./.github/skills/db-migrations-api-docs/scripts/generate-openapi.sh`

## References
- Workflow and release-note template: [migration-doc-instructions.md](./references/migration-doc-instructions.md)
