#!/bin/sh

DIR=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)

dotnet build "$DIR/../Sources/Ledger/Ledger.csproj"
