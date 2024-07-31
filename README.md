# Proyecto de concurrencia de productores y consumidores

## Pasos para ejecutar el proyecto: 

- Instalar docker
- Instalar rabbitmq con: docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
- Buildear app con: docker build -t producer-consumer-app .
- Crear red docker: docker network create producerConsumerNetwork
- Ejecutar contenedor rabbitMQ en la red creada: docker run -d --name rabbitmq --network producerConsumerNetwork -p 5672:5672 -p 15672:15672 rabbitmq:3-management
- Ejecutar contenedor de la aplicación en la red creada: docker run --network producerConsumerNetwork --env-file .env producer-consumer-app
