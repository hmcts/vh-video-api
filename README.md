# vh-video-api

## HMCTS

[![Build Status](https://dev.azure.com/hmctsreform/VirtualHearings/_apis/build/status/Apps-CI/hmcts.vh-notification-api?repoName=hmcts%2Fvh-notification-api&branchName=master)](https://dev.azure.com/hmctsreform/VirtualHearings/_build/latest?definitionId=188&repoName=hmcts%2Fvh-notification-api&branchName=master)

[![VideoApi.Client package in vh-packages feed in Azure Artifacts](https://hmctsreform.feeds.visualstudio.com/3f69a23d-fbc7-4541-afc7-4cccefcad773/_apis/public/Packaging/Feeds/vh-packages/Packages/80002570-7840-44ca-8d91-58fe07774f40/Badge)](https://hmctsreform.visualstudio.com/VirtualHearings/_artifacts/feed/vh-packages/NuGet/VideoApi.Client?preferRelease=true)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=vh-notification-api&metric=alert_status)](https://sonarcloud.io/dashboard?id=vh-notification-api)

## SDS

[![Build Status](https://dev.azure.com/hmctsreform/VirtualHearings/_apis/build/status/Apps-CI/hmcts.vh-notification-api?repoName=hmcts%2Fvh-notification-api&branchName=master)](https://dev.azure.com/hmctsreform/VirtualHearings/_build/latest?definitionId=188&repoName=hmcts%2Fvh-notification-api&branchName=master)

[![VideoApi.Client package in vh-packages feed in Azure Artifacts](https://feeds.dev.azure.com/hmcts/cf3711aa-2aed-4f62-81a8-2afaee0ce26d/_apis/public/Packaging/Feeds/vh-packages/Packages/2cd477d4-635e-48e1-987f-1d91d35179a6/Badge)](https://dev.azure.com/hmcts/Video%20Hearings/_artifacts/feed/vh-packages/NuGet/VideoApi.Client?preferRelease=true)


[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=vh-notification-api&metric=alert_status)](https://sonarcloud.io/dashboard?id=vh-notification-api)

## Running Sonar Analysis

``` bash
dotnet sonarscanner begin /k:"vh-video-api" /d:sonar.cs.opencover.reportsPaths="VideoAPI/Artifacts/Coverage/coverage.opencover.xml" /d:sonar.coverage.exclusions="VideoApi/Program.cs,VideoApi/Startup.cs,VideoApi/Extensions/**,VideoApi/Swagger/**,**/VideoApi/ConfigureServicesExtensions.cs,**/Testing.Common/**,**/Testing.Common/Helper/,Helper/Builders/Api/,Helper/Builders/Domain/,VideoApi.Common/**,VideoApi.DAL/Mappings/**,VideoApi.DAL/SeedData/**,VideoApi.DAL/VideoApiDbContext.cs,VideoApi.DAL/**/DesignTimeHearingsContextFactory.cs,VideoApi.DAL/Migrations/**,VideoApi.Domain/Ddd/**,VideoApi.Domain/Validations/**" /d:sonar.cpd.exclusions="VideoApi.DAL/Migrations/**" /d:sonar.verbose=true
dotnet build VideoAPI/VideoApi.sln
dotnet sonarscanner end
```

## Running code coverage

First ensure you are running a terminal in the VideoAPI directory of this repository and then run the following commands.

``` bash
dotnet test --no-build VideoApi.UnitTests/VideoApi.UnitTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat="\"opencover,cobertura,json,lcov\"" /p:CoverletOutput=../Artifacts/Coverage/ /p:MergeWith='../Artifacts/Coverage/coverage.json' /p:Exclude="\"[*]VideoApi.Extensions.*,[VideoApi]VideoApi.ConfigureServicesExtensions,[VideoApi]VideoApi.Startup,[VideoApi]VideoApi.Program,[*]VideoApi.Swagger.*,[VideoApi.*Tests?]*,[*]VideoApi.DAL.SeedData.*,[*]VideoApi.DAL.Migrations.*,[*]VideoApi.DAL.Mappings.*,[*]VideoApi.Domain.Ddd.*,[*]VideoApi.Domain.Validations.*,[VideoApi.DAL]VideoApi.DAL.VideoApiDbContext,[VideoApi.DAL]VideoApi.DAL.DesignTimeHearingsContextFactory,[*]VideoApi.Common.*,[*]Testing.Common.*,[*]VideoApi.Services.*\""

dotnet test --no-build VideoApi.IntegrationTests/VideoApi.IntegrationTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat="\"opencover,cobertura,json,lcov\"" /p:CoverletOutput=../Artifacts/Coverage/ /p:MergeWith='../Artifacts/Coverage/coverage.json' /p:Exclude="\"[*]VideoApi.Extensions.*,[VideoApi]VideoApi.ConfigureServicesExtensions,[VideoApi]VideoApi.Startup,[VideoApi]VideoApi.Program,[*]VideoApi.Swagger.*,[VideoApi.*Tests?]*,[*]VideoApi.DAL.SeedData.*,[*]VideoApi.DAL.Migrations.*,[*]VideoApi.DAL.Mappings.*,[*]VideoApi.Domain.Ddd.*,[*]VideoApi.Domain.Validations.*,[VideoApi.DAL]VideoApi.DAL.VideoApiDbContext,[VideoApi.DAL]VideoApi.DAL.DesignTimeHearingsContextFactory,[*]VideoApi.Common.*,[*]Testing.Common.*,[*]VideoApi.Services.*\""

```

## Generate HTML Report

Under the unit test project directory

``` bash
dotnet reportgenerator "-reports:./Coverage/coverage.opencover.xml" "-targetDir:./Artifacts/Coverage/Report" -reporttypes:Html -sourcedirs:./VideoApi
```

##Branch name git hook will run on pre commit and control the standard for new branch name.

The branch name should start with: feature/VIH-XXXX-branchName  (X - is digit).
If git version is less than 2.9 the pre-commit file from the .githooks folder need copy to local .git/hooks folder.
To change git hooks directory to directory under source control run (works only for git version 2.9 or greater) :
$ git config core.hooksPath .githooks

##Commit message 
The commit message will be validated by prepare-commit-msg hook.
The commit message format should start with : 'feature/VIH-XXXX : ' folowing by 8 or more characters description of commit, otherwise the warning message will be presented.

## Run Zap scan locally

To run Zap scan locally update the following settings and run acceptance\integration tests

User Secrets:

- "Services:VideoApiUrl": "https://VideoApi_AC/"

Update following configuration under appsettings.json under VideoApi.AcceptanceTests or  VideoApi.IntegrationTests

- "Services:VideoApiUrl": "https://VideoApi_AC/"
- "ZapConfiguration:ZapScan": true
- "ConnectionStrings:VhVideoApi": "Server=localhost,1433;Database=VhVideoApi;User=sa;Password=VeryStrongPassword!;" (IntegrationTest alone)

Note: Ensure you have Docker desktop engine installed and setup


### Running all tests in Docker

Open a terminal at the root level of the repository and run the following command

```console
docker-compose -f "docker-compose.tests.yml" up --build --abort-on-container-exit
```

> You may need to create a `.env` file to store the environment variables