using FoodOrderingWebsite.Data;
using FoodOrderingWebsite.Models;
using FoodOrderingWebsite.Models.Basket;
using FoodOrderingWebsite.Models.Dish;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Mail;

namespace FoodOrderingWebsite.Services
{
    public class BasketService
    {
        private readonly ApplicationDbContext _context;
        public BasketService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<BasketResponseModel> GetAllBaskets()
        {
            List<Basket> baskets = _context.Baskets.ToList();
            List<BasketResponseModel> response = new List<BasketResponseModel>();
            foreach (var basket in baskets)
            {
                response.Add(new BasketResponseModel
                {
                    Id = basket.Id,
                    Name = basket.Name,
                    Amount = basket.Amount,
                    Price = basket.Price,
                    TotalPrice = basket.TotalPrice,
                    Image = basket.Image
                });
            }

            return response;
        }

        public string AddDishToBasket(Guid userId, Guid DishId)
        {
            Basket basket = _context.Baskets.FirstOrDefault(b => b.UserId == userId && b.DishId == DishId);
            if (basket != null)
            {
                basket.Amount += 1;
                basket.TotalPrice = basket.Amount * basket.Price;
                _context.Baskets.Update(basket);
                _context.SaveChanges();
                return "ok";
            }
            else
            {
                Dish currentDish = _context.Dishes.FirstOrDefault(d => d.Id == DishId);
                if (currentDish != null)
                {
                    basket = new Basket
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        OrderId = Guid.Empty,
                        DishId = DishId,
                        Name = currentDish.Name,
                        Amount = 1,
                        Price = currentDish.Price,
                        TotalPrice = currentDish.Price,
                        Image = currentDish.Image
                    };
                    _context.Baskets.Add(basket);
                    _context.SaveChanges();
                    return "ok";
                }
                return "notfound";
            }
        }

        public string DeleteDishFromBasket(Guid userId, Guid DishId)
        {
            Basket basket = _context.Baskets.FirstOrDefault(b => b.UserId == userId && b.DishId == DishId);
            if (basket != null)
            {
                basket.Amount -= 1;
                basket.TotalPrice = basket.Amount * basket.Price;
                _context.Baskets.Update(basket);

                if (basket.Amount == 0)
                {
                    _context.Baskets.Remove(basket);
                }
                _context.SaveChanges();
                return "ok";
            }
            else
            {
                return "notfound";
            }
        }
    }
}
