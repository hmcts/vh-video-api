namespace RefData.Migrations;

public static class SqlFileHelper
{
    public static void RunSqlFile(string filePath, MigrationBuilder migrationBuilder)
    {
        Console.WriteLine("Path is " + filePath);
        var sql = File.ReadAllText(filePath);
        var batches = sql.Split(new [] {"GO;"}, StringSplitOptions.None);
        foreach (var batch in batches)
        {
            if(string.IsNullOrWhiteSpace(batch)) continue;
            Console.WriteLine("---Start--------------------");
            Console.WriteLine(batch);
            Console.WriteLine("---End--------------------");
            migrationBuilder.Sql(batch);
        }
    }
}