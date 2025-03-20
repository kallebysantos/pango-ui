#!/usr/bin/env bash

PANGO_VERSION=$1

dotnet tool install --local --create-manifest-if-needed \
  PangoUI.Tool --version "$PANGO_VERSION"
