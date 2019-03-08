# vh-video-api

## Running Sonar Analysis

``` bash
dotnet sonarscanner begin /k:"vh-video-api" /d:sonar.cs.opencover.reportsPaths="VideoAPI/Artifacts/Coverage/coverage.opencover.xml" /d:sonar.coverage.exclusions="Video.API/Program.cs,Video.API/Startup.cs,Video.API/Extensions/**,Video.API/Swagger/**,Video.API/ConfigureServicesExtensions.cs,**/Testing.Common/**,**/Testing.Common/Helper/,**/Testing.Common/Helper/Builders/Api,Testing.Common/Helper/Builders/Domain,VideoApi.Common/**,VideoApi.DAL/Mappings/**,VideoApi.DAL/SeedData/**,VideoApi.DAL/VideoApiDbContext.cs,VideoApi.DAL/**/DesignTimeHearingsContextFactory.cs,VideoApi.DAL/Migrations/**,VideoApi.Domain/Ddd/**,VideoApi.Domain/Validations/**,VideoApi.DAL/Commands/Core/**,VideoApi.DAL/Queries/Core/**" /d:sonar.cpd.exclusions="VideoApi.DAL/Migrations/**" /d:sonar.verbose=true
dotnet build VideoAPI/VideoApi.sln
dotnet sonarscanner end

```

## Running code coverage

First ensure you are running a terminal in the VideoAPI directory of this repository and then run the following commands.

``` bash
dotnet test --no-build VideoApi.UnitTests/VideoApi.UnitTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat="\"opencover,cobertura,json,lcov\"" /p:CoverletOutput=../Artifacts/Coverage/ /p:MergeWith='../Artifacts/Coverage/coverage.json' /p:Exclude="\"[*]Video.API.Extensions.*,[Video.API]Video.API.ConfigureServicesExtensions,[Video.API]Video.API.Startup,[Video.API]Video.API.Program,[*]Video.API.Swagger.*,[*]VideoApi.DAL.Commands.Core.*,[*]VideoApi.DAL.Queries.Core.*,[VideoApi.*Tests?]*,[*]VideoApi.DAL.SeedData.*,[*]VideoApi.DAL.Migrations.*,[*]VideoApi.DAL.Mappings.*,[*]VideoApi.Domain.Ddd.*,[*]VideoApi.Domain.Validations.*,[VideoApi.DAL]VideoApi.DAL.VideoApiDbContext,[VideoApi.DAL]VideoApi.DAL.DesignTimeHearingsContextFactory,[*]VideoApi.Common.*,[*]Testing.Common.*,[VideoApi.Events.Handlers.Core]VideoApi.Events.Handlers.Core.EventHandler\""

dotnet test --no-build VideoApi.IntegrationTests/VideoApi.IntegrationTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat="\"opencover,cobertura,json,lcov\"" /p:CoverletOutput=../Artifacts/Coverage/ /p:MergeWith='../Artifacts/Coverage/coverage.json' /p:Exclude="\"[*]Video.API.Extensions.*,[Video.API]Video.API.ConfigureServicesExtensions,[Video.API]Video.API.Startup,[Video.API]Video.API.Program,[*]Video.API.Swagger.*,[*]VideoApi.DAL.Commands.Core.*,[*]VideoApi.DAL.Queries.Core.*,[VideoApi.*Tests?]*,[*]VideoApi.DAL.SeedData.*,[*]VideoApi.DAL.Migrations.*,[*]VideoApi.DAL.Mappings.*,[*]VideoApi.Domain.Ddd.*,[*]VideoApi.Domain.Validations.*,[VideoApi.DAL]VideoApi.DAL.VideoApiDbContext,[VideoApi.DAL]VideoApi.DAL.DesignTimeHearingsContextFactory,[*]VideoApi.Common.*,[*]Testing.Common.*,[VideoApi.Events.Handlers.Core]VideoApi.Events.Handlers.Core.EventHandler\""

```

## Generate HTML Report

Under the unit test project directory

``` bash
dotnet reportgenerator "-reports:../Artifacts/Coverage/coverage.opencover.xml" "-targetDir:../Artifacts/Coverage/Report" -reporttypes:HtmlInline_AzurePipelines
```