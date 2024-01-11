#!/bin/sh
set -x

exclusions="[Testing.Common]*,[VideoApi.Common]VideoApi.Common.*,[VideoApi.Domain]*.Ddd*,[VideoApi.DAL]*.Migrations*,[VideoApi]*.Swagger"
configuration=Release

# Script is for docker compose tests where the script is at the root level
dotnet test VideoApi/VideoApi.UnitTests/VideoApi.UnitTests.csproj -c $configuration --results-directory ./TestResults --logger "trx;LogFileName=VideoApi-Unit-Tests-TestResults.trx" \
    "/p:CollectCoverage=true" \
    "/p:Exclude=\"${exclusions}\"" \
    "/p:CoverletOutput=${PWD}/Coverage/" \
    "/p:MergeWith=${PWD}/Coverage/coverage.json" \
    "/p:CoverletOutputFormat=\"opencover,json,cobertura,lcov\"" ||
    {
        echo "##vso[task.logissue type=error]DotNet Unit Tests Failed."
        echo "##vso[task.complete result=Failed]"
        exit 1
    }

dotnet ef database update -p VideoApi/VideoApi.DAL/VideoApi.DAL.csproj -s VideoApi/VideoApi.DAL/VideoApi.DAL.csproj
dotnet ef database update -p RefData/RefData.csproj -s RefData/RefData.csproj

dotnet test VideoApi/VideoApi.IntegrationTests/VideoApi.IntegrationTests.csproj -c $configuration --results-directory ./TestResults --logger "trx;LogFileName=VideoApi-Integration-Tests-TestResults.trx" \
    "/p:CollectCoverage=true" \
    "/p:Exclude=\"${exclusions}\"" \
    "/p:CoverletOutput=${PWD}/Coverage/" \
    "/p:MergeWith=${PWD}/Coverage/coverage.json" \
    "/p:CoverletOutputFormat=\"opencover,json,cobertura,lcov\"" ||
    {
        echo "##vso[task.logissue type=error]DotNet Integration Tests Failed."
        echo "##vso[task.complete result=Failed]"
        exit 1
    }
