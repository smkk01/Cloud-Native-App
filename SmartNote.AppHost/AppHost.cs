var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var database = postgres.AddDatabase("SmartNoteDb");
var redis = builder.AddRedis("redis");

builder.AddProject<Projects.SmartNote_Api>("smartnote-api")
.WithReference(database)
.WithReference(redis);
builder.Build().Run();
