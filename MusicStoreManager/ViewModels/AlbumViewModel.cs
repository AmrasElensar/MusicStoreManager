using MusicStoreManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStoreManager.ViewModels
{   /// <summary>
    /// An album with ID, Title, Price, AlbumartUrl, Artist, ArtistID and Genre
    /// </summary>
    public class AlbumViewModel
    {
        /// <summary>
        /// The Id of the album
        /// </summary>
        public int AlbumId { get; set; }
        /// <summary>
        /// The album Title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// The price of the album
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// The location of the albumart
        /// </summary>
        public string AlbumArtUrl { get; set; }
        /// <summary>
        /// The name of the artist
        /// </summary>
        public string Artist { get; set; }
        /// <summary>
        /// The id of the artist
        /// </summary>
        public int ArtistId { get; set; }
        /// <summary>
        /// The genre of the album
        /// </summary>
        public string Genre { get; set; }
    }
}
