@echo off
setlocal

pushd "%~dp0"

set dir=.paket
set bootstrapper=%dir%\paket.bootstrapper.exe
set paket=%dir%\paket.exe

if not exist "%paket%" (
  "%bootstrapper%"
  if errorlevel 1 (
    exit /b %errorlevel%
  )
)

"%paket%" %*
