#!/bin/bash

mkdir -p .github/hooks/logs

echo "[$(date)] Tool execution triggered" >> .github/hooks/logs/tools.log
echo "Tool Input: $COPILOT_TOOL_INPUT" >> .github/hooks/logs/tools.log
echo "-------------------------------------" >> .github/hooks/logs/tools.log