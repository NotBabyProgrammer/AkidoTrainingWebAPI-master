FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AkidoTrainingWebAPI/AkidoTrainingWebAPI.csproj", "AkidoTrainingWebAPI/"]
RUN dotnet restore "AkidoTrainingWebAPI/AkidoTrainingWebAPI.csproj"
COPY . .
WORKDIR "/src/AkidoTrainingWebAPI"
RUN dotnet build "AkidoTrainingWebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AkidoTrainingWebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AkidoTrainingWebAPI.dll"]