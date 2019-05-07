using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStoreManager.Entities;
using MusicStoreManager.Models;
using MusicStoreManager.Services;

namespace MusicStoreManager.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ShoppingCart _shoppingCart;

        public OrderController(IOrderRepository orderRepository, ShoppingCart shoppingCart)
        {
            _orderRepository = orderRepository;
            _shoppingCart = shoppingCart;
        }

        // GET: /<controller>/
        [Authorize]
        public IActionResult Checkout()
        {
            var order = new Order();
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;
            foreach (var shoppingCartItem in items)
            {
                var orderDetail = new OrderDetail()
                {
                    Quantity = shoppingCartItem.Amount,
                    AlbumId = shoppingCartItem.Album.AlbumId,
                    OrderId = order.OrderId,
                    UnitPrice = shoppingCartItem.Album.Price,
                    Album = shoppingCartItem.Album
                    
                };
                order.OrderDetail.Add(orderDetail);
            }
            order.Total = _shoppingCart.GetShoppingCartTotal();

            return View(order);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Checkout(Order order)
        {
            var items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            if (_shoppingCart.ShoppingCartItems.Count == 0)
            {
                ModelState.AddModelError("", "Your cart is empty, do some shopping first...");
            }

            if (ModelState.IsValid)
            {
                _orderRepository.CreateOrder(order);
                _shoppingCart.ClearCart();
                return RedirectToAction("CheckoutComplete");
            }
            return View(order);
        }

        public IActionResult CheckoutComplete()
        {
            ViewBag.CheckoutCompleteMessage = "Thank you for your order, we'll ship your albums as soon as possible!";
            return View();
        }
    }
}
