# Create Base Image.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app
EXPOSE 80
EXPOSE 443

# Publish .NET App.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS publish
WORKDIR /src
COPY VideoApi .
RUN dotnet restore --configfile nuget.config "VideoApi/VideoApi.csproj"
WORKDIR "/src/VideoApi"
RUN dotnet publish "VideoApi.csproj" -c Release -o /app/publish

# Create Final Image.
FROM base AS final
RUN apt-get update && apt-get install -y curl \
    && apt-get clean
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VideoApi.dll"]
