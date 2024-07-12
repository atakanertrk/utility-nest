FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /App
COPY --from=build-env /App/out .

ENV ASPNETCORE_URLS=http://+:4998
EXPOSE 4998
ENTRYPOINT ["dotnet", "TaskAPI.dll"]