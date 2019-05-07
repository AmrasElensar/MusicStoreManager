using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStoreManager.Models
{
    public class ArtistForCreation
    {
        public string Name { get; set; }
        public ICollection<AlbumForCreation> Albums { get; set; } = new List<AlbumForCreation>();

    }
}
