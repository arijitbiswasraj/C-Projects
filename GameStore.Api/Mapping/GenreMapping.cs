using Gamestore.Api.Dtos;
using GameStore.Api.Entities;

namespace Gamestore.Api.Mapping;

public static class GnereMapping
{
    public static GenreDto ToDto(this Genre genre)
    {
        return new GenreDto(genre.Id,genre.Name);
    }
}