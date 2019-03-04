# vh-video-api

## Running Sonar Analysis

``` bash
dotnet sonarscanner begin /k:"hmcts-vh-video-api" /d:sonar.cs.opencover.reportsPaths="Artifacts/Coverage/coverage.opencover.xml" /d:sonar.coverage.exclusions="Video.API/Program.cs,Video.API/Startup.cs,Video.API/Extensions/**,Video.API/Swagger/**,Video.API/ConfigureServicesExtensions.cs,Testing.Common/**,VideoApi.Common/**,VideoApi.DAL/Mappings/**,VideoApi.DAL/SeedData/**,VideoApi.DAL/VideoApiDbContext.cs,VideoApi.DAL/**/DesignTimeHearingsContextFactory.cs,VideoApi.DAL/Migrations/**,VideoApi.Domain/Ddd/**,VideoApi.Domain/Validations/**,VideoApi.DAL/Commands/**,VideoApi.DAL/Queries/**" /d:sonar.cpd.exclusions="VideoApi.DAL/Migrations/**" /d:sonar.verbose=false
dotnet build
dotnet sonarscanner end

```

## Running code coverage

First ensure you are running a terminal in the VideoAPI directory of this repository and then run the following commands.

``` bash
dotnet test --no-build VideoApi.UnitTests/VideoApi.UnitTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat="\"opencover,cobertura,json,lcov\"" /p:CoverletOutput=../Artifacts/Coverage/ /p:MergeWith='../Artifacts/Coverage/coverage.json' /p:Exclude="\"[VideoApi.*Tests?]*,[Video.API]Startup,Video.API.Swagger]*,[*]VideoApi.DAL.SeedData.*,[*]VideoApi.DAL.Migrations.*,[*]VideoApi.DAL.Mappings.*,[*]VideoApi.Domain.Ddd.*,[VideoApi.DAL]VideoApiDbContext,[VideoApi.DAL]VideoApi.DAL.DesignTimeHearingsContextFactory,[VideoApi.Common]*,[Testing.Common]*\""

dotnet test --no-build VideoApi.IntegrationTests/VideoApi.IntegrationTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat="\"opencover,cobertura,json,lcov\"" /p:CoverletOutput=../Artifacts/Coverage/ /p:MergeWith='../Artifacts/Coverage/coverage.json' /p:Exclude="\"[VideoApi.*Tests?]*,[Video.API]Startup,Video.API.Swagger]*,[*]VideoApi.DAL.SeedData.*,[*]VideoApi.DAL.Migrations.*,[*]VideoApi.DAL.Mappings.*,[*]VideoApi.Domain.Ddd.*,[VideoApi.DAL]VideoApiDbContext,[VideoApi.DAL]VideoApi.DAL.DesignTimeHearingsContextFactory,[VideoApi.Common]*,[Testing.Common]*\""

```