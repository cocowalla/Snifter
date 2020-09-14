dotnet restore .\Snifter.sln

dotnet build .\src\App\App.csproj --configuration Release
dotnet build .\src\Snifter\Snifter.csproj --configuration Release

dotnet pack .\src\Snifter -c Release
