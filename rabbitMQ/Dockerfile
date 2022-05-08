FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine as builder
USER root
WORKDIR /opt/rabbitMQ
COPY . ./

RUN ["dotnet", "publish", "-c", "Release"]
RUN ["dotnet", "tool", "install", "--global", "dotnet-ef"]
RUN ["dotnet", "ef", "migrations", "add", "InitialCreate"]
RUN ["dotnet", "ef", "database", "update"]

FROM mcr.microsoft.com/dotnet/aspnet:5.0

# Add dotnet core env for enable swagger 
ENV ASPNETCORE_ENVIRONMENT=Development

USER root

# RUN apt-get update && apt-get upgrade
# RUN apt install -y curl
# RUN apt install -y lsof

WORKDIR /opt/rabbitMQ/publish

EXPOSE 8080

COPY --from=builder /opt/rabbitMQ/bin/Release/net5.0/publish .

# RUN ["dotnet", "tool", "install", "--global", "dotnet-ef"]
# RUN ["dotnet", "ef", "migrations", "add", "InitialCreate"]
# RUN ["dotnet", "ef", "database", "update"]

ENTRYPOINT ["dotnet", "rabbitMQ.dll"]