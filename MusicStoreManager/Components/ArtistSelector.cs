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
    public class ArtistSelector : ViewComponent
    {
        private readonly IArtistRepository _artistRepository;

        public ArtistSelector(IArtistRepository artistRepository)
        {
            _artistRepository = artistRepository;
        }

        public IViewComponentResult Invoke()
        {
            var artistViewModel = new ArtistViewModel();
            var artists = _artistRepository.GetArtists().OrderBy(a => a.Name);


            foreach (var artist in artists)
            {
                var artistItem = new SelectListItem(artist.Name, artist.ArtistId.ToString());
                artistViewModel.ArtistDropDownList.Add(artistItem);
            }

            return View(artistViewModel);
        }
    }
}
