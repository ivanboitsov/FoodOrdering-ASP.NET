using FoodOrderingWebsite.Data;
using FoodOrderingWebsite.Models.Basket;
using FoodOrderingWebsite.Models.Dish;
using FoodOrderingWebsite.Models.Order;
using System;

namespace FoodOrderingWebsite.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;
        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool IsOrderTimeValid(DateTime dateTime)
        {
            if (dateTime >= DateTime.Now.AddMinutes(30))
            {
                return true;
            }
            return false;
        }

        public OrderResponseModel? GetOrderById(Guid id)
        {
            Order order = _context.Orders.FirstOrDefault(o => o.Id == id);
            if (order != null)
            {
                OrderResponseModel response = new OrderResponseModel
                {
                    Id = order.Id,
                    DeliveryTime = order.DeliveryTime,
                    OrderTime = order.OrderTime,
                    Status = order.Status,
                    Price = order.Price,
                    Address = order.Address,
                };
                List<BasketResponseModel> dishes = new List<BasketResponseModel>();
                List<Basket> baskets = _context.Baskets.ToList();
                foreach (var basket in baskets)
                {
                    if (basket.UserId == order.UserId)
                    {
                        dishes.Add(new BasketResponseModel
                        {
                            Id = basket.Id,
                            Name = basket.Name,
                            Amount = basket.Amount,
                            Price = basket.Price,
                            TotalPrice = basket.TotalPrice,
                            Image = basket.Image
                        });
                    }
                }
                response.dishes = dishes;

                return response;
            }
            return null;
        }

        public List<OrderResponseModel>? GetOrders()
        {
            List<Order> orders = _context.Orders.ToList();
            List<OrderResponseModel> response = new List<OrderResponseModel>();
            if (orders != null)
            {
                foreach (var order in orders)
                {
                    OrderResponseModel orderRes = new OrderResponseModel
                    {
                        Id = order.Id,
                        DeliveryTime = order.DeliveryTime,
                        OrderTime = order.OrderTime,
                        Status = order.Status,
                        Price = order.Price,
                        Address = order.Address,
                    };
                    List<BasketResponseModel> dishes = new List<BasketResponseModel>();
                    List<Basket> baskets = _context.Baskets.ToList();
                    foreach (var basket in baskets)
                    {
                        if (basket.UserId == order.UserId && basket.OrderId == order.Id)
                        {
                            dishes.Add(new BasketResponseModel
                            {
                                Id = basket.Id,
                                Name = basket.Name,
                                Amount = basket.Amount,
                                Price = basket.Price,
                                TotalPrice = basket.TotalPrice,
                                Image = basket.Image
                            });
                        }
                    }
                    orderRes.dishes = dishes;
                    response.Add(orderRes);
                }

                return response;
            }
            return null;
        }

        public OrderResponseModel? CreateOrder(CreateOrderModel rawOrder, Guid user)
        {
            List<Order> userOrders = _context.Orders.Where(o => o.UserId == user).ToList();

            Order order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = user,
                OrderTime = DateTime.UtcNow,
                DeliveryTime = rawOrder.deliveryTime,
                Status = "In Progress",
                Price = 0,
                Address = rawOrder.addresId
            };
            List<Basket> baskets = _context.Baskets.Where(b => b.UserId == order.UserId && b.OrderId == Guid.Empty).ToList();

            if (baskets.Count() <= 0)
            {
                return null;
            }

            List<BasketResponseModel> Dishes = new List<BasketResponseModel>();

            foreach (var basket in baskets)
            {
                basket.OrderId = order.Id;
                _context.Baskets.Update(basket);
                order.Price += basket.TotalPrice;
                Dishes.Add(new BasketResponseModel
                {
                    Id = basket.Id,
                    Name = basket.Name,
                    Amount = basket.Amount,
                    Price = basket.Price,
                    TotalPrice = basket.TotalPrice,
                    Image = basket.Image,
                });
            }

            if (Dishes.Count() > 0)
            {
                OrderResponseModel response = new OrderResponseModel
                {
                    Id = order.Id,
                    DeliveryTime = order.DeliveryTime,
                    OrderTime = order.OrderTime,
                    Status = order.Status,
                    Price = order.Price,
                    dishes = Dishes,
                    Address = order.Address,
                };

                _context.Orders.Add(order);
                _context.SaveChanges();
                return response;
            }

            return null;
        }

        public string? ConfirmOrder(Guid ordId, Guid userId)
        {
            Order order = _context.Orders.FirstOrDefault(o => o.Id == ordId);
            if (order != null)
            {
                if (order.UserId == userId)
                {
                    if (order.Status != "Confirmed")
                    {
                        order.Status = "Confirmed";
                        _context.Orders.Update(order);
                        List<Basket> baskets = _context.Baskets.Where(b => b.UserId == userId).ToList();
                        foreach (var basket in baskets)
                        {
                            UserTestedDish tested = _context.TestedDishes.FirstOrDefault(d => d.UserId == userId && d.DishId == basket.DishId);
                            if (tested == null)
                            {
                                tested = new UserTestedDish
                                {
                                    Id = Guid.NewGuid(),
                                    UserId = userId,
                                    DishId = basket.DishId
                                };
                                _context.TestedDishes.Add(tested);
                            }
                        }
                        _context.SaveChanges();
                        return "ok";
                    }
                    return "alreadyConfirmed";
                }
                return "forbidden";
            }
            return "notfound";
        }
    }
}
