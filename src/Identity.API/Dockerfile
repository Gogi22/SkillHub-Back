FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS publish
WORKDIR /src
COPY ["src/Identity.API/Identity.API.csproj", "Identity.API/"]
COPY ["src/Common/Common.csproj", "Common/"]
RUN dotnet restore "Identity.API/Identity.API.csproj"
COPY src/Common Common/
COPY src/Identity.API Identity.API
WORKDIR /src/Identity.API
RUN dotnet publish "Identity.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV SqlServerConnectionString="Server=host.docker.internal;Database=Identity;User Id=sa;Password=YourPassword123!;Trust Server Certificate=true;"
ENV RabbitMQ__Host=host.docker.internal
ENTRYPOINT ["dotnet", "Identity.API.dll"]