ARG SDK_VERSION=2.2
ARG RUNTIME_VERSION=2.2

FROM mcr.microsoft.com/dotnet/core/aspnet:${RUNTIME_VERSION} AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80;https://+:443
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:${SDK_VERSION} AS build
WORKDIR /src
COPY . .
RUN dotnet restore Video.API
RUN dotnet build Video.API -c Release -o /app/build

FROM build AS publish
RUN dotnet publish Video.API -c Release -o /app/publish

FROM base AS final
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Video.API.dll"]
