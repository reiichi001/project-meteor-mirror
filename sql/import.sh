#!/bin/bash
IMPORT_PATH="path/to/ffxiv-classic-server/sql/"
USER=root
PASS=root
DBNAME=ffxiv_server

echo Creating Database $DBNAME
mysql -h localhost -u $USER -p$PASS DROP $DBNAME

echo Creating Database $DBNAME
mysql -h localhost -u $USER -p$PASS CREATE $DBNAME IF NOT EXISTS $DBNAME

echo Loading $DBNAME tables into the database

for X in $IMPORT_PATH'*.sql';
do
	for Y in $X
	do
		echo Importing $Y;
		mysql $DBNAME -h localhost -u $USER -p$PASS < $Y
	done
done

echo Finished!

