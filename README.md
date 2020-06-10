# vh-video-api

## Running Sonar Analysis

``` bash
dotnet sonarscanner begin /k:"vh-video-api" /d:sonar.cs.opencover.reportsPaths="VideoAPI/Artifacts/Coverage/coverage.opencover.xml" /d:sonar.coverage.exclusions="Video.API/Program.cs,Video.API/Startup.cs,Video.API/Extensions/**,Video.API/Swagger/**,**/Video.API/ConfigureServicesExtensions.cs,**/Testing.Common/**,**/Testing.Common/Helper/,Helper/Builders/Api/,Helper/Builders/Domain/,VideoApi.Common/**,VideoApi.DAL/Mappings/**,VideoApi.DAL/SeedData/**,VideoApi.DAL/VideoApiDbContext.cs,VideoApi.DAL/**/DesignTimeHearingsContextFactory.cs,VideoApi.DAL/Migrations/**,VideoApi.Domain/Ddd/**,VideoApi.Domain/Validations/**" /d:sonar.cpd.exclusions="VideoApi.DAL/Migrations/**" /d:sonar.verbose=true
dotnet build VideoAPI/VideoApi.sln
dotnet sonarscanner end
```

## Running code coverage

First ensure you are running a terminal in the VideoAPI directory of this repository and then run the following commands.

``` bash
dotnet test --no-build VideoApi.UnitTests/VideoApi.UnitTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat="\"opencover,cobertura,json,lcov\"" /p:CoverletOutput=../Artifacts/Coverage/ /p:MergeWith='../Artifacts/Coverage/coverage.json' /p:Exclude="\"[*]Video.API.Extensions.*,[Video.API]Video.API.ConfigureServicesExtensions,[Video.API]Video.API.Startup,[Video.API]Video.API.Program,[*]Video.API.Swagger.*,[VideoApi.*Tests?]*,[*]VideoApi.DAL.SeedData.*,[*]VideoApi.DAL.Migrations.*,[*]VideoApi.DAL.Mappings.*,[*]VideoApi.Domain.Ddd.*,[*]VideoApi.Domain.Validations.*,[VideoApi.DAL]VideoApi.DAL.VideoApiDbContext,[VideoApi.DAL]VideoApi.DAL.DesignTimeHearingsContextFactory,[*]VideoApi.Common.*,[*]Testing.Common.*,[*]VideoApi.Services.*\""

dotnet test --no-build VideoApi.IntegrationTests/VideoApi.IntegrationTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat="\"opencover,cobertura,json,lcov\"" /p:CoverletOutput=../Artifacts/Coverage/ /p:MergeWith='../Artifacts/Coverage/coverage.json' /p:Exclude="\"[*]Video.API.Extensions.*,[Video.API]Video.API.ConfigureServicesExtensions,[Video.API]Video.API.Startup,[Video.API]Video.API.Program,[*]Video.API.Swagger.*,[VideoApi.*Tests?]*,[*]VideoApi.DAL.SeedData.*,[*]VideoApi.DAL.Migrations.*,[*]VideoApi.DAL.Mappings.*,[*]VideoApi.Domain.Ddd.*,[*]VideoApi.Domain.Validations.*,[VideoApi.DAL]VideoApi.DAL.VideoApiDbContext,[VideoApi.DAL]VideoApi.DAL.DesignTimeHearingsContextFactory,[*]VideoApi.Common.*,[*]Testing.Common.*,[*]VideoApi.Services.*\""

```

## Generate HTML Report

Under the unit test project directory

``` bash
dotnet reportgenerator "-reports:../Artifacts/Coverage/coverage.opencover.xml" "-targetDir:../Artifacts/Coverage/Report" -reporttypes:HtmlInline_AzurePipelines
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

## Run Stryker

To run stryker mutation test, go to UnitTest folder under command prompt and run the following command

```bash
dotnet stryker
```

From the results look for line(s) of code highlighted with Survived\No Coverage and fix them.


If in case you have not installed stryker previously, please use one of the following commands

### Global
```bash
dotnet tool install -g dotnet-stryker
```
### Local
```bash
dotnet tool install dotnet-stryker
```

To update latest version of stryker please use the following command

```bash
dotnet tool update --global dotnet-stryker
```