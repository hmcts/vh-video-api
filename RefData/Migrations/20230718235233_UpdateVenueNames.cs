using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RefData.Migrations
{
    public partial class UpdateVenueNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlFile1 = Path.Combine("data/9777_update_venue_names.sql");
            RunSqlFile(File.ReadAllText(sqlFile1), migrationBuilder);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }

        /// <summary>
        /// EF Core does not like GO statements in the migration files, so we need to split the file into batches
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="migrationBuilder"></param>
        private void RunSqlFile(string sql, MigrationBuilder migrationBuilder)
        {
            string[] batches = sql.Split(new [] {"GO;"}, StringSplitOptions.None);
            foreach (string batch in batches)
            {
                migrationBuilder.Sql(batch);
            }
        }
    }
}
