using Microsoft.EntityFrameworkCore;
using MusicStoreManager.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MusicStoreManager.Services;
using MusicStoreManager.ViewModels;
using MusicStoreManager.Entities;
using MusicStoreManager.Models;

namespace MusicStoreManager.Services
{
    public class AlbumRepository : IAlbumRepository
    {
        private readonly mvcMusicStoreContext _mvcMusicStoreContext;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IArtistRepository _artistRepository;

        public AlbumRepository(mvcMusicStoreContext mvcMusicStoreContext, IPropertyMappingService propertyMappingService, IArtistRepository artistRepository)
        {
            _mvcMusicStoreContext = mvcMusicStoreContext;
            _propertyMappingService = propertyMappingService;
            _artistRepository = artistRepository;
        }

        IEnumerable<Album> IAlbumRepository.AlbumsOfTheWeek
        { get { return _mvcMusicStoreContext.Album.Include(c => c.Genre).Include(a => a.Artist).Where(w => w.IsAlbumOfTheWeek); } }

        public PagedList<Album> GetAllAlbums(ResourceParams resourceParams)
        {
            var albumsBeforePaging = _mvcMusicStoreContext
                .Album
                .Include(a => a.Artist)
                .Include(g => g.Genre)
                .ApplySort(resourceParams.OrderBy, _propertyMappingService.GetPropertyMapping<Album, AlbumDto>());


            if (!string.IsNullOrEmpty(resourceParams.Genre))
            {
                var genreSearchParam = resourceParams.Genre.Trim().ToLowerInvariant();
                albumsBeforePaging = albumsBeforePaging.Where(a => a.Genre.Name.ToLowerInvariant() == genreSearchParam);
            }

            if(!string.IsNullOrEmpty(resourceParams.SearchQuery))
            {
                var searchQueryParam = resourceParams.SearchQuery.Trim().ToLowerInvariant();
                albumsBeforePaging = albumsBeforePaging.Where(a =>
                a.Genre.Name.ToLowerInvariant().Contains(searchQueryParam)
                || a.Artist.Name.ToLowerInvariant().Contains(searchQueryParam)
                || a.Title.ToLowerInvariant().Contains(searchQueryParam));
            }

            return PagedList<Album>.Create(albumsBeforePaging, 
                resourceParams.PageNumber, 
                resourceParams.PageSize);
                
        }

        public Album GetAlbumById(int albumId)
        {
            return _mvcMusicStoreContext.Album.Include(a => a.Artist).Include(g => g.Genre).FirstOrDefault(a => a.AlbumId == albumId);
        }

        public Album GetAlbumForArtist(int artistId, int albumId)
        {
            return _mvcMusicStoreContext.Album.Include(a => a.Artist).Include(g => g.Genre).Where(a => a.ArtistId == artistId && a.AlbumId == albumId).FirstOrDefault();
        }

        public PagedList<Album> GetAlbumsForArtist(int artistId, ResourceParams resourceParams)
        {

            var albumsBeforePaging = _mvcMusicStoreContext
                .Album
                .Include(a => a.Artist)
                .Include(g => g.Genre)
                .Where(a => a.ArtistId == artistId)
                .ApplySort(resourceParams.OrderBy, _propertyMappingService.GetPropertyMapping<Album, AlbumDto>());


            if (!string.IsNullOrEmpty(resourceParams.Genre))
            {
                var genreSearchParam = resourceParams.Genre.Trim().ToLowerInvariant();
                albumsBeforePaging = albumsBeforePaging.Where(a => a.Genre.Name.ToLowerInvariant() == genreSearchParam);
            }

            if (!string.IsNullOrEmpty(resourceParams.SearchQuery))
            {
                var searchQueryParam = resourceParams.SearchQuery.Trim().ToLowerInvariant();
                albumsBeforePaging = albumsBeforePaging.Where(a =>
                a.Genre.Name.ToLowerInvariant().Contains(searchQueryParam)
                || a.Artist.Name.ToLowerInvariant().Contains(searchQueryParam)
                || a.Title.ToLowerInvariant().Contains(searchQueryParam));
            }

            return PagedList<Album>.Create(albumsBeforePaging,
                resourceParams.PageNumber,
                resourceParams.PageSize);

           
        }


        public void AddAlbumForArtist(int artistId, Album album)
        {
            var artist = _artistRepository.GetArtist(artistId);
            if (artist != null)
            {
                artist.Albums.Add(album);
            }
        }

        public void DeleteAlbum(Album album)
        {
            _mvcMusicStoreContext.Album.Remove(album);
        }

        public bool Save()
        {
            return (_mvcMusicStoreContext.SaveChanges() >= 0);
        }

        public void UpdateAlbumForArtist(Album album)
        {
            // no code in this version, update is done by savechanges();
        }

        public void AlbumIdInsertOn()
        {
            _mvcMusicStoreContext.Database.OpenConnection();
            _mvcMusicStoreContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.album ON");
            
        }

        public void AlbumIdInsertOff()
        {
            _mvcMusicStoreContext.Database.OpenConnection();
            _mvcMusicStoreContext.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.album OFF");
            _mvcMusicStoreContext.Database.CloseConnection();
        }

        // not needed
        //public IEnumerable<Album> GetAlbumsByGenre(int genreId)
        //{
        //    return _mvcMusicStoreContext.Album.Include(a => a.Artist).Include(g => g.Genre).Where(g => g.GenreId == genreId);
        //}

        // not needed
        //public IEnumerable<Album> AlbumSearch(string searchString)
        //{
        //    return _mvcMusicStoreContext.Album.Include(a => a.Artist).Include(g => g.Genre).Where(a => a.Title.Contains(searchString) || a.Artist.Name.Contains(searchString));
        //}


    }
}
