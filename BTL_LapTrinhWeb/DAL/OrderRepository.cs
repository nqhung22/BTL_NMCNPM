using BTL_LapTrinhWeb.Models;
using System;
using System.Collections.Generic;
using System.Web;

namespace BTL_LapTrinhWeb.DAL
{
    public class OrderRepository
    {
        public void SaveOrder(Order order)
        {
            var application = HttpContext.Current.Application;
            lock (application)
            {
                var history = application["OrderHistory"] as List<Order> ?? new List<Order>();
                history.Add(order);
                application["OrderHistory"] = history;
            }
        }

        public List<Order> GetAllOrders()
        {
            var application = HttpContext.Current.Application;
            lock (application)
            {
                var orders = application["OrderHistory"] as List<Order>;
                return orders == null ? new List<Order>() : new List<Order>(orders);
            }
        }
    }
}