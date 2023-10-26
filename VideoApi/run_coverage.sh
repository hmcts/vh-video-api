rm -rf Artifacts

exclude=\"[*]VideoApi.Extensions.*,[VideoApi]VideoApi.ConfigureServicesExtensions,[VideoApi]VideoApi.Startup,[VideoApi]VideoApi.Program,[*]VideoApi.Swagger.*,[VideoApi.*Tests?]*,[*]VideoApi.DAL.SeedData.*,[*]VideoApi.DAL.Migrations.*,[*]VideoApi.DAL.Mappings.*,[*]VideoApi.Domain.Ddd.*,[*]VideoApi.Domain.Validations.*,[VideoApi.DAL]VideoApi.DAL.VideoApiDbContext,[VideoApi.DAL]VideoApi.DAL.DesignTimeHearingsContextFactory,[*]VideoApi.Common.*,[*]Testing.Common.*"
dotnet test VideoApi.UnitTests/VideoApi.UnitTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat="\"opencover,cobertura,json,lcov\"" /p:CoverletOutput=../Artifacts/Coverage/ /p:MergeWith='../Artifacts/Coverage/coverage.json' /p:Exclude="${exclude}"
dotnet test VideoApi.IntegrationTests/VideoApi.IntegrationTests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat="\"opencover,cobertura,json,lcov\"" /p:CoverletOutput=../Artifacts/Coverage/ /p:MergeWith='../Artifacts/Coverage/coverage.json' /p:Exclude="${exclude}"

~/.dotnet/tools/reportgenerator -reports:Artifacts/Coverage/coverage.opencover.xml -targetDir:Artifacts/Coverage/Report -reporttypes:HtmlInline_AzurePipelines

open Artifacts/Coverage/Report/index.htm
