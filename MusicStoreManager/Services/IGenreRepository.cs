using MusicStoreManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStoreManager.Services
{
    public interface IGenreRepository
    {
        //IEnumerable<Genre> GetAllGenres();
        //Genre GenreById(int id);
        IEnumerable<Genre> Genres { get; }
        bool GenreExists(int genreId);
    }
}
