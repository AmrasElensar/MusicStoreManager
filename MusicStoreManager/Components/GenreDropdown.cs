using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MusicStoreManager.Services;
using MusicStoreManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStoreManager.Components
{
    public class GenreDropDown : ViewComponent
    {
        private readonly IGenreRepository _genreRepository;

        public GenreDropDown(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public IViewComponentResult Invoke()
        {
            var genres = _genreRepository.Genres.OrderBy(g => g.Name);
            return View(genres);
        }
    }
}
