# Create Base Image.
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Publish .NET App.
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS publish
WORKDIR /src
COPY VideoApi .
RUN dotnet restore --configfile nuget.config "VideoApi/VideoApi.csproj"
WORKDIR "/src/VideoApi"
RUN dotnet publish "VideoApi.csproj" -c Release -o /app/publish

# Create Final Image.
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VideoApi.dll"]