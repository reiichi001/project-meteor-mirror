#!/bin/bash
IMPORT_PATH="C://coding//repositories//ffxiv related//ffxivclassic//ffxiv-classic-server//sql//"
USER=root
PASS=root
DBNAME=ffxiv_server

ECHO Creating Database $DBNAME
mysqladmin -h localhost -u $USER -p$PASS DROP $DBNAME

ECHO Creating Database $DBNAME
mysqladmin -h localhost -u $USER -p$PASS CREATE $DBNAME IF NOT EXISTS $DBNAME

ECHO Loading $DBNAME tables into the database
sh cd $IMPORT_PATH

for X in '*.sql';
do
	for Y in $X
	do
		echo Importing $Y;
		"C:\program files\mysql\mysql server 5.7\bin\mysql" $DBNAME -h localhost -u $USER -p$PASS < $Y
	done
done

ECHO Finished!