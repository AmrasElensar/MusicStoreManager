using MusicStoreManager.Entities;
using MusicStoreManager.Models;
using MusicStoreManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStoreManager.Services
{
    public class GenreRepository : IGenreRepository
    {
        private readonly mvcMusicStoreContext _mvcMusicStoreContext;

        public GenreRepository(mvcMusicStoreContext mvcMusicStoreContext)
        {
            _mvcMusicStoreContext = mvcMusicStoreContext;
        }



        public IEnumerable<Genre> Genres
        { get { return _mvcMusicStoreContext.Genre; } }

        //public Genre GenreById(int id)
        //{
        //    return _mvcMusicStoreContext.Genre.FirstOrDefault(g => g.GenreId == id);
        //}
        //public string GenreDescription(int id)
        //{
        //    string desc = (from genre in _mvcMusicStoreContext.Genre where genre.GenreId == id select new { genre.Description }).ToString();
        //    return desc;

        //}

        public bool GenreExists(int genreId)
        {
            return _mvcMusicStoreContext.Genre.Any(g => g.GenreId == genreId);
        }

        public IEnumerable<Genre> GetAllGenres()
        {
            return _mvcMusicStoreContext.Genre;
        }
    }
}
