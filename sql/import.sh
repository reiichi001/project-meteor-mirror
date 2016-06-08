#!/bin/bash
IMPORT_PATH=C:/repositories/ffxiv-classic-server/sql/
USER=root
PASS=root
DBNAME=ffxiv_server

ECHO Creating Database $DBNAME
mysqladmin -h localhost -u $USER -p$PASS DROP $DBNAME

ECHO Creating Database $DBNAME
mysqladmin -h localhost -u $USER -p$PASS CREATE $DBNAME

ECHO Loading $DBNAME tables into the database
cd $IMPORT_PATH
FOR %%X IN (*.sql) DO ECHO Importing %%X & "c:\program files\mysql\mysql server 5.7\bin\mysql" $DBNAME -h localhost -u $USER -p$PASS < %%X

ECHO Finished!