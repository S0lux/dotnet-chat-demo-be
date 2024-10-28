FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["dotnet-simple-chat-be/dotnet-simple-chat-be.csproj", "dotnet-simple-chat-be/"]
RUN dotnet restore "dotnet-simple-chat-be/dotnet-simple-chat-be.csproj"
COPY . .
WORKDIR "/src/dotnet-simple-chat-be"
RUN dotnet build "dotnet-simple-chat-be.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "dotnet-simple-chat-be.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dotnet-simple-chat-be.dll"]
