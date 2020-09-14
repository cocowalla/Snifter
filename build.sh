#!/bin/bash
set -ev

dotnet restore ./Snifter.sln
dotnet build ./src/App/App.csproj --configuration Release --framework netcoreapp3.1
dotnet build ./src/Snifter/Snifter.csproj --configuration Release --framework netcoreapp3.1
