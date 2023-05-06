FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SkillHub.API/SkillHub.API.csproj", "SkillHub.API/"]
RUN dotnet restore "SkillHub.API/SkillHub.API.csproj"
COPY . .
WORKDIR "/src/SkillHub.API"
RUN dotnet build "SkillHub.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SkillHub.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SkillHub.API.dll"]
