#!/bin/sh
set -x

rm -d -r ${PWD}/Coverage
rm -d -r ${PWD}/TestResults

# dotnet sonarscanner begin /k:"${SONAR_PROJECT_KEY}" /o:"${SONAR_ORG}" /version:"${SONAR_PROJECT_VERSION}" /name:"${SONAR_PROJECT_NAME}" /d:sonar.host.url="${SONAR_HOST}" /d:sonar.login="${SONAR_TOKEN}" /d:sonar.cs.opencover.reportsPaths="${PWD}/Coverage/coverage.opencover.xml" /d:sonar.coverage.exclusions="**/VideoApi/Swagger/**/*,**/Program.cs,**/Startup.cs,**/Testing.Common/**/*,**/VideoApi.Common/**/*,**/VideoApi.IntegrationTests/**/*,**/VideoApi.UnitTests/**/*,**/VideoApi/Extensions/*,**/VideoApi.DAL/Migrations/**/*" /d:sonar.cpd.exclusions="**/Program.cs,**/Startup.cs,**/Testing.Common/**/*,**/VideoApi/Swagger/**/*,VideoApi/VideoApi.DAL/Migrations/*,VideoApi/VideoApi.DAL/TemplateDataForEnvironments.cs"

exclusions="[Testing.Common]*,[VideoApi.Common]VideoApi.Common.*,[VideoApi.Domain]*.Ddd*,[VideoApi.DAL]*.Migrations*,[VideoApi]*.Swagger,[VideoApi.Services]*"

dotnet build VideoApi/VideoApi.sln -c Release
# Script is for docker compose tests where the script is at the root level
# dotnet test VideoApi/VideoApi.UnitTests/VideoApi.UnitTests.csproj -c Release --no-build --results-directory ./TestResults --logger "trx;LogFileName=VideoApi-Unit-Tests-TestResults.trx" \
#     "/p:CollectCoverage=true" \
#     "/p:Exclude=\"${exclusions}\"" \
#     "/p:CoverletOutput=${PWD}/Coverage/" \
#     "/p:MergeWith=${PWD}/Coverage/coverage.json" \
#     "/p:CoverletOutputFormat=\"opencover,json,cobertura,lcov\""

dotnet test VideoApi/VideoApi.IntegrationTests/VideoApi.IntegrationTests.csproj -c Release --no-build --filter TestCategory=azurite --results-directory ./TestResults --logger "trx;LogFileName=VideoApi-Integration-Tests-TestResults.trx" \
    "/p:CollectCoverage=true" \
    "/p:Exclude=\"${exclusions}\"" \
    "/p:CoverletOutput=${PWD}/Coverage/" \
    "/p:MergeWith=${PWD}/Coverage/coverage.json" \
    "/p:CoverletOutputFormat=\"opencover,json,cobertura,lcov\""

# dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN}"
