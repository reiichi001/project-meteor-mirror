@ECHO OFF
REM SETLOCAL
SET CWD = %~dp0

REM Echo Launch dir: "%~dp0"
REM Echo Current dir: "%CD%"

REM =============
REM COPY LOBBY CONFIG
REM =============
REM Required files: lobby_config.ini

SET /a foundlfolders = 0

if exist "%~dp0\..\Lobby Server\bin\Debug" (
SET /a foundlfolders = %foundlfolders% + 1
echo Found Lobby Debug build folder. 

echo Copying lobby_config.ini if needed...
xcopy lobby_config.ini "%~dp0\..\Lobby Server\bin\Debug\" /d /y /q
) 

if exist "%~dp0\..\Lobby Server\bin\Release" (
SET /a foundlfolders = %foundlfolders% + 1
echo Found Lobby Release build folder. 

echo Copying lobby_config.ini if needed...
xcopy lobby_config.ini "%~dp0\..\Lobby Server\bin\Release\" /d /y /q
) 

if %foundlfolders% LSS 1 (
echo Could not find debug or release folder for the Lobby server. Please compile the project first!
)

REM =============
REM COPY WORLD CONFIG
REM =============
REM Required files: world_config.ini

SET /a foundwfolders = 0

if exist "%~dp0\..\World Server\bin\Debug" (
SET /a foundwfolders = %foundwfolders% + 1
echo Found World Debug build folder. 

echo Copying world_config.ini if needed...
xcopy world_config.ini "%~dp0\..\World Server\bin\Debug\" /d /y /q
) 

if exist "%~dp0\..\World Server\bin\Release" (
SET /a foundwfolders = %foundwfolders% + 1
echo Found World Release build folder. 

echo Copying world_config.ini if needed...
xcopy world_config.ini "%~dp0\..\World Server\bin\Release\" /d /y /q
) 

if %foundwfolders% LSS 1 (
echo Could not find debug or release folder for the World server. Please compile the project first!
)

REM =============
REM COPY MAP CONFIG
REM =============
REM Required files: map_config.ini staticactors.bin scripts/

SET /a foundmfolders = 0

if exist "%~dp0\..\Map Server\bin\Debug" (
SET /a foundmfolders = %foundmfolders% + 1
echo Found Map Debug build folder. 

echo Copying map_config.ini if needed...
xcopy map_config.ini "%~dp0\..\Map Server\bin\Debug\" /d /y /q

if exist staticactors.bin (
echo Copying staticactors.bin if needed...
xcopy staticactors.bin "%~dp0\..\Map Server\bin\Debug\" /d /y /q
) else (
echo Cannot copy the staticactors.bin file because it doesn't exist in data\
)


echo Copying scripts folder if needed...
xcopy scripts "%~dp0\..\Map Server\bin\Debug\scripts\" /e /d /y /s /q
) 

if exist "%~dp0\..\Map Server\bin\Release" (
SET /a foundmfolders = %foundmfolders% + 1
echo Found Map Release build folder. 

echo Copying map_config.ini if needed...
xcopy map_config.ini "%~dp0\..\Map Server\bin\Release\" /d /y /q

if exist staticactors.bin (
echo Copying staticactors.bin if needed...
xcopy staticactors.bin "%~dp0\..\Map Server\bin\Release\" /d /y /q
) else (
echo Cannot copy the staticactors.bin file because it doesn't exist in data\
)

echo Copying scripts folder if needed...
xcopy scripts "%~dp0\..\Map Server\bin\Release\scripts\" /e /d /y /s /q
) 

if %foundmfolders% LSS 1 (
echo Could not find debug or release folder for the Map server. Please compile the project first!
)

Pause