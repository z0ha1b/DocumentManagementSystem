using DocumentManagement.Core.Extensions;
using DocumentManagement.Core.Options;
using DocumentManagement.Persistence.Extensions;
using DocumentManagement.Workflows.Extensions;
using Storage.Net;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var dbConnectionString = configuration.GetConnectionString("Sqlite");

var services = builder.Services;

services.AddDomainServices();

// Configure Storage for DocumentStorage.

services.Configure<DocumentStorageOptions>(options => options.BlobStorageFactory = () => StorageFactory.Blobs.DirectoryFiles("Uploads"));
services.AddDomainPersistence(dbConnectionString);

builder.Services.AddControllers();

builder.Services.AddRazorPages();

// builder.Services.AddWorkflowServices(configuration);


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// app.AddWorkflowMiddlewares();

app.MapRazorPages();


app.Run();