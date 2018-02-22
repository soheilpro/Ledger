#!/bin/sh

DIR=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)

function _publish {
  local PLATFORM=$1

  rm -rf "$DIR/../Publish/$PLATFORM"
  mkdir -p "$DIR/../Publish/$PLATFORM"

  dotnet publish "$DIR/../Sources/Ledger/Ledger.csproj" --configuration Release --runtime $PLATFORM --output "$DIR/../Publish/$PLATFORM"
}

_publish linux-x64
_publish osx-x64
_publish win-x64
