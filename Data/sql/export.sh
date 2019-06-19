#!/bin/bash
EXPORT_PATH=C:/repositories/ffxiv-classic-server/sql/
USER=root
PASS=root
DBNAME=ffxiv_server
for T in `mysqlshow -u $USER -p$PASS $DBNAME %`;
do
    echo "Backing up $T"
    mysqldump -u $USER -p$PASS $DBNAME $T --extended-insert=FALSE --no-tablespaces --no-autocommit > $EXPORT_PATH/$T.sql
done;