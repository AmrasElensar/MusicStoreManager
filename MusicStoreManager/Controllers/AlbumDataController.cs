using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MusicStoreManager.Models;
using MusicStoreManager.ViewModels;
using AutoMapper;
using MusicStoreManager.Helpers;
using Newtonsoft.Json;
using MusicStoreManager.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using MusicStoreManager.Entities;
using Microsoft.AspNetCore.Authorization;

namespace MusicStoreManager.Controllers
{
    [Route("api/albums")]
    public class AlbumDataController : Controller
    {
        private readonly IAlbumRepository _albumRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IArtistRepository _artistRepository;
        private readonly ILogger<AlbumDataController> _ilogger;
        private readonly ITypeHelperService _typeHelperService;

        public AlbumDataController(IAlbumRepository albumRepository, IGenreRepository genreRepository,
            IUrlHelper urlHelper, IPropertyMappingService propertyMappingService,
            IArtistRepository artistRepository,
            ILogger<AlbumDataController> ilogger,
            ITypeHelperService typeHelperService)
        {
            _albumRepository = albumRepository;
            _genreRepository = genreRepository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _artistRepository = artistRepository;
            _ilogger = ilogger;
            _typeHelperService = typeHelperService;
        }

        [HttpGet(Name = "GetAllAlbums")]
        public IActionResult GetAllAlbums(ResourceParams resourceParams, [FromHeader(Name = "Accept")] string mediaType)
        {

            if (!_propertyMappingService.ValidMappingExistsFor<Album, AlbumDto>
                (resourceParams.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProps<AlbumDto>(resourceParams.Fields))
            {
                return BadRequest();
            }

            var dbAlbums = _albumRepository.GetAllAlbums(resourceParams);
            
            var albums = Mapper.Map<IEnumerable<AlbumDto>>(dbAlbums);

            if (mediaType == "application/vnd.musicstore.hateos+json")
            {
                var paginationMetadata = new
                {
                    totalCount = dbAlbums.TotalCount,
                    pageSize = dbAlbums.PageSize,
                    currentPage = dbAlbums.CurrentPage,
                    totalPages = dbAlbums.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

                var links = CreateLinksForAlbums(resourceParams, dbAlbums.HasNext, dbAlbums.HasPrevious);

                var shapedAlbums = albums.ShapeData(resourceParams.Fields);

                var shapedAlbumsWithLinks = shapedAlbums.Select(album =>
                {
                    var albumAsDictionary = album as IDictionary<string, object>;
                    var albumLinks = CreateLinksForAlbum((int)albumAsDictionary["AlbumId"], (int)albumAsDictionary["ArtistId"], resourceParams.Fields);

                    albumAsDictionary.Add("links", albumLinks);

                    return albumAsDictionary;

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
                var previousPageLink = dbAlbums.HasPrevious ?
                    CreateAlbumsResourceUri(resourceParams, ResourceUriType.PreviousPage) : null;

                var nextPageLink = dbAlbums.HasNext ?
                    CreateAlbumsResourceUri(resourceParams, ResourceUriType.NextPage) : null;

                var paginationMetadata = new
                {
                    previousPageLink = previousPageLink,
                    nextPageLink = nextPageLink,
                    totalCount = dbAlbums.TotalCount,
                    pageSize = dbAlbums.PageSize,
                    currentPage = dbAlbums.CurrentPage,
                    totalPages = dbAlbums.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

                return Ok(albums.ShapeData(resourceParams.Fields));
            }
            
        }

        [HttpGet("artist/{artistId}", Name = "GetAllAlbumsForArtist")]
        public IActionResult GetAllAlbumsForArtist(int artistId, ResourceParams resourceParams, [FromHeader(Name = "Accept")] string mediaType)
        {

            if (!_propertyMappingService.ValidMappingExistsFor<Album, AlbumDto>
                (resourceParams.OrderBy))
            {
                return BadRequest();
            }

            if (!_typeHelperService.TypeHasProps<AlbumDto>(resourceParams.Fields))
            {
                return BadRequest();
            }

            var dbAlbums = _albumRepository.GetAlbumsForArtist(artistId, resourceParams);

            var albums = Mapper.Map<IEnumerable<AlbumDto>>(dbAlbums);

            if (mediaType == "application/vnd.musicstore.hateos+json")
            {
                var paginationMetadata = new
                {
                    totalCount = dbAlbums.TotalCount,
                    pageSize = dbAlbums.PageSize,
                    currentPage = dbAlbums.CurrentPage,
                    totalPages = dbAlbums.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

                var links = CreateLinksForAlbumsForArtist(resourceParams, dbAlbums.HasNext, dbAlbums.HasPrevious);

                var shapedAlbums = albums.ShapeData(resourceParams.Fields);

                var shapedAlbumsWithLinks = shapedAlbums.Select(album =>
                {
                    var albumAsDictionary = album as IDictionary<string, object>;
                    var albumLinks = CreateLinksForAlbum((int)albumAsDictionary["AlbumId"], artistId, resourceParams.Fields);

                    albumAsDictionary.Add("links", albumLinks);

                    return albumAsDictionary;

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
                var previousPageLink = dbAlbums.HasPrevious ?
                    CreateAlbumsForArtistResourceUri(resourceParams, ResourceUriType.PreviousPage) : null;

                var nextPageLink = dbAlbums.HasNext ?
                    CreateAlbumsForArtistResourceUri(resourceParams, ResourceUriType.PreviousPage) : null;

                var paginationMetadata = new
                {
                    previousPageLink = previousPageLink,
                    nextPageLink = nextPageLink,
                    totalCount = dbAlbums.TotalCount,
                    pageSize = dbAlbums.PageSize,
                    currentPage = dbAlbums.CurrentPage,
                    totalPages = dbAlbums.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

                return Ok(albums.ShapeData(resourceParams.Fields));
            }
        }

        [HttpGet("artist/{artistId}/{id}", Name = "GetAlbum")]
        public IActionResult GetAlbumByIdForArtist(int id, int artistId, [FromQuery] string fields, [FromHeader(Name = "Accept")] string mediaType)
        {
            var dbalbum = _albumRepository.GetAlbumForArtist(artistId, id);
            if (dbalbum == null)
            {
                return NotFound();
            }

            if (!_typeHelperService.TypeHasProps<AlbumDto>(fields))
            {
                return BadRequest();
            }

            var album = Mapper.Map<AlbumDto>(dbalbum);

            if (mediaType == "application/vnd.musicstore.hateos+json")
            {
                var links = CreateLinksForAlbum(id, artistId, fields);

                var linkedResourceToReturn = album.ShapeData(fields) as IDictionary<string, object>;

                linkedResourceToReturn.Add("links", links);

                return Ok(linkedResourceToReturn);
            }
            else
            {
                return Ok(album.ShapeData(fields));
            }
            
        }

        [HttpPost("artist/{artistId}", Name = "CreateAlbumForArtist")]
        public IActionResult CreateAlbumForArtist(int artistId, [FromBody]AlbumForCreation album, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (album == null)
            {
                return BadRequest();
            }

            if (!_genreRepository.GenreExists(album.GenreId))
            {
                ModelState.AddModelError(nameof(AlbumForCreation), "You should submit a valid Genre Id.");
            }

            if (album.Price == 0)
            {
                ModelState.AddModelError(nameof(AlbumForUpdate), "You should submit a price.");
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (!_artistRepository.ArtistExists(artistId))
            {
                return NotFound();
            }

            var dbAlbum = Mapper.Map<Album>(album);

            _albumRepository.AddAlbumForArtist(artistId, dbAlbum);

            if (!_albumRepository.Save())
            {
                throw new Exception($"Creating an album for artist {artistId} failed on save.");
            }


            var albumToReturn = Mapper.Map<AlbumDto>(dbAlbum);

            if (mediaType == "application/vnd.musicstore.hateos+json")
            {
                var links = CreateLinksForAlbum(albumToReturn.AlbumId, artistId, null);

                var linkedResourceToReturn = albumToReturn.ShapeData(null) as IDictionary<string, object>;

                linkedResourceToReturn.Add("links", links);

                return CreatedAtRoute("GetAlbum", new { id = linkedResourceToReturn["AlbumId"] }, linkedResourceToReturn);
            }
            else
            {
                return CreatedAtRoute("GetAlbum", new { id = albumToReturn.AlbumId }, albumToReturn);
            }
                
        }

        [HttpDelete("artist/{artistId}/{id}", Name ="DeleteAlbum")]
        public IActionResult DeleteAlbum(int artistId, int id)
        {
            if (!_artistRepository.ArtistExists(artistId))
            {
                return NotFound();
            }

            var albumForDeletion = _albumRepository.GetAlbumForArtist(artistId, id);
            if (albumForDeletion == null)
            {
                return NotFound();
            }

            _albumRepository.DeleteAlbum(albumForDeletion);

            if (!_albumRepository.Save())
            {
                throw new Exception($"Deleting album {id}, for artist {artistId} failed on save.");
            }

            _ilogger.LogInformation(100, $"Album {id} for artist {artistId} was deleted");

            return NoContent();
        }

        [HttpPut("artist/{artistId}/{id}", Name ="UpdateAlbum")]
        //[Authorize(Roles = "admin")]
        public IActionResult UpdateAlbumForArtist(int artistId, int id, [FromBody] AlbumForUpdate album, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (album == null)
            {
                return BadRequest();
            }

            if (!_genreRepository.GenreExists(album.GenreId))
            {
                ModelState.AddModelError(nameof(AlbumForUpdate), "You should submit a valid Genre Id.");
            }

            if (album.Price == 0)
            {
                ModelState.AddModelError(nameof(AlbumForUpdate), "You should submit a price.");
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (!_artistRepository.ArtistExists(artistId))
            {
                return NotFound();
            }

            var albumForUpdate = _albumRepository.GetAlbumForArtist(artistId, id);
            if (albumForUpdate == null)
            {
                var albumToAdd = Mapper.Map<Album>(album);
                albumToAdd.AlbumId = id;

                _albumRepository.AddAlbumForArtist(artistId, albumToAdd);
                _albumRepository.AlbumIdInsertOn();

                if (!_albumRepository.Save())
                {
                    throw new Exception($"Upserting album {id} for artist {artistId} failed on save.");
                }

                _albumRepository.AlbumIdInsertOff();

                var albumToReturn = Mapper.Map<AlbumDto>(albumToAdd);

                if (mediaType == "application/vnd.musicstore.hateos+json")
                {
                    var links = CreateLinksForAlbum(albumToReturn.AlbumId, artistId, null);

                    var linkedResourceToReturn = albumToReturn.ShapeData(null) as IDictionary<string, object>;

                    linkedResourceToReturn.Add("links", links);

                    return CreatedAtRoute("GetAlbum", new { id = linkedResourceToReturn["AlbumId"] }, linkedResourceToReturn);
                }
                else
                {
                    return CreatedAtRoute("GetAlbum", new { id = albumToReturn.AlbumId }, albumToReturn);
                }
            }

            Mapper.Map(album, albumForUpdate);
            _albumRepository.UpdateAlbumForArtist(albumForUpdate);

            if (!_albumRepository.Save())
            {
                throw new Exception($"Updating album {id} for artist {artistId} failed on save.");
            }

            return NoContent();
        }

        [HttpPatch("artist/{artistId}/{id}", Name = "PartiallyUpdateAlbum")]
        public IActionResult PartiallyUpdateAlbumForArtist(int artistId, int id, [FromBody] JsonPatchDocument<AlbumForUpdate> patchdoc)
        {
            if (patchdoc == null)
            {
                return BadRequest();
            }

            if (!_artistRepository.ArtistExists(artistId))
            {
                return NotFound();
            }

            var albumForPartialUpdate = _albumRepository.GetAlbumForArtist(artistId, id);

            if (albumForPartialUpdate == null)
            {
                return NotFound();

                // upsert does not work with patch due to FK constraints;

            }

            var albumToPatch = Mapper.Map<AlbumForUpdate>(albumForPartialUpdate);

            patchdoc.ApplyTo(albumToPatch, ModelState);

            if (!_genreRepository.GenreExists(albumToPatch.GenreId))
            {
                ModelState.AddModelError(nameof(AlbumForUpdate), "You should submit a valid Genre Id.");
            }

            if (albumToPatch.Price == 0)
            {
                ModelState.AddModelError(nameof(AlbumForUpdate), "You should submit a price.");
            }

            TryValidateModel(albumToPatch);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            Mapper.Map(albumToPatch, albumForPartialUpdate);

            _albumRepository.UpdateAlbumForArtist(albumForPartialUpdate);

            if (!_albumRepository.Save())
            {
                throw new Exception($"Patching album {id} for artist {artistId} failed on save.");
            }

            return NoContent();

        }

        private string CreateAlbumsResourceUri(ResourceParams resourceParams, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetAllAlbums",
                        new
                        {
                            fields = resourceParams.Fields,
                            orderBy = resourceParams.OrderBy,
                            searchQuery = resourceParams.SearchQuery,
                            genre = resourceParams.Genre,
                            pageNumber = resourceParams.PageNumber - 1,
                            pageSize = resourceParams.PageSize
                        });

                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetAllAlbums",
                        new
                        {
                            fields = resourceParams.Fields,
                            orderBy = resourceParams.OrderBy,
                            searchQuery = resourceParams.SearchQuery,
                            genre = resourceParams.Genre,
                            pageNumber = resourceParams.PageNumber + 1,
                            pageSize = resourceParams.PageSize
                        });
                case ResourceUriType.Current:
                default:
                    return _urlHelper.Link("GetAllAlbums",
                        new
                        {
                            fields = resourceParams.Fields,
                            orderBy = resourceParams.OrderBy,
                            searchQuery = resourceParams.SearchQuery,
                            genre = resourceParams.Genre,
                            pageNumber = resourceParams.PageNumber,
                            pageSize = resourceParams.PageSize
                        });
            }
        }

        private string CreateAlbumsForArtistResourceUri(ResourceParams resourceParams, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetAllAlbumsForArtist",
                        new
                        {
                            fields = resourceParams.Fields,
                            orderBy = resourceParams.OrderBy,
                            searchQuery = resourceParams.SearchQuery,
                            genre = resourceParams.Genre,
                            pageNumber = resourceParams.PageNumber - 1,
                            pageSize = resourceParams.PageSize
                        });

                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetAllAlbumsForArtist",
                        new
                        {
                            fields = resourceParams.Fields,
                            orderBy = resourceParams.OrderBy,
                            searchQuery = resourceParams.SearchQuery,
                            genre = resourceParams.Genre,
                            pageNumber = resourceParams.PageNumber + 1,
                            pageSize = resourceParams.PageSize
                        });

                default:
                    return _urlHelper.Link("GetAllAlbumsForArtist",
                        new
                        {
                            fields = resourceParams.Fields,
                            orderBy = resourceParams.OrderBy,
                            searchQuery = resourceParams.SearchQuery,
                            genre = resourceParams.Genre,
                            pageNumber = resourceParams.PageNumber,
                            pageSize = resourceParams.PageSize
                        });
            }
        }

        private IEnumerable<Link> CreateLinksForAlbums(ResourceParams resourceParams, bool hasNext, bool hasPrevious)
        {
            var links = new List<Link>();

            links.Add(
                new Link(CreateAlbumsResourceUri(resourceParams, ResourceUriType.Current), "self", "GET")
            );

            if (hasNext)
            {
                links.Add(
                    new Link(CreateAlbumsResourceUri(resourceParams, ResourceUriType.NextPage), "nextPage","GET")
                );
            }

            if (hasPrevious)
            {
                links.Add(
                new Link(CreateAlbumsResourceUri(resourceParams, ResourceUriType.PreviousPage), "previousPage", "GET")
                    );
            }

            return links;
        }

        private IEnumerable<Link> CreateLinksForAlbumsForArtist(ResourceParams resourceParams, bool hasNext, bool hasPrevious)
        {
            var links = new List<Link>();

            links.Add(
                new Link(CreateAlbumsForArtistResourceUri(resourceParams, ResourceUriType.Current), "self", "GET")
            );

            if (hasNext)
            {
                links.Add(
                    new Link(CreateAlbumsForArtistResourceUri(resourceParams, ResourceUriType.NextPage), "nextPage", "GET")
                );
            }

            if (hasPrevious)
            {
                links.Add(
                new Link(CreateAlbumsForArtistResourceUri(resourceParams, ResourceUriType.PreviousPage), "previousPage", "GET")
                    );
            }

            return links;
        }

        private IEnumerable<Link> CreateLinksForAlbum(int albumId, int artistId, string fields)
        {
            var links = new List<Link>();

            if(string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new Link(_urlHelper.Link("GetAlbum", new { id = albumId, artistId = artistId }), "self", "GET"));
            }
            else
            {
                links.Add(
                    new Link(_urlHelper.Link("GetAlbum", new { id = albumId, artistId = artistId, fields = fields }), "self", "GET"));
            }

            links.Add(
                new Link(_urlHelper.Link("DeleteAlbum", new { id = albumId, artistId = artistId }), "delete_album", "DELETE"));

            links.Add(
                new Link(_urlHelper.Link("UpdateAlbum", new { id = albumId, artistId = artistId }), "update_album", "PUT"));

            links.Add(
                new Link(_urlHelper.Link("PartiallyUpdateAlbum", new { id = albumId, artistId = artistId }), "partially_update_album", "PATCH"));

            return links;
        }
    }
}
