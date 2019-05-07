using MusicStoreManager.Entities;
using MusicStoreManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStoreManager.ViewModels
{
    public class AlbumListViewModel
    {
        public IEnumerable<Album> Albums { get; set; }
        public string Genre { get; set; }
        public string SearchString { get; set; }
    }
}
