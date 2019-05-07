using MusicStoreManager.Entities;
using MusicStoreManager.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStoreManager.Services
{
    public interface IAlbumRepository
    {
        PagedList<Album> GetAllAlbums(ResourceParams resourceParams);
        Album GetAlbumById(int albumId);
        //IEnumerable<Album> GetAlbumsByGenre(int genreId);
        //IEnumerable<Album> AlbumSearch(string searchString);
        IEnumerable<Album> AlbumsOfTheWeek { get; }
        void AddAlbumForArtist(int artistId, Album album);
        Album GetAlbumForArtist(int artistId, int albumId);
        PagedList<Album> GetAlbumsForArtist(int artistId, ResourceParams resourceParams);
        void UpdateAlbumForArtist(Album album);
        void DeleteAlbum(Album album);
        void AlbumIdInsertOn();
        void AlbumIdInsertOff();
        bool Save();
    }
}
