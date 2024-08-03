FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

ENV PRODUCER_COUNT=2
ENV CONSUMER_COUNT=2
ENV MESSAGE_COUNT=3
ENV RABBITMQ_HOST=rabbitmq

ENTRYPOINT ["dotnet", "ProducerConsumerApp.dll"]
