using System.Collections.Generic;
using BTL_LapTrinhWeb.Models;
using BTL_LapTrinhWeb.BLL;

namespace BTL_LapTrinhWeb.Services
{
    public class ProductDetailService
    {
        private ProductDetailManager _manager = new ProductDetailManager();

        public Products GetProduct(int id)
        {
            return _manager.GetProduct(id);
        }

        public List<Products> GetSimilarProducts(Products current)
        {
            return _manager.GetSimilarProducts(current);
        }
        public decimal GetDisplayPrice(Products product)
        {
            return _manager.GetDiscountPrice(product);
        }

        public List<string> ToggleFavorite(List<string> favorites, string key)
        {
            return _manager.ToggleFavorite(favorites, key);
        }

        public List<Review> GetReviews(int productId, int userId)
        {
            return _manager.GetReviews(productId, userId);
        }

        public bool HasPurchased(int userId, int productId)
        {
            return _manager.HasPurchased(userId, productId);
        }

        public void SaveReview(Review review)
        {
            _manager.SaveReview(review);
        }

        public void DeleteReview(int reviewId, int userId)
        {
            _manager.DeleteReview(reviewId, userId);
        }
    }
}