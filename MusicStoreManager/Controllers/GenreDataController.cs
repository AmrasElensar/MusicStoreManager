using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MusicStoreManager.Models;
using MusicStoreManager.ViewModels;
using AutoMapper;
using MusicStoreManager.Helpers;

namespace MusicStoreManager.Controllers
{
    [Route("api/albums")]
    public class GenreDataController : Controller
    {
        private readonly IAlbumRepository _albumRepository;
        private readonly IGenreRepository _genreRepository;

        public GenreDataController(IAlbumRepository albumRepository, IGenreRepository genreRepository)
        {
            _albumRepository = albumRepository;
            _genreRepository = genreRepository;
        }

        [HttpGet("genre/{genre?}")]
        public IActionResult GetAlbumsByGenre(string genre, ResourceParams resourceParams)
        {
            IEnumerable<Album> dbalbums;
            List<AlbumViewModel> albums = new List<AlbumViewModel>();
            string currentGenre = string.Empty;

            if (string.IsNullOrEmpty(genre))
            {
                dbalbums = _albumRepository.GetAllAlbums(resourceParams);
                currentGenre = "All albums";
            }
            else
            {
                dbalbums = _albumRepository.GetAllAlbums(resourceParams).Where(a => a.Genre.Name == genre).OrderBy(i => i.AlbumId);
                currentGenre = _genreRepository.Genres.FirstOrDefault(g => g.Name == genre).Name;
            }

            try
            {
                return Ok(Mapper.Map(dbalbums, albums));
            }
            catch (Exception e)
            {
                return BadRequest("Failed to get albums");
            }
        }

        //[HttpGet("genre/{genre?}")]
        //public IActionResult GetAlbumsByGenreBACKUP(string genre)
        //{
        //    IEnumerable<Album> dbalbums;
        //    List<AlbumViewModel> albums = new List<AlbumViewModel>();
        //    string currentGenre = string.Empty;

        //    if (string.IsNullOrEmpty(genre))
        //    {
        //        dbalbums = _albumRepository.GetAllAlbums.OrderBy(a => a.Artist.Name);
        //        currentGenre = "All albums";
        //    }
        //    else
        //    {
        //        dbalbums = _albumRepository.GetAllAlbums.Where(a => a.Genre.Name == genre).OrderBy(i => i.AlbumId);
        //        currentGenre = _genreRepository.Genres.FirstOrDefault(g => g.Name == genre).Name;
        //    }

        //    try
        //    {
        //        return Ok(Mapper.Map(dbalbums, albums));
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest("Failed to get albums");
        //    }
        //}
    }
}
