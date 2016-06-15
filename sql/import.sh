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
for X in *.sql;
do 
	echo Importing $X;
	"C:\program files\mysql\mysql server 5.7\bin\mysql" $DBNAME -h localhost -u $USER -p$PASS < $X
done

ECHO Finished!