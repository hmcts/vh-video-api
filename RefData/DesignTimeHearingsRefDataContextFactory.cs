namespace RefData
{
    [ExcludeFromCodeCoverage]
    public class DesignTimeHearingsRefDataContextFactory : IDesignTimeDbContextFactory<RefDataContext>
    {
        public RefDataContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddUserSecrets("9AECE566-336D-4D16-88FA-7A76C27321CD")
                .AddEnvironmentVariables()
                .Build();
            var builder = new DbContextOptionsBuilder<RefDataContext>();
            builder.UseSqlServer(config.GetConnectionString("VideoApi"));
            builder.ReplaceService<IRelationalCommandBuilderFactory, DynamicSqlRelationalCommandBuilderFactory>();
            var context = new RefDataContext(builder.Options);
            return context;
        }
    }

    [ExcludeFromCodeCoverage]
    public class DynamicSqlRelationalCommandBuilder : RelationalCommandBuilder
    {
        public DynamicSqlRelationalCommandBuilder(RelationalCommandBuilderDependencies dependencies) : base(dependencies)
        {
        }

        public override IRelationalCommand Build()
        {
            var commandText = base.Build().CommandText;
            commandText = commandText.Replace("SET XACT_ABORT ON;", string.Empty);
            commandText = commandText.Replace("SET XACT_ABORT ON", string.Empty);
            commandText = commandText.Replace("SET XACT_ABORT OFF;", string.Empty);
            commandText = commandText.Replace("SET XACT_ABORT OFF", string.Empty);
            commandText = commandText.Replace("BEGIN TRANSACTION;", string.Empty);
            commandText = commandText.Replace("BEGIN TRANSACTION", string.Empty);
            
            commandText = commandText.Replace("COMMIT TRANSACTION;", string.Empty);
            commandText = commandText.Replace("COMMIT;", string.Empty);
            commandText = commandText.Replace("COMMIT", string.Empty);
            commandText = "EXECUTE ('" + commandText.Replace("'", "''") + "')";
            return new RelationalCommand(Dependencies, commandText, Parameters);
        }
    }
    
    [ExcludeFromCodeCoverage]
    public class DynamicSqlRelationalCommandBuilderFactory : RelationalCommandBuilderFactory
    {
        public DynamicSqlRelationalCommandBuilderFactory(RelationalCommandBuilderDependencies dependencies) : base(dependencies)
        {
        }

        public override IRelationalCommandBuilder Create()
        {
            return new DynamicSqlRelationalCommandBuilder(Dependencies);
        }
    }
}