using System;
using System.Collections.Generic;
using System.Linq;
using BTL_LapTrinhWeb.Models;

namespace BTL_LapTrinhWeb.DAL
{
    public class ProductDetailRepository
    {
        private ProductRepository _repo = new ProductRepository();

        public Products GetProductById(int id)
        {
            return _repo.GetProductByID(id);
        }

        public List<Products> GetSimilarProducts(int categoryId, string gender, int productId)
        {
            return _repo.GetAllProducts()
                .Where(p => p.CategoryID == categoryId &&
                            p.Gender == gender &&
                            p.ProductID != productId)
                .Take(6)
                .ToList();
        }

        public List<Review> GetReviews(int productId, int userId)
        {
            var reviews = new List<Review>();
            const string query = @"
                SELECT r.ReviewID, r.UserID, r.ProductID, r.Rating, r.Comment, r.CreatedAt,
                       COALESCE(NULLIF(u.Name COLLATE DATABASE_DEFAULT, ''),
                                u.Username COLLATE DATABASE_DEFAULT) AS Author,
                       CAST(CASE WHEN r.UserID = @UserID THEN 1 ELSE 0 END AS bit) AS CanEdit
                FROM dbo.reviews AS r
                INNER JOIN dbo.Users AS u ON u.UserID = r.UserID
                WHERE r.ProductID = @ProductID
                ORDER BY r.CreatedAt DESC, r.ReviewID DESC";

            using (var connection = DBHelper.GetConnection())
            using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserID", userId);
                command.Parameters.AddWithValue("@ProductID", productId);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reviews.Add(new Review
                        {
                            ReviewID = (int)reader["ReviewID"],
                            UserID = (int)reader["UserID"],
                            ProductID = (int)reader["ProductID"],
                            Rating = (int)reader["Rating"],
                            Comment = reader["Comment"].ToString(),
                            CreatedAt = (DateTime)reader["CreatedAt"],
                            Author = reader["Author"].ToString(),
                            CanEdit = (bool)reader["CanEdit"]
                        });
                    }
                }
            }

            return reviews;
        }

        public void SaveReview(Review review)
        {
            string query = review.ReviewID == 0
                ? @"INSERT INTO dbo.reviews (UserID, ProductID, Rating, Comment, CreatedAt)
                    VALUES (@UserID, @ProductID, @Rating, @Comment, GETDATE())"
                : @"UPDATE dbo.reviews
                    SET Rating = @Rating, Comment = @Comment
                    WHERE ReviewID = @ReviewID AND UserID = @UserID AND ProductID = @ProductID";

            using (var connection = DBHelper.GetConnection())
            using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserID", review.UserID);
                command.Parameters.AddWithValue("@ProductID", review.ProductID);
                command.Parameters.AddWithValue("@Rating", review.Rating);
                command.Parameters.AddWithValue("@Comment", review.Comment);
                if (review.ReviewID != 0)
                {
                    command.Parameters.AddWithValue("@ReviewID", review.ReviewID);
                }

                connection.Open();
                int affectedRows = command.ExecuteNonQuery();
                if (affectedRows == 0)
                {
                    throw new System.InvalidOperationException("The review could not be saved because it no longer exists or is not owned by this user.");
                }
            }
        }

        public void DeleteReview(int reviewId, int userId)
        {
            using (var connection = DBHelper.GetConnection())
            using (var command = new System.Data.SqlClient.SqlCommand(
                "DELETE FROM dbo.reviews WHERE ReviewID = @ReviewID AND UserID = @UserID", connection))
            {
                command.Parameters.AddWithValue("@ReviewID", reviewId);
                command.Parameters.AddWithValue("@UserID", userId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}