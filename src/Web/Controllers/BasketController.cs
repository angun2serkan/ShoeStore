﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.Models;

namespace Web.Controllers
{
    public class BasketController : Controller
    {
        private readonly IBasketViewModelService _basketViewModelService;
        private readonly IBasketService _basketService;

        public BasketController(IBasketViewModelService basketViewModelService, IBasketService basketService)
        {
            _basketViewModelService = basketViewModelService;
            _basketService = basketService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _basketViewModelService.GetBasketViewModelAsync());
        }

        [HttpPost]
        public async Task<IActionResult> AddToBasket(int productId, int quantity = 1)
        {
            var basket = await _basketViewModelService.AddToBasketAsync(productId, quantity);
            return Json(new NavBasketViewModel() { TotalItemsCount = basket.TotalItemsCount });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([ModelBinder(Name = "quantities")] Dictionary<int, int> quantities)
        {
            await _basketViewModelService.UpdateBasketAsync(quantities);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Empty()
        {
            var basket = await _basketViewModelService.GetOrCreateBasketAsync();
            await _basketService.DeleteBasketAsync(basket.Id);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int basketItemId)
        {
            var basket = await _basketViewModelService.GetOrCreateBasketAsync();
            await _basketService.DeleteBasketItemAsync(basket.Id, basketItemId);

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View();
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel vm)
        {
            if (ModelState.IsValid)
            {
                // payment recevied..

                var address = new Address(vm.Street, vm.City, vm.State, vm.ZipCode, vm.Country);
                var result = await _basketViewModelService.CompleteCheckoutAsync(address);

                return RedirectToAction("OrderComplete", result);
            }

            return View();
        }

        public async Task<IActionResult> OrderComplete(OrderCompleteViewModel vm)
        {
            return View(vm);
        }
    }
}
