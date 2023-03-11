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

var songs = app.MapGroup("/songs");

songs.MapGet("/", CreateSong);
songs.MapGet("/", GetAllSongs);
songs.MapGet("/{id}", GetSongById);
songs.MapPut("/{id}", UpdateSong);
songs.MapDelete("/{id}", DeleteSong);

#endregion

#region Artists API endpoints

var artists = app.MapGroup("/artists");

artists.MapPost("/", CreateArtist);

#endregion

app.Run();

#region Songs API methods
static async Task<IResult> CreateSong(CreateSongDto data, AppDbContext dbContext)
{
    var artist = dbContext.Artists.FirstOrDefault(x => x.Id == data.ArtistId);
    if (artist is null)
    {
        Dictionary<string, string[]> errors = new()
        {
            { "Artist", new string[] { "Artist not found" } }
        };
        return TypedResults.ValidationProblem(errors: errors);
    }

    var song = new Song()
    {
        Title = data.Title,
        ArtistId = data.ArtistId
    };

    await dbContext.Songs.AddAsync(song);
    await dbContext.SaveChangesAsync();

    return TypedResults.Created($"/songs/{song.Id}", song);
};

static async Task<IResult> GetAllSongs(AppDbContext dbContext)
{
    var songs =  await dbContext.Songs.Select(x => new GetAllSongsDto(x.Id, x.Title, x.ArtistId)).ToListAsync();
    return TypedResults.Ok(songs);
};

static IResult GetSongById(int id, AppDbContext dbContext)
{
    var song = dbContext.Songs.Include(x => x.Artist).FirstOrDefault(x => x.Id == id);

    if (song is null)
        return TypedResults.NotFound();

    return TypedResults.Ok(new GetSongDto(song.Id, song.Title, song.ArtistId, song.Artist?.Name));
};

static async Task<IResult> UpdateSong(int id, UpdateSongDto data,AppDbContext dbContext)
{
    var song = dbContext.Songs.FirstOrDefault(x => x.Id == id);
    if (song is null)
        return TypedResults.NotFound();

    song.Title = data.Title ?? song.Title;
    song.ArtistId = data.ArtistId ?? song.ArtistId;

    dbContext.Songs.Update(song);
    await dbContext.SaveChangesAsync();

    return TypedResults.NoContent();

};

static async Task<IResult> DeleteSong(int id, AppDbContext dbContext)
{
    var song = await dbContext.Songs.FirstOrDefaultAsync(x => x.Id == id);
    if (song is null)
        return TypedResults.NotFound();

    dbContext.Songs.Remove(song);
    await dbContext.SaveChangesAsync();

    return TypedResults.Ok();
};

#endregion

#region Artists API methods
static async Task<IResult> CreateArtist(CreateArtistDto data, AppDbContext dbContext)
{
    var artist = new Artist()
    {
        Name = data.Name
    };

    await dbContext.Artists.AddAsync(artist);
    await dbContext.SaveChangesAsync();

    return TypedResults.Created($"/artists/{artist.Id}", artist);
};

#endregion







