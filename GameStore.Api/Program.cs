using Gamestore.Api.Data;
using Gamestore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("GameStore");

builder.Services.AddSqlite<GameStoreContext>(connString);

var app = builder.Build();

app.MapGamesEndpoints();

app.MapGenresEndpoints();

await app.MigrateDbAsync();

//in async await whatever code you put after the await keyowrd all that code is executed after the awaiting task is completed
//kind of like synchronous, but this is not so in case of ui thread because it does a lot of operations in the back which you dont
//know or you are not writing those like rendering the ui we want the ui thread to be free
//
app.Run();
