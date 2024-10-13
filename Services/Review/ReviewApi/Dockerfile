#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# USER app
WORKDIR /app
EXPOSE 8082
EXPOSE 8083

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Review/ReviewApi/ReviewApi.csproj", "Services/Review/ReviewApi/"]
COPY ["Services/Review/Application/Application.csproj", "Services/Review/Application/"]
COPY ["Services/Review/Domain/Domain.csproj", "Services/Review/Domain/"]
COPY ["Services/Review/Persistence/Persistence.csproj", "Services/Review/Persistence/"]
COPY ["Services/Review/Proxies/Proxies.csproj", "Services/Review/Proxies/"]
# RUN dotnet restore "./Services/Review/ReviewApi/./ReviewApi.csproj"
RUN dotnet restore "Services/Review/ReviewApi/ReviewApi.csproj"
COPY . .
WORKDIR "/src/Services/Review/ReviewApi"
# RUN dotnet build "./ReviewApi.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet build "ReviewApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
# RUN dotnet publish "./ReviewApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
# RUN dotnet publish "ReviewApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
RUN dotnet publish "ReviewApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReviewApi.dll"]