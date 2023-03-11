namespace Playlist.Models
{
    public class Song
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int ArtistId { get; set; }
        public virtual Artist Artist { get; set; }
    }
}
