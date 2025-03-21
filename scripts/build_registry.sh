#!/usr/bin/env bash

set -e

SCRIPT=$(readlink -f "$0")
SCRIPTPATH=$(dirname "$SCRIPT")

export $(grep -v '^#' "$SCRIPTPATH/../.env.public" | xargs)

COMPONENTS_PATH=${1:-"./src/Pango.UI/Components"}
OUTPUT_PATH=${2:-"./publish/registry"}

dotnet pango registry create-metadata \
  $(find $COMPONENTS_PATH -maxdepth 1 -mindepth 1 ! -name "*.cs") \
  -o $OUTPUT_PATH
