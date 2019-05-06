FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY VehiclesAPI.csproj VehiclesAPI/
RUN dotnet restore VehiclesAPI/VehiclesAPI.csproj
WORKDIR /src/VehiclesAPI
COPY . .
RUN dotnet build VehiclesAPI.csproj -c Debug -o /app

FROM build AS publish
RUN dotnet publish VehiclesAPI.csproj -c Debug -o /app

FROM base AS final
COPY --from=publish /app .
EXPOSE 80
CMD ["dotnet", "VehiclesAPI.dll", "--urls", "http://0.0.0.0:80"]
