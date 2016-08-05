@echo off
rem set window
title Making CARS build
color 1E
cls

rem set environment
if exist "C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe" goto x86
if exist "C:\Program Files\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe" goto x64
goto error

:x86
set path="C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE"
goto normal

:x64
set path="C:\Program Files\Microsoft Visual Studio 11.0\Common7\IDE"
goto normal

:error
echo "NO VS 2012 found..."
exit

:normal
echo START...
rem for RELEASE Build *******************************************************************
rem clean projects for release build
echo.
echo *****************************************************************************
echo STEP 1 - Cleaning Release Build...
echo *****************************************************************************
devenv cars.sln /clean Release

rem clean release folders
echo.
echo *****************************************************************************
echo STEP 2  - Clean release folders...
echo *****************************************************************************
echo.
rd /s /q C:\inetpub\wwwroot\Release\CARSSERVICE
rd /s /q C:\inetpub\wwwroot\Release\CARS
echo Cleaned

rem build in release
echo.
echo *****************************************************************************
echo STEP 3  - Making Build in Release Mode
echo *****************************************************************************
devenv cars.sln /build Release

rem copy release build to build folder
echo.
echo *****************************************************************************
echo STEP 4 - Copy files to build folder
echo *****************************************************************************
echo.
md C:\inetpub\wwwroot\Release\CARSSERVICE
copy /y .\CARSService\log4net.config C:\inetpub\wwwroot\Release\CARSSERVICE
copy /y .\CARSService\clientaccesspolicy.xml C:\inetpub\wwwroot\Release\CARSSERVICE
copy /y .\CARSService\CARSService.svc C:\inetpub\wwwroot\Release\CARSSERVICE
md C:\inetpub\wwwroot\Release\CARSSERVICE\emails
copy /y .\CARSService\emails\*.txt C:\inetpub\wwwroot\Release\CARSSERVICE\emails
md C:\inetpub\wwwroot\Release\CARSSERVICE\bin
copy /y .\CARSService\bin\*.dll C:\inetpub\wwwroot\Release\CARSSERVICE\bin

md C:\inetpub\wwwroot\Release\CARS
copy /y .\CARS.Web\web.config C:\inetpub\wwwroot\Release\CARS
copy /y .\CARS.Web\Silverlight.js C:\inetpub\wwwroot\Release\CARS
copy /y .\Default.html.bk C:\inetpub\wwwroot\Release\CARS\Default.html
copy /y .\Default.aspx.bk C:\inetpub\wwwroot\Release\CARS\Default.aspx
copy /y .\CARS.Web\ComingSoon.html C:\inetpub\wwwroot\Release\CARS
md C:\inetpub\wwwroot\Release\CARS\Image
copy /y .\CARS.Web\Image\*.*  C:\inetpub\wwwroot\Release\CARS\Image
md C:\inetpub\wwwroot\Release\CARS\ICO
copy /y .\CARS.Web\ICO\*.*  C:\inetpub\wwwroot\Release\CARS\ICO
md C:\inetpub\wwwroot\Release\CARS\ClientBin
copy /y .\CARS.Web\ClientBin\*.*  C:\inetpub\wwwroot\Release\CARS\ClientBin
md C:\inetpub\wwwroot\Release\CARS\bin
copy /y .\CARS.Web\bin\*.dll  C:\inetpub\wwwroot\Release\CARS\bin

rem for DEBUG Build *******************************************************************
rem clean projects for debug build
echo.
echo *****************************************************************************
echo SETP 5 - Clean projects again
echo *****************************************************************************
devenv cars.sln /clean Release
devenv cars.sln /clean Debug

rem clean debug folders
echo.
echo *****************************************************************************
echo STEP 6 - Cleaning IIS...
echo *****************************************************************************
echo.
rd /s /q C:\inetpub\wwwroot\CARSSERVICE
rd /s /q C:\inetpub\wwwroot\CARS
echo Cleaned

rem build in debug
echo.
echo *****************************************************************************
echo STEP 7 - Making Build in Debug Mode
echo *****************************************************************************
devenv cars.sln /build Debug

rem copy debug build to build folder
echo.
echo *****************************************************************************
echo STEP 8 - Copy files to IIS folder (Debug build)
echo *****************************************************************************
echo.
md C:\inetpub\wwwroot\CARSSERVICE
copy /y .\CARSService\web.config C:\inetpub\wwwroot\CARSService
copy /y .\CARSService\log4net.config C:\inetpub\wwwroot\CARSSERVICE
copy /y .\CARSService\clientaccesspolicy.xml C:\inetpub\wwwroot\CARSSERVICE
copy /y .\CARSService\CARSService.svc C:\inetpub\wwwroot\CARSSERVICE
md C:\inetpub\wwwroot\CARSSERVICE\emails
copy /y .\CARSService\emails\*.txt C:\inetpub\wwwroot\CARSSERVICE\emails
md C:\inetpub\wwwroot\CARSSERVICE\bin
copy /y .\CARSService\bin\*.dll C:\inetpub\wwwroot\CARSSERVICE\bin

md C:\inetpub\wwwroot\CARS
copy /y .\CARS.Web\web.config C:\inetpub\wwwroot\CARS
copy /y .\CARS.Web\Silverlight.js C:\inetpub\wwwroot\CARS
copy /y .\CARS.Web\Default.html C:\inetpub\wwwroot\CARS
copy /y .\CARS.Web\Default.aspx C:\inetpub\wwwroot\CARS
copy /y .\CARS.Web\ComingSoon.html C:\inetpub\wwwroot\CARS
md C:\inetpub\wwwroot\CARS\Image
copy /y .\CARS.Web\Image\*.*  C:\inetpub\wwwroot\CARS\Image
md C:\inetpub\wwwroot\CARS\ICO
copy /y .\CARS.Web\ICO\*.*  C:\inetpub\wwwroot\CARS\ICO
md C:\inetpub\wwwroot\CARS\ClientBin
copy /y .\CARS.Web\ClientBin\*.*  C:\inetpub\wwwroot\CARS\ClientBin
md C:\inetpub\wwwroot\CARS\bin
copy /y .\CARS.Web\bin\*.dll  C:\inetpub\wwwroot\CARS\bin

echo.
echo *****************************************************************************
echo.
echo ALL DONE
echo.
pause