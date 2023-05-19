#!/bin/sh
set -x

exclusions="[Testing.Common]*,[VideoApi.Common]VideoApi.Common.*,[VideoApi.Domain]*.Ddd*,[VideoApi.DAL]*.Migrations*,[VideoApi]*.Swagger,[VideoApi.Services]*"
configuration=Release

# Script is for docker compose tests where the script is at the root level
dotnet test VideoApi/VideoApi.UnitTests/VideoApi.UnitTests.csproj -c $configuration --results-directory ./TestResults --logger "trx;LogFileName=VideoApi-Unit-Tests-TestResults.trx" \
    "/p:CollectCoverage=true" \
    "/p:Exclude=\"${exclusions}\"" \
    "/p:CoverletOutput=${PWD}/Coverage/" \
    "/p:MergeWith=${PWD}/Coverage/coverage.json" \
    "/p:CoverletOutputFormat=\"opencover,json,cobertura,lcov\""
dotnet test VideoApi/VideoApi.IntegrationTests/VideoApi.IntegrationTests.csproj -c $configuration --results-directory ./TestResults --logger "trx;LogFileName=VideoApi-Integration-Tests-TestResults.trx" \
    "/p:CollectCoverage=true" \
    "/p:Exclude=\"${exclusions}\"" \
    "/p:CoverletOutput=${PWD}/Coverage/" \
    "/p:MergeWith=${PWD}/Coverage/coverage.json" \
    "/p:CoverletOutputFormat=\"opencover,json,cobertura,lcov\"
    