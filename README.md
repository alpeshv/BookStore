# BookStore

## Run application
Run `docker-compose up --build` from src folder

Swagger: localhost:3000/swagger
Port 3000 is bound to api container port 80 in docker-compose.yml. The current docker-compose.yml is creating api contaner under *development* mode.

## View log file
```
docker exec -t -i <container_name> /bin/bash
cd logs
cat <filename>.txt

## Health Check End-point
localhost:3000/health
```

## Missing functionality
- JWT Authorisation
- Fake SMTP Service
- Pagination
- Caching
