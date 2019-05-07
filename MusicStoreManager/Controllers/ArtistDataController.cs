using Microsoft.AspNetCore.Mvc;
using MusicStoreManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MusicStoreManager.Services;
using MusicStoreManager.ViewModels;
using Microsoft.AspNetCore.Http;
using MusicStoreManager.Helpers;
using Newtonsoft.Json;
using MusicStoreManager.Entities;

namespace MusicStoreManager.Controllers
{
    [Route("api/artists")]
    public class ArtistDataController : Controller
    {
        private readonly IAlbumRepository _albumRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IArtistRepository _artistRepository;
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;

        public ArtistDataController(IAlbumRepository albumRepository,
            IGenreRepository genreRepository,
            IArtistRepository artistRepository,
            IUrlHelper urlHelper,
            IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService)
        {
            _albumRepository = albumRepository;
            _genreRepository = genreRepository;
            _artistRepository = artistRepository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        [HttpGet(Name = "GetArtists")]
        public IActionResult GetArtists(ResourceParams resourceParams, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<Artist, ArtistDto>
                (resourceParams.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProps<ArtistDto>(resourceParams.Fields))
            {
                return BadRequest();
            }

            var dbArtists = _artistRepository.GetArtists(resourceParams);

            var artists = Mapper.Map<IEnumerable<ArtistDto>>(dbArtists);

            if (mediaType == "application/vnd.musicstore.hateos+json")
            {
                var paginationMetadata = new
                {
                    totalCount = dbArtists.TotalCount,
                    pageSize = dbArtists.PageSize,
                    currentPage = dbArtists.CurrentPage,
                    totalPages = dbArtists.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                var links = CreateLinksForArtists(resourceParams, dbArtists.HasNext, dbArtists.HasPrevious);

                var shapedArtists = artists.ShapeData(resourceParams.Fields);

                var shapedAlbumsWithLinks = shapedArtists.Select(artist =>
                {
                    var artistAsDictionary = artist as IDictionary<string, object>;
                    var albumLinks = CreateLinksForArtist((int)artistAsDictionary["ArtistId"], resourceParams.Fields);

                    artistAsDictionary.Add("links", albumLinks);

                    return artistAsDictionary;

                });

                var linkedCollectionResource = new
                {
                    value = shapedAlbumsWithLinks,
                    links = links
                };

                return Ok(linkedCollectionResource);
            }
            else
            {
                var previousPageLink = dbArtists.HasPrevious ?
                    CreateArtistResourceUri(resourceParams, ResourceUriType.PreviousPage) : null;

                var nextPageLink = dbArtists.HasNext ?
                    CreateArtistResourceUri(resourceParams, ResourceUriType.PreviousPage) : null;

                var paginationMetadata = new
                {
                    previousPageLink = previousPageLink,
                    nextPageLink = nextPageLink,
                    totalCount = dbArtists.TotalCount,
                    pageSize = dbArtists.PageSize,
                    currentPage = dbArtists.CurrentPage,
                    totalPages = dbArtists.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                return Ok(artists.ShapeData(resourceParams.Fields));
            }
        }

        [HttpGet("{artistId}", Name = "GetArtist")]
        public IActionResult GetArtist(int artistId, [FromQuery] string fields, [FromHeader(Name = "Accept")] string mediaType)
        {
            var dbArtist = _artistRepository.GetArtist(artistId);

            if (dbArtist == null)
            {
                return NotFound();
            }

            if (!_typeHelperService.TypeHasProps<ArtistDto>(fields))
            {
                return BadRequest();
            }

            var artist = Mapper.Map<ArtistDto>(dbArtist);

            if (mediaType == "application/vnd.musicstore.hateos+json")
            {
                var links = CreateLinksForArtist(artist.ArtistId, null);

                var linkedResourceToReturn = artist.ShapeData(null) as IDictionary<string, object>;

                linkedResourceToReturn.Add("links", links);

                return CreatedAtRoute("GetArtist", new { artistId = linkedResourceToReturn["ArtistId"] }, linkedResourceToReturn);
            }
            else
            {
                return CreatedAtRoute("GetArtist", new { artistId = artist.ArtistId }, artist);
            }
        }

        [HttpPost(Name = "CreateArtist")]
        public IActionResult CreateArtist([FromBody] ArtistForCreation artist, [FromHeader(Name = "Accept")] string mediaType)
        {

            if (artist == null)
            {
                return BadRequest();
            }

            var dbArtist = Mapper.Map<Artist>(artist);

            _artistRepository.AddArtist(dbArtist);

            if (!_artistRepository.Save())
            {
                throw new Exception("Creating an artist failed on save.");
            }

            var artistToReturn = Mapper.Map<ArtistDto>(dbArtist);

            if (mediaType == "application/vnd.musicstore.hateos+json")
            {
                var links = CreateLinksForArtist(artistToReturn.ArtistId, null);

                var linkedResourceToReturn = artistToReturn.ShapeData(null) as IDictionary<string, object>;

                linkedResourceToReturn.Add("links", links);

                return CreatedAtRoute("GetArtist", new { artistId = linkedResourceToReturn["ArtistId"] }, linkedResourceToReturn);
            }
            else
            {
                return CreatedAtRoute("GetArtist", new { artistId = artistToReturn.ArtistId }, artistToReturn);
            }
        }

        [HttpPost("{id}")]
        public IActionResult BlockAuthorCreation(int id)
        {
            if (_artistRepository.ArtistExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }

        [HttpDelete("{artistId}", Name = "DeleteArtist")]
        public IActionResult DeleteArtist(int id)
        {
            var artistToDelete = _artistRepository.GetArtist(id);

            if (artistToDelete == null)
            {
                return NotFound();
            }

            _artistRepository.DeleteArtist(artistToDelete);

            if (!_artistRepository.Save())
            {
                throw new Exception($"Deleting artist {id} failed on save.");
            }

            return NoContent();
        }

        private string CreateArtistResourceUri(ResourceParams resourceParams, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetArtists",
                        new
                        {
                            fields = resourceParams.Fields,
                            orderBy = resourceParams.OrderBy,
                            searchQuery = resourceParams.SearchQuery,
                            pageNumber = resourceParams.PageNumber - 1,
                            pageSize = resourceParams.PageSize
                        });

                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetArtists",
                        new
                        {
                            fields = resourceParams.Fields,
                            orderBy = resourceParams.OrderBy,
                            searchQuery = resourceParams.SearchQuery,
                            pageNumber = resourceParams.PageNumber + 1,
                            pageSize = resourceParams.PageSize
                        });
                case ResourceUriType.Current:
                default:
                    return _urlHelper.Link("GetArtists",
                        new
                        {
                            fields = resourceParams.Fields,
                            orderBy = resourceParams.OrderBy,
                            searchQuery = resourceParams.SearchQuery,
                            pageNumber = resourceParams.PageNumber,
                            pageSize = resourceParams.PageSize
                        });
            }
        }

        private IEnumerable<Link> CreateLinksForArtist(int id, string fields)
        {
            var links = new List<Link>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new Link(_urlHelper.Link("GetArtist", new { artistId = id }), "self", "GET")
                    );
            }
            else
            {
                links.Add(
                    new Link(_urlHelper.Link("GetArtist", new { artistId = id, fields = fields }), "self", "GET")
                    );
            }

            links.Add(
                new Link(_urlHelper.Link("DeleteArtist", new { artistId = id }), "delete_artist", "DELETE"));

            links.Add(
                new Link(_urlHelper.Link("GetAllAlbumsForArtist", new { artistId = id }), "get_all_albums_for_artist", "GET")
                );

            links.Add(
                new Link(_urlHelper.Link("CreateAlbumForArtist", new { artistId = id }), "create_album_for_artist", "POST")
                );


            return links;
        }

        private IEnumerable<Link> CreateLinksForArtists(ResourceParams resourceParams, bool hasNext, bool hasPrevious)
        {
            var links = new List<Link>();

            links.Add(
                new Link(CreateArtistResourceUri(resourceParams, ResourceUriType.Current), "self", "GET")
            );

            if (hasNext)
            {
                links.Add(
                    new Link(CreateArtistResourceUri(resourceParams, ResourceUriType.NextPage), "nextPage", "GET")
                );
            }

            if (hasPrevious)
            {
                links.Add(
                new Link(CreateArtistResourceUri(resourceParams, ResourceUriType.PreviousPage), "previousPage", "GET")
                    );
            }

            return links;
        }
    }
}
