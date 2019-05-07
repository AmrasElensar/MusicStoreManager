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
    public class GenreSelector: ViewComponent
    {
        private readonly IGenreRepository _genreRepository;

        public GenreSelector(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public IViewComponentResult Invoke()
        {
            var genreViewModel = new GenreViewModel();
            var genres = _genreRepository.Genres.OrderBy(g => g.Name);


            foreach (var genre in genres)
            {
                var genreItem = new SelectListItem(genre.Name, genre.GenreId.ToString());
                genreViewModel.GenreDropdownList.Add(genreItem);
            }

            return View(genreViewModel);
        }
    }
}
