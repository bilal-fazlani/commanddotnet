dotnet build -f netcoreapp3.1
dotnet publish -f netcoreapp3.1 -r win10-x64 -c Release /p:PublishSingleFile=true
copy bin\Release\netcoreapp3.1\win10-x64\publish\CommandDotNet.Example.exe .