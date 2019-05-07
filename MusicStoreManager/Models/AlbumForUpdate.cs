using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStoreManager.Models
{
    public class AlbumForUpdate : AlbumForChange
    {
        [Required(ErrorMessage = "Please submit an albumArt url.")]
        public override string AlbumArtUrl { get => base.AlbumArtUrl; set => base.AlbumArtUrl = value; }

    }
}
