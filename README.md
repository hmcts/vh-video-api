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

## Running ZAP Security Testing

Download and Install ZAP from https://www.zaproxy.org/download/

Configure or note the ZAP API Key from Tools > options > API > API Key field

Configure or note the port under which ZAP is run from Tools > options > Local Proxies > Port 

Install Certificate by going to Tools>Options>Dynamic SSL Certificate. Click Generate and then click Save.
Save the certificate in the desired location. Open your browser and install the Certificate to your browser (Firefox, Chrome, IE) accordingly

Update ZAPConfiguration parameters on with the values observed above ApiAddress, ApiPort, ApiKey (User Secrets)

Update WorkingDirectory with location under which ZAP.exe is installed

To turn on\off ZAP security passive scan Set RunZAP - true\false

To turn on\off ZAP security scans individually such as Spider\AjaxSpider\ActiveScan set respective parameters RunSpider\RunAjaxSpider\RunActiveScan - true\false


##Branch name git hook will run on pre commit and control the standard for new branch name.

The branch name should start with: feature/VIH-XXXX-branchName  (X - is digit).
If git version is less than 2.9 the pre-commit file from the .githooks folder need copy to local .git/hooks folder.
To change git hooks directory to directory under source control run (works only for git version 2.9 or greater) :
$ git config core.hooksPath .githooks

##Commit message 
The commit message will be validated by prepare-commit-msg hook.
The commit message format should start with : 'feature/VIH-XXXX : ' folowing by 8 or more characters description of commit, otherwise the warning message will be presented.


