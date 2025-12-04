using Azure.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
    var con = new SqlConnection(
        builder.Configuration.GetConnectionString("DefaultConnection"));

app.MapGet("/", () => "Azure Data Solution Demo API is running in CU Azure Demo Web App");

app.MapGet("/products", async () =>
{
    var products = new List<string>();
    //var con = new SqlConnection(
    //    builder.Configuration.GetConnectionString("DefaultConnection"));

    await con.OpenAsync();
    var cmd = new SqlCommand("SELECT Name FROM Products", con);
    var reader = await cmd.ExecuteReaderAsync();

    while (await reader.ReadAsync())
        products.Add(reader.GetString(0));

    await con.CloseAsync();

    return products;
});

app.MapPost("/products", async (ProductRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
        return Results.BadRequest("Product name is required");

    //using var con = new SqlConnection(connStr);
    await con.OpenAsync();

    var cmd = new SqlCommand(
        "INSERT INTO Products (Name) VALUES (@name)",
        con);

    cmd.Parameters.AddWithValue("@name", request.Name);

    await cmd.ExecuteNonQueryAsync();

    await con.CloseAsync();

    return Results.Ok(new
    {
        Message = "Product added successfully",
        Product = request.Name
    });
});

app.Run();