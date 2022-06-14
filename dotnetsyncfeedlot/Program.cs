using Couchbase.Lite.Query;
using dotnetsyncfeedlot;

var dbManager = new DatabaseManager();
var database = dbManager.GetDatabase();
var watch = new System.Diagnostics.Stopwatch();
watch.Start();
dbManager.StartReplicationAsync();
while (!dbManager.GetStatus())
{
}
var query = QueryBuilder.Select(SelectResult.All())
    .From(DataSource.Database(database));
// Run the query
var result = query.Execute(); 
Console.WriteLine($"Number of rows :: {result.AllResults().Count}");
watch.Stop();
Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
