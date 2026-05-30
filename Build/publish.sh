#!/bin/sh

[ -z "$1" ] && echo "Syntax: $0 <runtime_identifier>" && exit 1

set -euo pipefail

BUILD_DIR=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
RUNTIME_IDENTIFIER=${1-}

if [ -n "$RUNTIME_IDENTIFIER" ]; then
	shift
fi

exec dotnet msbuild "$BUILD_DIR/Build.proj" -t:Publish ${RUNTIME_IDENTIFIER:+"-p:RuntimeIdentifier=$RUNTIME_IDENTIFIER"} "$@"
