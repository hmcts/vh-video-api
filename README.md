# vh-video-api

## Running code coverage

``` bash
dotnet test --no-build VideoAPI/VideoApi.UnitTests/VideoApi.UnitTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat="\"opencover,cobertura,json,lcov\"" /p:CoverletOutput=../Artifacts/Coverage/ /p:MergeWith='../Artifacts/Coverage/coverage.json' /p:Exclude="\"[VideoApi.*Tests?]*,[Video.API]Startup,[*]VideoApi.DAL.SeedData.*,[*]VideoApi.DAL.Migrations.*,[*]VideoApi.DAL.Mappings.*,[VideoApi.DAL]Bookings.DAL.VideoApiDbContext,[VideoApi.DAL]Bookings.DAL.DesignTimeHearingsContextFactory,[VideoApi.Common]*,[Testing.Common]*\""

dotnet test --no-build VideoAPI/VideoApi.IntegrationTests/VideoApi.IntegrationTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat="\"opencover,cobertura,json,lcov\"" /p:CoverletOutput=../Artifacts/Coverage/ /p:MergeWith='../Artifacts/Coverage/coverage.json' /p:Exclude="\"[VideoApi.*Tests?]*,[Video.API]Startup,[*]VideoApi.DAL.SeedData.*,[*]VideoApi.DAL.Migrations.*,[*]VideoApi.DAL.Mappings.*,[VideoApi.DAL]Bookings.DAL.VideoApiDbContext,[VideoApi.DAL]Bookings.DAL.DesignTimeHearingsContextFactory,[VideoApi.Common]*,[Testing.Common]*\""

```