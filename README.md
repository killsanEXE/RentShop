## Start application on Docker

`docker-compose up --build`

There is possible situation, when there is confilict between db-init and seed process(API started before db-init is done). In such case `Ctrl+C` and restart docker-compose with previous command.

## Stop docker containers 

`docker-compose down`

## Stop and clean db

`docker-compose down -v`