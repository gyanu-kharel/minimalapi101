namespace Playlist.ViewModels
{
    public record CreateSongDto(string Title, int ArtistId);
    public record GetAllSongsDto(int Id, string? Title, int ArtistId);
    public record GetSongDto(int Id, string? Title, int ArtistId, string? Artist);
    public record UpdateSongDto(string? Title, int? ArtistId);
}
