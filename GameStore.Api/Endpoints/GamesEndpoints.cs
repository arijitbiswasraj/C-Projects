using Gamestore.Api.Data;
using Gamestore.Api.Mapping;
using GameStore.Api;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;


namespace Gamestore.Api.Endpoints;

public static class GamesEndpoints
{

    const string GetGameEndpointName = "GetGame";

    // private static readonly List<GameSummaryDto> games = [
    //     new (
    //         1,
    //         "Street Fighter II",
    //         "Fighting",
    //         19.99M,
    //         new DateOnly(1992, 7, 15)),
    //     new (2, "Final Fantasy XIV", "Roleplaying", 59.99M, new DateOnly(2010, 9,30)),
    //     new (3, "FIFA 23", "Sports", 69.99M, new DateOnly(2022, 9,27))
    // ];

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games").WithParameterValidation();
        //GET /games
        group.MapGet("/", async (GameStoreContext dbContext) =>
             await dbContext.Games
             .Include(game => game.Genre)
             .Select(game => game.ToGameSummaryDto())
             .AsNoTracking()
             .ToListAsync());
 
        // GET /games/1

        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) => 
        {
            Game? game = await dbContext.Games.FindAsync(id);

            return game is null ? Results.NotFound(): Results.Ok(game.ToGameDetailsDto());

        })
        .WithName(GetGameEndpointName);

        // POST /games

        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) => 
        {
            Game game = newGame.ToEntity();         

            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();
            
            return Results.CreatedAtRoute(GetGameEndpointName, new {id = game.Id}, game.ToGameDetailsDto());

        });


        //PUT /games

        group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) => 
        {
            var existingGame = await dbContext.Games.FindAsync(id);

            if(existingGame is null)
            {
                return Results.NotFound();        
            }
            
            dbContext.Entry(existingGame)
            .CurrentValues
            .SetValues(updatedGame.ToEntity(id));
            //this code above updates the db using entt fmwrk

            await dbContext.SaveChangesAsync();


            return Results.NoContent();

        });

        //DELETE /games/1

        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) => 
        {
            await dbContext.Games.Where(game => game.Id == id)
                .ExecuteDeleteAsync();
            return Results.NoContent();
        });
        return group;
    }
}