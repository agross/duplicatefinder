@echo off
setlocal

pushd "%~dp0"

chcp 65001 > NUL

for %%a in (.) do set dirname=%%~na
set args=%*
set LANG=en_US.UTF-8

:build
cls
title %dirname%: %~n0 %args% (in %~dp0)

set CI=
if defined TEAMCITY_PROJECT_NAME set CI=--without development

call bundle.cmd check
if errorlevel 1 call bundle.cmd install %CI%
if errorlevel 1 goto wait

call bundle.cmd exec rake %args%

:wait
rem Bail if we're running a TeamCity build or from Visual Studio.
if defined TEAMCITY_PROJECT_NAME goto quit
if defined VS_BUILD_EVENT goto quit

rem Loop the build script.
echo (Q)uit, [optional new build target] followed by (Enter) runs the build again
set /p args=
if /i "%args%"=="Q" goto quit

goto build

:quit
exit /b %errorlevel%
