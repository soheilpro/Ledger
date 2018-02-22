#!/bin/sh

DIR=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)

dotnet test "$DIR/../Sources/Ledger.Tests/Ledger.Tests.csproj"
