@echo off
setlocal

set /p version="Enter nuget release version: "

dotnet pack .\AnyMapper\AnyMapper\AnyMapper.csproj -property:Configuration=Release --version-suffix %version%
