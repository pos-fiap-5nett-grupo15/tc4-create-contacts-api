FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /App

COPY ./CreateContact/. ./

RUN dotnet restore  ./CreateContact.Api/CreateContact.Api.csproj
RUN dotnet build ./CreateContact.Api/CreateContact.Api.csproj
RUN dotnet publish  ./CreateContact.Api/CreateContact.Api.csproj -c Release --output Out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /App
COPY --from=build /App/Out/ ./

ENTRYPOINT ["dotnet", "CreateContact.Api.dll"]

