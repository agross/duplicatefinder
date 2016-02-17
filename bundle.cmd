@echo off
setlocal

pushd "%~dp0"

chcp 65001 > NUL

if defined TEAMCITY_PROJECT_NAME echo ##teamcity[blockOpened name='%0 %*']

set LANG=en_US.UTF-8

call gem.bat which bundler > NUL 2>&1
if errorlevel 1 (
  echo Installing bundler...
  call gem.bat install bundler --no-ri --no-rdoc
)

call bundle.bat %*

:exit
if defined TEAMCITY_PROJECT_NAME echo ##teamcity[blockClosed name='%0 %*']
exit /b %errorlevel%
