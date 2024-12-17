FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /App

COPY ./CreateContact/. ./

RUN dotnet restore  ./CreateContract.Worker/CreateContract.Worker.csproj
RUN dotnet build ./CreateContract.Worker/CreateContract.Worker.csproj
RUN dotnet publish  ./CreateContract.Worker/CreateContract.Worker.csproj -c Release --output Out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /App
COPY --from=build /App/Out/ ./

ENTRYPOINT ["dotnet", "CreateContract.Worker.dll"]

