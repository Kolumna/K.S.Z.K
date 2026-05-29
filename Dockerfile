FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app


COPY src/Krasnoludki.Web/Krasnoludki.Web.csproj ./src/Krasnoludki.Web/

COPY src/ ./src/

RUN dotnet publish ./src/Krasnoludki.Web/Krasnoludki.Web.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build-env /app/out .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Krasnoludki.Web.dll"]