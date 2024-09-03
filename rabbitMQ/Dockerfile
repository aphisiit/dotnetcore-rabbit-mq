FROM mcr.microsoft.com/dotnet/sdk:8.0.401 AS builder
USER root
WORKDIR /opt/rabbitMQ
COPY . ./

RUN ["dotnet", "publish", "rabbitMQ.csproj",  "-c", "Release"]
RUN ["dotnet", "tool", "install", "--global", "dotnet-ef", "--version", "8.0.8"]

ENV PATH $PATH:/root/.dotnet/tools

RUN ["dotnet", "ef", "migrations", "add", "InitialCreate_docker"]
RUN ["dotnet", "ef", "database", "update"]

FROM mcr.microsoft.com/dotnet/aspnet:8.0.8

# Add dotnet core env for enable swagger 
ENV ASPNETCORE_ENVIRONMENT Development

USER root

# RUN apt-get update && apt-get upgrade
# RUN apt install -y curl
# RUN apt install -y lsof

WORKDIR /opt/rabbitMQ

EXPOSE 8080

COPY --from=builder /opt/rabbitMQ/bin/Release/net8.0/publish .
COPY --from=builder /opt/rabbitMQ/sqliteDB sqliteDB

# RUN ["dotnet", "tool", "install", "--global", "dotnet-ef"]
# RUN ["dotnet", "ef", "migrations", "add", "InitialCreate"]
# RUN ["dotnet", "ef", "database", "update"]

ENTRYPOINT ["dotnet", "rabbitMQ.dll"]