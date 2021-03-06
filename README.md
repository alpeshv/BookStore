# BookStore

## Run application
Run `docker-compose up --build` from src folder

Swagger: localhost:3000/swagger

Port 3000 is bound to api container port 80 in docker-compose.yml. The current docker-compose.yml is creating api container under *development* mode.

## View log file
```
docker exec -t -i <container_name> /bin/bash
cd logs
ls
cat <filename>.txt
```

## Health Check End-point
localhost:3000/health

## Missing functionality
- JWT Authorisation
- Fake SMTP Service
- Pagination
- Caching

## System Design

### Project Structure
![Project Structure](https://github.com/alpeshv/BookStore/blob/master/charts/project-structure.jpg)

### System Architecture
![System Design](https://github.com/alpeshv/BookStore/blob/master/charts/system-design.jpg)
