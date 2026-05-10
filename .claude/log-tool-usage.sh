#!/usr/bin/env bash
# Log Agent and Skill tool usage to usage.log

LOG_FILE="$(dirname "$0")/usage.log"
INPUT=$(cat)
TIMESTAMP=$(date '+%Y-%m-%dT%H:%M:%S')

TOOL_NAME=$(echo "$INPUT" | jq -r '.tool_name // empty')

if [ "$TOOL_NAME" = "Agent" ]; then
    SUBAGENT=$(echo "$INPUT" | jq -r '.tool_input.subagent_type // "general-purpose"')
    DESCRIPTION=$(echo "$INPUT" | jq -r '.tool_input.description // empty')
    echo "${TIMESTAMP} | pre | Agent | ${SUBAGENT} | ${DESCRIPTION}" >> "$LOG_FILE"

elif [ "$TOOL_NAME" = "Skill" ]; then
    SKILL=$(echo "$INPUT" | jq -r '.tool_input.skill // empty')
    ARGS=$(echo "$INPUT" | jq -r '.tool_input.args // empty')
    echo "${TIMESTAMP} | pre | Skill | ${SKILL} | ${ARGS}" >> "$LOG_FILE"
fi
