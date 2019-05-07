using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStoreManager.ViewModels
{
    public class GenreViewModel
    {
        public int GenreId { get; set; }
        public List<SelectListItem> GenreDropdownList { get; set; } = new List<SelectListItem>();
    }
}
