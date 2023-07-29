// See https://aka.ms/new-console-template for more information
using AutoFixture;
using SimpleFileDbV1.db;
using System.Diagnostics;

var fixture = new Fixture();
var stopwatch = new Stopwatch();

var config = new SimpleFileDbConfiguration(@"C:\Temp\SimpleFileDb\v1\");
config.RegisterDataType<TestObjectA>(i => i.Id);

ISimpleFileDb db = new SimpleFileDb(config);

var testObjects = fixture.CreateMany<TestObjectA>(100);


Console.WriteLine("Saving items to db");
stopwatch.Start();
foreach (var item in testObjects)
{
    db.Save(item);
}
stopwatch.Stop();
Console.WriteLine($"Items saved to db in {stopwatch.Elapsed}");


Console.WriteLine("Fetching items from db one by one");
stopwatch.Restart();
foreach (var item in testObjects)
{
    var storedItem = db.GetById<TestObjectA>(item.Id);
}
stopwatch.Stop();
Console.WriteLine($"Items fetched from db in {stopwatch.Elapsed}");


Console.WriteLine("Fetching all items from db");
stopwatch.Restart();
var allItems = db.GetAll<TestObjectA>().ToList();
stopwatch.Stop();
Console.WriteLine($"All items fetched from db in {stopwatch.Elapsed}");


Console.WriteLine("Deleting items from db one by one");
stopwatch.Restart();
foreach (var item in testObjects)
{
    db.DeleteById<TestObjectA>(item.Id);
}
stopwatch.Stop();
Console.WriteLine($"Items deleted from db in {stopwatch.Elapsed}");



internal record TestObjectA(int Id, string Text, List<TestSubObjectA> SubObjects);

internal record TestSubObjectA(int Id, string Text);

