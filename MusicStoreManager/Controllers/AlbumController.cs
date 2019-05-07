using Microsoft.AspNetCore.Mvc;
using MusicStoreManager.Models;
using MusicStoreManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using MusicStoreManager.Services;
using MusicStoreManager.Entities;
using MusicStoreManager.ApiClient;
using AutoMapper;
using Marvin.StreamExtensions;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace MusicStoreManager.Controllers
{
    public class AlbumController : Controller
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly Microsoft.AspNetCore.Identity.UserManager<IdentityUser> _userManager;
        private readonly IAlbumRepository _albumRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly MusicStoreApiClient _httpClient;

        public AlbumController(IHttpContextAccessor httpContext, UserManager<IdentityUser> userManager, IAlbumRepository albumRepository, IGenreRepository genreRepository, MusicStoreApiClient httpClient)
        {
            _httpContext = httpContext;
            _userManager = userManager;
            _albumRepository = albumRepository;
            _genreRepository = genreRepository;
            _httpClient = httpClient;
        }


        public async Task<ActionResult> Genre(int? pageNumber, int? pageSize, string genre, string searchQuery, string orderBy, string fields, string accept)
        {
            if (genre == "Sexy Music")
            { return RedirectToAction("SexyMusic"); }
            else
            {
                var dbalbums = await _httpClient.GetAllAlbumsAsync(pageNumber, pageSize, genre, searchQuery, orderBy, fields, accept);

                var albums = Mapper.Map<IEnumerable<AlbumViewModel>>(dbalbums);

                return View(albums);
            }

        }
        public IActionResult List()
        {
            return View();
        }

        public async Task<IActionResult> UpdateAlbum(int id, int artistId)
        {
            var dbalbum = await _httpClient.GetAlbumAsync(id, artistId);
            var album = Mapper.Map<AlbumViewModel>(dbalbum);
            return View(album);
        }

        public IActionResult SexyMusic()
        {
            return View();
        }
       
        public async Task<ActionResult> Details(int id, int artistId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(_httpContext.HttpContext.User);

                ViewBag.IsAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            }

            var dbalbum = await _httpClient.GetAlbumAsync(id, artistId);

            var album = Mapper.Map<AlbumViewModel>(dbalbum);

            return View(album);
        }
    }
}
