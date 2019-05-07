using MusicStoreManager.Entities;
using MusicStoreManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStoreManager.ViewModels
{
    public class HomeViewModel
    {

        public IEnumerable<Album> AlbumsOfTheWeek { get; set; }
        public string SearchString { get; set; }
    }
}
