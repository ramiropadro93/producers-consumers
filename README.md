# Proyecto de concurrencia de productores y consumidores

## Descripción

Este proyecto implementa un sistema de productores y consumidores utilizando RabbitMQ como sistema de mensajería. El propósito es demostrar cómo múltiples productores pueden enviar mensajes a una cola y cómo múltiples consumidores pueden recibir y procesar esos mensajes de manera concurrente.

## Requisitos

- Docker
- RabbitMQ

## Cómo ejecutar

1. Clona este repositorio.
2. Navega al directorio del proyecto.
3. Construye la imagen Docker:

    ```sh
    docker build -t producer-consumer-app .
    ```

4. Crea una red Docker para que los contenedores puedan comunicarse:

    ```sh
    docker network create producerConsumerNetwork
    ```

5. Inicia un contenedor RabbitMQ en la red creada:

    ```sh
    docker run -d --name rabbitmq --network producerConsumerNetwork -p 5672:5672 -p 15672:15672 rabbitmq:3-management
    ```

6. Inicia el contenedor de la aplicación:

    ```sh
    docker run --env-file .env --network producerConsumerNetwork producer-consumer-app
    ```

## Variables de entorno

El archivo `.env` debe contener las siguientes variables:

```env
PRODUCER_COUNT=5
CONSUMER_COUNT=2
RABBITMQ_HOST=rabbitmq
MESSAGE_COUNT=3
