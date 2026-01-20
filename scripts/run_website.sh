#!/usr/bin/env bash

set -e

SCRIPT=$(readlink -f "$0")
SCRIPTPATH=$(dirname "$SCRIPT")

export $(grep -v '^#' "$SCRIPTPATH/../.env.public" | xargs)

if [[ $* == *"--build"* ]]; then
    ./scripts/build_docs.sh
    ./scripts/build_registry.sh
fi

bash -c "docker run --rm -it \
    -v ./publish/site/wwwroot:/app \
    -v ./publish/registry:/app/api \
    -w /app -p 8000:8000 python:3 python -m http.server 8000"
