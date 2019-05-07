using Microsoft.EntityFrameworkCore;
using MusicStoreManager.Entities;
using MusicStoreManager.Helpers;
using MusicStoreManager.Models;
using MusicStoreManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStoreManager.Services
{
    public class ArtistRepository : IArtistRepository
    {
        private readonly mvcMusicStoreContext _mvcMusicStoreContext;
        private readonly IPropertyMappingService _propertyMappingService;

        public ArtistRepository(mvcMusicStoreContext mvcMusicStoreContext, IPropertyMappingService propertyMappingService)
        {
            _mvcMusicStoreContext = mvcMusicStoreContext;
            _propertyMappingService = propertyMappingService;
        }

        public void AddArtist(Artist artist)
        {
            _mvcMusicStoreContext.Artist.Add(artist);
        }

        public bool ArtistExists(int artistId)
        {
            return _mvcMusicStoreContext.Artist.Any(a => a.ArtistId == artistId);
        }

        public void DeleteArtist(Artist artist)
        {
            _mvcMusicStoreContext.Artist.Remove(artist);
        }

        public Artist GetArtist(int artistId)
        {
            return _mvcMusicStoreContext.Artist.FirstOrDefault(a => a.ArtistId == artistId);
        }

        public IEnumerable<Artist> GetArtists()
        {
            return _mvcMusicStoreContext.Artist
                .OrderBy(a => a.Name)
                .ToList();
        }

        public PagedList<Artist> GetArtists(ResourceParams resourceParams)
        {
            var artistsBeforePaging = _mvcMusicStoreContext
                .Artist
                .ApplySort(resourceParams.OrderBy, _propertyMappingService.GetPropertyMapping<Artist, ArtistDto>());

            if (!string.IsNullOrEmpty(resourceParams.SearchQuery))
            {
                var searchQueryParam = resourceParams.SearchQuery.Trim().ToLowerInvariant();
                artistsBeforePaging = artistsBeforePaging.Where(a =>
                a.Name.ToLowerInvariant().Contains(searchQueryParam));
            }

            return PagedList<Artist>.Create(artistsBeforePaging,
                resourceParams.PageNumber,
                resourceParams.PageSize);

        }

        public IEnumerable<Artist> GetArtists(IEnumerable<int> artistIds)
        {
            return _mvcMusicStoreContext.Artist
                .Where(a => artistIds.Contains(a.ArtistId))
                .OrderBy(a => a.Name)
                .ToList();
        }

        public Album GetAlbumForArtist (int artistId, int albumId)
        {
            return _mvcMusicStoreContext.Album
              .Where(b => b.ArtistId == artistId && b.AlbumId == albumId).FirstOrDefault();
        }

        public IEnumerable<Album> GetAlbumsForArtist (int artistId)
        {
            return _mvcMusicStoreContext.Album
                        .Where(b => b.ArtistId == artistId).OrderBy(b => b.Title).ToList();
        }

        public bool Save()
        {
            return (_mvcMusicStoreContext.SaveChanges() >= 0);
        }
    }
}
