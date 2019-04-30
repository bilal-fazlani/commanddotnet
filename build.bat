dotnet pack ^
 -o output ^
 -c Release^
 CommandDotNet/CommandDotNet.csproj^
 /p:Version=%APPVEYOR_BUILD_VERSION%