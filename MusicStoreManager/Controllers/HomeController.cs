using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MusicStoreManager.Models;
using MusicStoreManager.Services;
using MusicStoreManager.ViewModels;


namespace MusicStoreManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAlbumRepository _albumRepository;
        private readonly IGenreRepository _genreRepository;

        public HomeController(IAlbumRepository albumRepository, IGenreRepository genreRepository)
        {
            _albumRepository = albumRepository;
            _genreRepository = genreRepository;
        }

        public ViewResult Index()
        {
            var homeViewModel = new HomeViewModel
            {
                AlbumsOfTheWeek = _albumRepository.AlbumsOfTheWeek
            };

            return View(homeViewModel);
        }
    }
}

