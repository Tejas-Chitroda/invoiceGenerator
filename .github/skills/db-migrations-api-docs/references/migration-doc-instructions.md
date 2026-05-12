# Migration and API Documentation Instructions

Use this file when the skill needs to write consistent release notes after migration and API updates.

## Step-by-Step
1. Confirm migration name and timestamp.
2. Summarize schema operations in plain language:
   - tables added/removed
   - columns added/removed/renamed
   - index or constraint changes
3. Summarize API contract impact:
   - new endpoints
   - request/response DTO changes
   - validation rule changes
4. Record compatibility and deployment notes:
   - breaking vs non-breaking
   - data backfill needs
   - rollback strategy
5. Add verification evidence:
   - migration command used
   - database update status
   - OpenAPI generation output path

## Output Template
## Release Change Summary

### Migration
- Name:
- Files:
- Database update status:

### Schema Changes
- 

### API Changes
- 

### Compatibility
- Breaking changes:
- Client action required:

### Rollback
- Strategy:
- Data risks:

### Verification
- `dotnet ef migrations add <Name>`
- `dotnet ef database update`
- OpenAPI output: `Backend/Invoice_Generator/docs/openapi.json`
