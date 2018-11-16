#!/bin/bash
set -ev

dotnet restore ./Snifter.sln
dotnet build ./src/Snifter.csproj --configuration Release --framework netcoreapp2.1
