using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStoreManager.Models
{
    public abstract class AlbumForChange
    {
        [Required(ErrorMessage = "You should select a genre.")]
        public int GenreId { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage = "The title should not have more than 100 characters")]
        public string Title { get; set; }
        public decimal Price { get; set; }
        public virtual string AlbumArtUrl { get; set; }
        public bool IsAlbumOfTheWeek { get; set; }
    }
}
