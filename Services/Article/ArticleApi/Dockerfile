#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Article/ArticleApi/ArticleApi.csproj", "Services/Article/ArticleApi/"]
COPY ["Services/Article/Application/Application.csproj", "Services/Article/Application/"]
COPY ["Services/Article/Domain/Domain.csproj", "Services/Article/Domain/"]
COPY ["Services/Article/Persistence/Persistence.csproj", "Services/Article/Persistence/"]
# RUN dotnet restore "./Services/Article/ArticleApi/./ArticleApi.csproj"
RUN dotnet restore "Services/Article/ArticleApi/ArticleApi.csproj"
COPY . .
WORKDIR "/src/Services/Article/ArticleApi"
# RUN dotnet build "./ArticleApi.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet build "ArticleApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
# RUN dotnet publish "./ArticleApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
# RUN dotnet publish "ArticleApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
RUN dotnet publish "ArticleApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ArticleApi.dll"]