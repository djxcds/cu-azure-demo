using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Azure Data Solution Demo API is running!");

app.MapGet("/products", async () =>
{
    var products = new List<string>();
    var con = new SqlConnection(
        builder.Configuration.GetConnectionString("DefaultConnection"));

    await con.OpenAsync();
    var cmd = new SqlCommand("SELECT Name FROM Products", con);
    var reader = await cmd.ExecuteReaderAsync();

    while (await reader.ReadAsync())
        products.Add(reader.GetString(0));

    return products;
});

app.Run();