using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MusicStoreManager.Entities;
using MusicStoreManager.Models;
using MusicStoreManager.Services;
using MusicStoreManager.ViewModels;

namespace MusicStoreManager.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IAlbumRepository _albumRepository;
        private readonly ShoppingCart _shoppingCart;
        // GET: /<controller>/

        public ShoppingCartController(IAlbumRepository albumRepository, ShoppingCart shoppingCart)
        {
            _albumRepository = albumRepository;
            _shoppingCart = shoppingCart;
        }
        public ViewResult Index()
        {
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            var shoppingCartViewModel = new ShoppingCartViewModel
            {
                ShoppingCart = _shoppingCart,
                ShoppingCartTotal = _shoppingCart.GetShoppingCartTotal()

            };

            return View(shoppingCartViewModel);
        }

        public RedirectToActionResult AddToShoppingCart (int id)
        {
            var selectedAlbum = _albumRepository.GetAlbumById(id);
            if (selectedAlbum != null)
            {
                _shoppingCart.AddToCart(selectedAlbum, 1);
            }
            return RedirectToAction("Index");
        }

        public RedirectToActionResult RemoveFromShoppingCart(int id)
        {
            var selectedAlbum = _albumRepository.GetAlbumById(id);
            if(selectedAlbum != null)
            {
                _shoppingCart.RemoveFromCart(selectedAlbum);
            }
            return RedirectToAction("Index");
        }
    }

}
