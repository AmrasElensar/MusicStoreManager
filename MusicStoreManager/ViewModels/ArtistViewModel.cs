using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStoreManager.ViewModels
{
    public class ArtistViewModel
    {
        public int ArtistId { get; set; }
        public string Name { get; set; }
        public List<SelectListItem> ArtistDropDownList { get; set; } = new List<SelectListItem>();
    }
}
