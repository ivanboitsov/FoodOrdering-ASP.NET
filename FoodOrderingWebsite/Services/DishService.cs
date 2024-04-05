using FoodOrderingWebsite.Data;
using FoodOrderingWebsite.Models;
using FoodOrderingWebsite.Models.Dish;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Security.Claims;
using System.Text.RegularExpressions;
using static FoodOrderingWebsite.Services.DishService;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FoodOrderingWebsite.Services
{
    
    public class DishService
    {
        private readonly ApplicationDbContext _context;
        public DishService(ApplicationDbContext context)
        {
            _context = context;
        }

        public class PaginationInfo
        {
            public int Size { get; set; }
            public int Count { get; set; }
            public int Current { get; set; }
        }

        public class DishListResponse
        {
            public List<Dish> Dishes { get; set; }
            public PaginationInfo Pagination { get; set; }
        }

        public Dish GetDishById(string id)
        {
            return _context.Dishes.FirstOrDefault(d => d.Id.ToString() == id);
        }

        public string GetDishRaiting(string id)
        {
            Dish dish = GetDishById(id);
            if (dish != null)
            {
                return GetDishById(id).Rating.ToString();
            }
            return null;
        }

        public bool IsUserAbleToPostRaiting(Dish dish, Guid userId)
        {
            UserTestedDish tested = _context.TestedDishes.FirstOrDefault(d => d.UserId == userId && d.DishId == dish.Id);
            if (tested != null)
            {
                return true;
            }
            return false;
        }

        public void PostDishRaiting(Dish dish, Guid userId, int raiting)
        {
            Raiting ratiting = new Raiting
            {
                Id = Guid.NewGuid(),
                DishId = dish.Id,
                UserId = userId,
                Rating = raiting,
            };

            _context.Raitings.Add(ratiting);
            _context.SaveChanges();
        }

        public void UpdateDishRaiting(Dish dish)
        {
            List<Raiting> raitings = _context.Raitings.Where(di => di.DishId == dish.Id).ToList();
            int raitingCount = raitings.Count();
            int summRaiting = 0;
            for (int i = 0; i < raitingCount; i++)
            {
                summRaiting += raitings[i].Rating;
            }
            dish.Rating = (double)summRaiting / (double)raitingCount;
            _context.Dishes.Update(dish);
            _context.SaveChanges();
        }

        public DishListResponse GetAllDishes(
        string category = "--",
        string vegetarian = "--",
        string sorting = "--",
        int page = 1)
        {
            Console.WriteLine(category); Console.WriteLine(vegetarian); Console.WriteLine(sorting); Console.WriteLine(page);
            System.Linq.IQueryable<FoodOrderingWebsite.Models.Dish.Dish> filteredDishes = _context.Dishes;

            if (category != "All")
            {
                filteredDishes = filteredDishes.Where(dish => dish.Category == category);
            }

            if (vegetarian != "NoMatter")
            {
                bool isVegetarian = bool.Parse(vegetarian);
                filteredDishes = filteredDishes.Where(dish => dish.Vegetarian == isVegetarian);
            }

            if (sorting != "--")
            {
                switch (sorting)
                {
                    case "NameAsc":
                        filteredDishes = filteredDishes.OrderBy(d => d.Name);
                        break;
                    case "NameDesc":
                        filteredDishes = filteredDishes.OrderByDescending(d => d.Name);
                        break;
                    case "PriceAsc":
                        filteredDishes = filteredDishes.OrderBy(d => d.Price);
                        break;
                    case "PriceDesc":
                        filteredDishes = filteredDishes.OrderByDescending(d => d.Price);
                        break;
                    case "RatingAsc":
                        filteredDishes = filteredDishes.OrderBy(d => d.Rating);
                        break;
                    case "RatingDesc":
                        filteredDishes = filteredDishes.OrderByDescending(d => d.Rating);
                        break;
                }
            }

            var pageSize = 10;
            int Totalpages = filteredDishes.Count() / pageSize;
            if (filteredDishes.Count() % pageSize != 0)
            {
                Totalpages += 1;
            }
            filteredDishes = filteredDishes.Skip((page - 1) * pageSize).Take(pageSize);

            PaginationInfo pagination = new PaginationInfo();
            pagination.Size = pageSize;
            pagination.Count = Totalpages;
            pagination.Current = page;


            return new DishListResponse
            {
                Dishes = filteredDishes.ToList(),
                Pagination = pagination
            };
        }
    }
}
