#!/usr/bin/env bash
# Log Agent and Skill tool usage to usage.log

LOG_FILE="$(dirname "$0")/usage.log"
INPUT=$(cat)
TIMESTAMP=$(date '+%Y-%m-%dT%H:%M:%S')

# Use PowerShell for JSON parsing (jq not available on Windows)
parse() {
    powershell -NoProfile -Command "
        \$json = '${INPUT}' | ConvertFrom-Json
        \$json.$1
    " 2>/dev/null
}

TOOL_NAME=$(powershell -NoProfile -Command "\$json = @'
${INPUT}
'@ | ConvertFrom-Json; \$json.tool_name" 2>/dev/null)

if [ "$TOOL_NAME" = "Agent" ]; then
    SUBAGENT=$(powershell -NoProfile -Command "\$json = @'
${INPUT}
'@ | ConvertFrom-Json; if (\$json.tool_input.subagent_type) { \$json.tool_input.subagent_type } else { 'general-purpose' }" 2>/dev/null)
    DESCRIPTION=$(powershell -NoProfile -Command "\$json = @'
${INPUT}
'@ | ConvertFrom-Json; \$json.tool_input.description" 2>/dev/null)
    echo "${TIMESTAMP} | pre | Agent | ${SUBAGENT} | ${DESCRIPTION}" >> "$LOG_FILE"

elif [ "$TOOL_NAME" = "Skill" ]; then
    SKILL=$(powershell -NoProfile -Command "\$json = @'
${INPUT}
'@ | ConvertFrom-Json; \$json.tool_input.skill" 2>/dev/null)
    ARGS=$(powershell -NoProfile -Command "\$json = @'
${INPUT}
'@ | ConvertFrom-Json; \$json.tool_input.args" 2>/dev/null)
    echo "${TIMESTAMP} | pre | Skill | ${SKILL} | ${ARGS}" >> "$LOG_FILE"
fi
