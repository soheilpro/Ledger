#!/bin/sh

set -euo pipefail

BUILD_DIR=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)

exec dotnet msbuild "$BUILD_DIR/Build.proj" -t:PublishAll "$@"
