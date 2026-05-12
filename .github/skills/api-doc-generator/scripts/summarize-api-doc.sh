#!/usr/bin/env bash
set -euo pipefail

OPENAPI_FILE="${OPENAPI_FILE:-Backend/Invoice_Generator/docs/openapi.json}"
SUMMARY_FILE="${SUMMARY_FILE:-Backend/Invoice_Generator/docs/api-change-summary.md}"

if [[ ! -f "$OPENAPI_FILE" ]]; then
  echo "OpenAPI file not found: $OPENAPI_FILE"
  exit 1
fi

mkdir -p "$(dirname "$SUMMARY_FILE")"

ENDPOINT_COUNT="0"
SCHEMA_COUNT="0"

if command -v jq >/dev/null 2>&1; then
  ENDPOINT_COUNT="$(jq '.paths | keys | length' "$OPENAPI_FILE")"
  SCHEMA_COUNT="$(jq '.components.schemas | keys | length' "$OPENAPI_FILE")"
else
  # Fallback counts when jq is unavailable.
  ENDPOINT_COUNT="$(grep -o '"/[^"]*":' "$OPENAPI_FILE" | wc -l | tr -d ' ')"
  SCHEMA_COUNT="$(grep -o '"components"' "$OPENAPI_FILE" | wc -l | tr -d ' ')"
fi

GENERATED_AT="$(date -u +"%Y-%m-%dT%H:%M:%SZ")"

cat > "$SUMMARY_FILE" <<EOF
# API Change Summary

Generated at: $GENERATED_AT
OpenAPI file: $OPENAPI_FILE

## Quick Stats
- Endpoint count: $ENDPOINT_COUNT
- Schema count: $SCHEMA_COUNT

## Changes To Review
- Added or removed endpoints:
- Request payload changes:
- Response payload/status code changes:
- Validation rule updates:

## Compatibility
- Breaking changes:
- Non-breaking changes:
- Client action items:
EOF

echo "API documentation summary"
echo "- OpenAPI file: $OPENAPI_FILE"
echo "- Endpoint count: $ENDPOINT_COUNT"
echo "- Schema count: $SCHEMA_COUNT"
echo "- Markdown summary: $SUMMARY_FILE"
