using Microsoft.AspNetCore.Mvc;
using MusicStoreManager.Models;
using MusicStoreManager.ViewModels;
using MusicStoreManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MusicStoreManager.Helpers;
using MusicStoreManager.Entities;

namespace MusicStoreManager.Controllers
{
    [Route("api/artistcollections")]
    public class ArtistCollectionController : Controller
    {
        private readonly IArtistRepository _artistRepository;

        public ArtistCollectionController(IArtistRepository artistRepository)
        {
            _artistRepository = artistRepository;
        }
        [HttpPost]
        public IActionResult CreateArtistCollection([FromBody] IEnumerable<ArtistForCreation> artistCollection)
        {

            if (artistCollection == null)
            {
                return BadRequest();
            }

            var dbArtistCollection = Mapper.Map<IEnumerable<Artist>>(artistCollection);

            foreach (var artist in dbArtistCollection)
            {
                _artistRepository.AddArtist(artist);
            }

            if (!_artistRepository.Save())
            {
                throw new Exception("Creating an artist collection failed on save");
            }

            var artistCollectionToReturn = Mapper.Map<IEnumerable<ArtistDto>>(dbArtistCollection);
            var idsString = string.Join(",", artistCollectionToReturn.Select(a => a.ArtistId));

            return CreatedAtRoute("GetArtistCollection", new { ids = idsString }, artistCollectionToReturn);

        }
        [HttpGet("({ids})", Name = "GetArtistCollection")]
        public IActionResult GetArtistCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))]IEnumerable<int> ids)
        {
            if (ids == null)
            {
                return BadRequest();
            }

            var dbArtists = _artistRepository.GetArtists(ids);
            
            if (ids.Count() != dbArtists.Count())
            {
                return NotFound();
            }

            var artistsToReturn = Mapper.Map<IEnumerable<ArtistDto>>(dbArtists);
            return Ok(artistsToReturn);
        }
    }
}

