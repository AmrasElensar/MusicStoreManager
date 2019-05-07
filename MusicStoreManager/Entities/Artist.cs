using System;
using System.Collections.Generic;

namespace MusicStoreManager.Entities
{
    public partial class Artist
    {
        public Artist()
        {
            Albums = new HashSet<Album>();
        }

        public int ArtistId { get; set; }
        public string Name { get; set; }

        public ICollection<Album> Albums { get; set; }
    }
}
