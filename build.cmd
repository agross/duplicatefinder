@echo off
set path=tools\Ruby\bin;%path%

:Build
cls
"tools\Ruby\bin\ruby.exe" "tools\Ruby\bin\rake" %*

rem Bail if we're running a TeamCity build.
if defined TEAMCITY_PROJECT_NAME goto Quit

rem Loop the build script.
set CHOICE=nothing
echo (Q)uit, (Enter) runs the build again
set /P CHOICE= 
if /i "%CHOICE%"=="Q" goto :Quit

GOTO Build

:Quit
exit /b %errorlevel%