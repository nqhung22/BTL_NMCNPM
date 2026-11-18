using BTL_LapTrinhWeb.Models;
using BTL_LapTrinhWeb.DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BTL_LapTrinhWeb.BLL
{
    public class CheckoutManager
    {
        private OrderRepository _repo = new OrderRepository();

        public decimal CalculateTotal(List<CartItem> cart)
        {
            return cart.Sum(i => i.LineTotal);
        }

        public Order CreateOrder(User user, List<CartItem> cart, string payment, string address, string city, string postal)
        {
            decimal total = CalculateTotal(cart);

            string orderCode = $"ORD-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpperInvariant()}";

            return new Order
            {
                OrderNumber = orderCode,
                Date = DateTime.Now,
                UserName = user.Name,
                UserID = user.Id,
                TotalAmount = total,
                PaymentMethod = payment,
                Address = address,
                City = city,
                Postal = postal,
                Items = cart
            };
        }

        public void SaveOrder(Order order)
        {
            _repo.SaveOrder(order);
        }
    }
}