using System.Collections.Generic;
using MusicStoreManager.Entities;
using MusicStoreManager.Helpers;
using MusicStoreManager.Models;

namespace MusicStoreManager.Services
{
    public interface IArtistRepository
    {
        void AddArtist(Artist artist);
        bool ArtistExists(int artistId);
        void DeleteArtist(Artist artist);
        Album GetAlbumForArtist(int artistId, int albumId);
        IEnumerable<Album> GetAlbumsForArtist(int artistId);
        Artist GetArtist(int artistId);
        PagedList<Artist> GetArtists(ResourceParams resourceParams);
        IEnumerable<Artist> GetArtists();
        IEnumerable<Artist> GetArtists(IEnumerable<int> artistIds);
        bool Save();
    }
}