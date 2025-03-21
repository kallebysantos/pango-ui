#!/usr/bin/env bash

set -e

SCRIPT=$(readlink -f "$0")
SCRIPTPATH=$(dirname "$SCRIPT")

export $(grep -v '^#' "$SCRIPTPATH/../.env.public" | xargs)

PROJECT_PATH=${1:-"./src/Pango.UI"}
OUTPUT_PATH=${2:-"./publish/site"}

dotnet publish $PROJECT_PATH -o $OUTPUT_PATH
