@ECHO OFF
SETLOCAL
REM =============
REM IMPORT CONFIG
REM =============
REM NOTE: No spaces before or after the '='!!!

REM =============
SET PATH_MYSQL="C:\wamp\bin\mysql\mysql5.6.17\bin\mysql.exe"
SET PATH_MYSQLADMIN="C:\wamp\bin\mysql\mysql5.6.17\bin\mysqladmin.exe"
SET PATH_SQL="D:\Coding\FFXIV Related\ffxiv-classic-map-server\sql"

SET USER=root
SET PASSWORD=
SET DBADDRESS=localhost
SET DBPORT=3306
SET DBNAME=ffxiv_server
REM =============

IF DEFINED PASSWORD (SET PASSWORD=-p%PASSWORD%)

ECHO Deleteing old database
%PATH_MYSQLADMIN% -h %DBADDRESS% -u %USER% %PASSWORD% DROP %DBNAME%

ECHO Creating new database
%PATH_MYSQLADMIN% -h %DBADDRESS% -u %USER% %PASSWORD% CREATE %DBNAME%

ECHO Loading tables into the database
cd %PATH_SQL%
FOR %%X IN (*.sql) DO ECHO Importing %%X & %PATH_MYSQL% %DBNAME% -h %DBADDRESS% -u %USER% < %%X
ECHO Finished!

ENDLOCAL
@ECHO ON