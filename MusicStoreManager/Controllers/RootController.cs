using Microsoft.AspNetCore.Mvc;
using MusicStoreManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStoreManager.Controllers
{
    [Route("api")]
    public class RootController : Controller
    {
        private readonly IUrlHelper _urlHelper;

        public RootController(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot([FromHeader(Name = "Accept")] string mediatype)
        {
            if (mediatype == "application/vnd.musicstore.hateos+json")
            {
                var links = new List<Link>();

                links.Add(
                    new Link(_urlHelper.Link("GetRoot", new { }), "self", "GET")
                    );

                links.Add(
                    new Link(_urlHelper.Link("GetAllAlbums", new { }), "all_albums", "GET")
                    );

                links.Add(
                    new Link(_urlHelper.Link("GetArtists", new { }), "all_artists", "GET")
                    );

                links.Add(
                    new Link(_urlHelper.Link("CreateArtist", new { }), "create_artist", "POST")
                    );

                return Ok(links);
            }

            return NoContent();
        }
    }
}
