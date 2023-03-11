using Microsoft.EntityFrameworkCore;
using Playlist.Models;
using Playlist.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("PlaylistDb"));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


#region Songs API endpoints

app.MapPost("/songs", async (CreateSongDto data, AppDbContext dbContext) =>
{
    var artist = dbContext.Artists.FirstOrDefault(x => x.Id == data.ArtistId);
    if (artist is null)
    {
        Dictionary<string, string[]> errors = new()
        {
            { "Artist", new string[] { "Artist not found" } }
        };
        return Results.ValidationProblem(errors: errors);
    }

    var song = new Song()
    {
        Title = data.Title,
        ArtistId = data.ArtistId
    };

    await dbContext.Songs.AddAsync(song);
    await dbContext.SaveChangesAsync();

    return Results.Created($"/songs/{song.Id}", song);
});

app.MapGet("/songs", async (AppDbContext dbContext) =>
{
    return await dbContext.Songs.Select(x => new GetAllSongsDto(x.Id, x.Title, x.ArtistId)).ToListAsync();
});

app.MapGet("/songs/{id}", (int id, AppDbContext dbContext) =>
{
    var song = dbContext.Songs.Include(x => x.Artist).FirstOrDefault(x => x.Id == id);

    if (song is null)
        return Results.NotFound();

    return Results.Ok(new GetSongDto(song.Id, song.Title, song.ArtistId, song.Artist?.Name));
});

app.MapPut("/songs/{id}", async (int id, UpdateSongDto data,AppDbContext dbContext) =>
{
    var song = dbContext.Songs.FirstOrDefault(x => x.Id == id);
    if (song is null)
        return Results.NotFound();

    song.Title = data.Title ?? song.Title;
    song.ArtistId = data.ArtistId ?? song.ArtistId;

    dbContext.Songs.Update(song);
    await dbContext.SaveChangesAsync();

    return Results.NoContent();

});

app.MapDelete("/songs/{id}", async (int id, AppDbContext dbContext) =>
{
    var song = await dbContext.Songs.FirstOrDefaultAsync(x => x.Id == id);
    if (song is null)
        return Results.NotFound();

    dbContext.Songs.Remove(song);
    await dbContext.SaveChangesAsync();

    return Results.Ok();
});

#endregion


#region Artists API endpoints

app.MapPost("/artists", async (CreateArtistDto data, AppDbContext dbContext) =>
{
    var artist = new Artist()
    {
        Name = data.Name
    };

    await dbContext.Artists.AddAsync(artist);
    await dbContext.SaveChangesAsync();

    return Results.Created($"/artists/{artist.Id}", artist);
});


#endregion



app.Run();



