#!/bin/sh

[ -z "$1" ] && echo "Syntax: $0 <runtime_identifier> <archive_format>" && exit 1
[ -z "$2" ] && echo "Syntax: $0 <runtime_identifier> <archive_format>" && exit 1

set -euo pipefail

BUILD_DIR=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
RUNTIME_IDENTIFIER=${1-}
ARCHIVE_FORMAT=${2-}

if [ -n "$RUNTIME_IDENTIFIER" ]; then
	shift
fi

if [ -n "$ARCHIVE_FORMAT" ]; then
	shift
fi

exec dotnet msbuild "$BUILD_DIR/Build.proj" -t:Release ${RUNTIME_IDENTIFIER:+"-p:RuntimeIdentifier=$RUNTIME_IDENTIFIER"} ${ARCHIVE_FORMAT:+"-p:ArchiveFormat=$ARCHIVE_FORMAT"} "$@"
