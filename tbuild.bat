dotnet test

dotnet pack ^
 -o output ^
 -c Release^
 CommandDotNet/CommandDotNet.csproj^
 /p:Version=1.0.0-SNAPSHOT