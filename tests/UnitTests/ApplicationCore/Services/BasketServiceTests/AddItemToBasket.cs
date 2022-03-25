using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Services;
using Ardalis.Specification;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.ApplicationCore.Services.BasketServiceTests
{
    public class AddItemToBasket
    {
        private readonly Mock<IRepository<Basket>> _mockBasketRepo = new();
        private readonly Mock<IRepository<BasketItem>> _mockBasketItemRepo = new();
        private readonly Mock<IRepository<Product>> _mockProductRepo = new();
        [Fact]
        public async Task MustThrowErrorWithNonPositiveQuantity()
        {
            var basket = new Basket();
            _mockBasketRepo.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Basket>>())).ReturnsAsync(basket);
            var basketId = 1;
            var productId = 1;
            var quantity = -3;
            var basketService = new BasketService(_mockBasketRepo.Object, _mockProductRepo.Object,_mockBasketItemRepo.Object);
            await Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                await basketService.AddItemToBasketAsync(basketId,productId,quantity);
            });

        }
    }
}
