@echo off

REM nuget restore -NonInteractive

msbuild Azure.ApiHub.Sdk.proj /t:Build /p:Configuration=Release
