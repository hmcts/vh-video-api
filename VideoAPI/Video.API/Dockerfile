#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Video.API/Video.API.csproj", "Video.API/"]
COPY ["VideoApi.Common/VideoApi.Common.csproj", "VideoApi.Common/"]
COPY ["VideoApi.Contract/VideoApi.Contract.csproj", "VideoApi.Contract/"]
COPY ["VideoApi.Domain/VideoApi.Domain.csproj", "VideoApi.Domain/"]
COPY ["VideoApi.Services/VideoApi.Services.csproj", "VideoApi.Services/"]
COPY ["VideoApi.Services.Generator/VideoApi.Services.Generator.csproj", "VideoApi.Services.Generator/"]
COPY ["VideoApi.DAL/VideoApi.DAL.csproj", "VideoApi.DAL/"]
COPY ["VideoApi.Events/VideoApi.Events.csproj", "VideoApi.Events/"]
RUN dotnet restore "Video.API/Video.API.csproj"
COPY . .
WORKDIR "/src/Video.API"
RUN dotnet build "Video.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Video.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Video.API.dll"]