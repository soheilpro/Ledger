#!/bin/sh

DIR=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)
VERSION=$(grep '<Version>.*</Version>' -o $DIR/../Sources/Ledger/Ledger.csproj | sed -e 's,<Version>\([^<]*\)</Version>,\1,')

function _tar {
  local PLATFORM=$1
  local OUTPUT=$DIR/../Releases/Ledger-$VERSION-$PLATFORM.tar.gz

  mkdir -p "$DIR/../Releases"
  rm "$OUTPUT"
  tar -cvzf "$OUTPUT" -C "$DIR/../Publish/$PLATFORM" .
}

function _zip {
  local PLATFORM=$1
  local OUTPUT=$DIR/../Releases/Ledger-$VERSION-$PLATFORM.zip

  mkdir -p "$DIR/../Releases"
  rm "$OUTPUT"
  zip --recurse-paths --junk-paths "$OUTPUT" "$DIR/../Publish/$PLATFORM/"
}

_tar linux-x64
_tar osx-x64
_zip win-x64
