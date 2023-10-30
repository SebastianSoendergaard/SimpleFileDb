// See https://aka.ms/new-console-template for more information
using AutoFixture;
using SimpleFileDatabase;
using System.Diagnostics;

var fixture = new Fixture();
var stopwatch = new Stopwatch();

/*
 * Version 2
 * Implement full CRUD operations, by simply creating, reading, updating and deleting files with the objects serialized as json.
 * Supports transactions
 * 
 * Not included:
 * - handling of file write failure
 * - thread safety
 * - indexing / searching options
 * - caching to improve performance
 * - other file formats
 * 
 */

var config = new SimpleFileDbConfiguration(@"C:\Temp\SimpleFileDb\v2\");
config.RegisterDataType<TestObjectA>(i => i.Id);

ISimpleFileDb db = new SimpleFileDb(config);

var testObjects = fixture.CreateMany<TestObjectA>(10);

// ==================================================
// CRUD operations
// ==================================================

Console.WriteLine("Creating items in db");
stopwatch.Start();
foreach (var item in testObjects)
{
    await db.CreateAsync(item);
}
stopwatch.Stop();
Console.WriteLine($"Items created in db in {stopwatch.Elapsed}");


Console.WriteLine("Fetching items from db one by one");
stopwatch.Restart();
foreach (var item in testObjects)
{
    var storedItem = await db.GetByIdAsync<TestObjectA>(item.Id);
}
stopwatch.Stop();
Console.WriteLine($"Items fetched from db in {stopwatch.Elapsed}");


Console.WriteLine("Updating items in db one by one");
stopwatch.Restart();
foreach (var item in testObjects)
{
    var newItem = item with { Text = fixture.Create<string>() };
    await db.UpdateAsync(newItem);
}
stopwatch.Stop();
Console.WriteLine($"Items updated in db in {stopwatch.Elapsed}");


Console.WriteLine("Fetching all items from db");
stopwatch.Restart();
var allItems = await GetAllTestObjects();
stopwatch.Stop();
Console.WriteLine($"All items fetched from db in {stopwatch.Elapsed}");


Console.WriteLine("Deleting items from db one by one");
stopwatch.Restart();
foreach (var item in testObjects)
{
    await db.DeleteByIdAsync<TestObjectA>(item.Id);
}
stopwatch.Stop();
Console.WriteLine($"Items deleted from db in {stopwatch.Elapsed}");

// ==================================================
// Transactions
// ==================================================

var transTestObjects = fixture.CreateMany<TestObjectA>(10).ToList();

Console.WriteLine("Insert items in transaction");
using (var trans = db.BeginTransaction())
{
    transTestObjects.ForEach(async x => await db.CreateAsync(x));
}
Console.WriteLine($"Inserted item count {(await GetAllTestObjects()).Count}");

Console.WriteLine("Insert items in transaction");
using (var trans = db.BeginTransaction())
{
    transTestObjects.ForEach(async x => await db.CreateAsync(x));
    db.CommitTransaction(trans);
}
Console.WriteLine($"Inserted item count {(await GetAllTestObjects()).Count}");

async Task<List<TestObjectA>> GetAllTestObjects()
{
    List<TestObjectA> allItems = new();
    await foreach (var item in db.GetAllAsync<TestObjectA>())
    {
        allItems.Add(item);
    }
    return allItems;
}


internal record TestObjectA(int Id, string Text, List<TestSubObjectA> SubObjects);

internal record TestSubObjectA(int Id, string Text);

