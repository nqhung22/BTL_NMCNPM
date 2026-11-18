using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BTL_LapTrinhWeb.Models;
using BTL_LapTrinhWeb.BLL;
using BTL_LapTrinhWeb.Services;

namespace BTL_LapTrinhWeb
{
    public partial class Product_Detail : System.Web.UI.Page
    {
        private ProductManager _pm = new ProductManager();
        private CartManager _cart = new CartManager();
        private ProductDetailManager _pdm = new ProductDetailManager();
        private ProductDetailService _reviewService = new ProductDetailService();

        private int ProductID
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["id"], out id) ? id : 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserLoggedIn"] is User user)
            {
                lnkSignIn.Text = user.Name;
            }

            if (!IsPostBack)
            {
                LoadProduct();
            }
        }

        private void LoadProduct()
        {
            var product = _pm.GetByID(ProductID);

            if (product == null)
            {
                Response.Redirect("~/Presentation/Trangchu.aspx");
                return;
            }

            product_name.InnerText = product.ProductName;
            product_description.InnerText = product.Description;
            review_product_name.InnerText = product.ProductName;
            btnFavoriteIcon.CommandArgument = product.ProductID.ToString();

            decimal finalPrice = _pm.GetFinalPrice(product);

            if (product.SalePercent > 0)
            {
                product_price.InnerHtml =
                    $"<span class='old-price'>${product.Price:F2}</span> " +
                    $"<span class='new-price'>${finalPrice:F2}</span>";
            }
            else
            {
                product_price.InnerText = $"${product.Price:F2}";
            }

            main_image.Src = ResolveUrl(product.Image ?? "~/Assets/images/no-image.png");

            imgThumbnailMain.Src = ResolveUrl(product.Image ?? "~/Assets/images/no-image.png");
            imgThumbnailBack.Src = ResolveUrl(product.Image1 ?? product.Image);
            imgThumbnailLeft.Src = ResolveUrl(product.Image2 ?? product.Image);
            imgThumbnailTop.Src = ResolveUrl(product.Image3 ?? product.Image);
            imgThumbnailUnder.Src = ResolveUrl(product.Image4 ?? product.Image);

            btnAddToBag.CommandArgument = product.ProductID.ToString();
            btnBuyAndCheckOut.CommandArgument = product.ProductID.ToString();
            btnFavoriteIcon.ImageUrl = IsFavorite(product.ProductID.ToString())
                ? "~/Assets/images/Chung/icon_favorite_green.png"
                : "~/Assets/images/Chung/icon_favorite.png";

            LoadSimilarProducts(product);
            LoadReviews();
        }

        private void LoadSimilarProducts(Products product)
        {
            var similar = _pdm.GetSimilarProducts(product);

            rptSimilarProducts.DataSource = similar;
            rptSimilarProducts.DataBind();
        }

        protected void AddToBag_Click(object sender, EventArgs e)
        {
            if (TryAddToCart())
            {
                lblAddToBagMessage.Text = "Product added to your bag.";
                lblAddToBagMessage.Visible = true;
            }
        }

        protected void BuyAndCheckOut_Click(object sender, EventArgs e)
        {
            if (TryAddToCart())
            {
                Response.Redirect("~/Presentation/Checkout.aspx");
            }
        }

        private bool TryAddToCart()
        {
            if (!(Session["UserLoggedIn"] is User user))
            {
                Response.Redirect("~/Presentation/LogIn.aspx");
                return false;
            }

            lblSizeError.Text = "";
            lblQuantityError.Text = "";

            if (!int.TryParse(ddlSize.SelectedValue, out int size))
            {
                lblSizeError.Text = "Please choose a size.";
                return false;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                lblQuantityError.Text = "Enter a quantity greater than zero.";
                return false;
            }

            _cart.Add(user.Id, ProductID, size, quantity);
            return true;
        }

        protected void btnAddToFavorites_Click(object sender, ImageClickEventArgs e)
        {
            string productId = ((ImageButton)sender).CommandArgument;
            var favorites = Session["Favorites"] as List<string> ?? new List<string>();
            string key = $"Product-{productId}";

            if (!favorites.Remove(key))
            {
                favorites.Add(key);
            }

            Session["Favorites"] = favorites;
            btnFavoriteIcon.ImageUrl = IsFavorite(productId)
                ? "~/Assets/images/Chung/icon_favorite_green.png"
                : "~/Assets/images/Chung/icon_favorite.png";
        }

        public bool IsFavorite(string productId)
        {
            var favorites = Session["Favorites"] as List<string> ?? new List<string>();
            return favorites.Contains($"Product-{productId}");
        }

        public bool IsFavorite(string productId, string gender)
        {
            return IsFavorite(productId);
        }

        private void LoadReviews()
        {
            var user = Session["UserLoggedIn"] as User;
            int userId = user?.Id ?? 0;
            var reviews = _reviewService.GetReviews(ProductID, userId);

            rptReviews.DataSource = reviews;
            rptReviews.DataBind();
            lblNoReview.Visible = reviews.Count == 0;
            lblReviewCount.Text = reviews.Count.ToString();

            if (reviews.Count > 0)
            {
                rating_value.InnerText = reviews.Average(review => review.Rating).ToString("F1");
            }

            pnlAddReview.Visible = user != null && _reviewService.HasPurchased(user.Id, ProductID);
        }

        protected void btnSubmitReview_Click(object sender, EventArgs e)
        {
            if (!(Session["UserLoggedIn"] is User user))
            {
                Response.Redirect("~/Presentation/LogIn.aspx");
                return;
            }

            if (!_reviewService.HasPurchased(user.Id, ProductID))
            {
                lblNoReview.Text = "Only customers who purchased this product can leave a review.";
                lblNoReview.Visible = true;
                return;
            }

            if (!int.TryParse(ddlRating.SelectedValue, out int rating) || rating < 1 || rating > 5 ||
                string.IsNullOrWhiteSpace(txtReview.Text))
            {
                lblFormTitle.Text = "Please select a rating and enter a comment.";
                return;
            }

            int reviewId = ViewState["EditingReviewID"] == null
                ? 0
                : Convert.ToInt32(ViewState["EditingReviewID"]);

            _reviewService.SaveReview(new Review
            {
                ReviewID = reviewId,
                UserID = user.Id,
                ProductID = ProductID,
                Rating = rating,
                Comment = txtReview.Text.Trim()
            });

            ViewState.Remove("EditingReviewID");
            txtReview.Text = "";
            ddlRating.SelectedValue = "0";
            btnSubmitReview.Text = "Submit Review";
            btnCancelEdit.Visible = false;
            LoadReviews();
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            ViewState.Remove("EditingReviewID");
            txtReview.Text = "";
            ddlRating.SelectedValue = "0";
            btnSubmitReview.Text = "Submit Review";
            btnCancelEdit.Visible = false;
        }

        protected void rptReviews_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!(Session["UserLoggedIn"] is User user) ||
                !int.TryParse(e.CommandArgument.ToString(), out int reviewId))
            {
                return;
            }

            if (e.CommandName == "EditReview")
            {
                var review = _reviewService.GetReviews(ProductID, user.Id)
                    .FirstOrDefault(item => item.ReviewID == reviewId && item.CanEdit);
                if (review == null)
                {
                    return;
                }

                ViewState["EditingReviewID"] = review.ReviewID;
                ddlRating.SelectedValue = review.Rating.ToString();
                txtReview.Text = review.Comment;
                btnSubmitReview.Text = "Update Review";
                btnCancelEdit.Visible = true;
            }
            else if (e.CommandName == "DeleteReview")
            {
                _reviewService.DeleteReview(reviewId, user.Id);
                LoadReviews();
            }
        }

        protected void search_button_Click(object sender, ImageClickEventArgs e)
        {
            string search = search_box.Text.Trim();
            Response.Redirect(string.IsNullOrEmpty(search)
                ? "~/Presentation/Men.aspx"
                : "~/Presentation/Men.aspx?search=" + Server.UrlEncode(search));
        }

        public string GetPriceDisplay(object dataItem)
        {
            decimal price = (decimal)DataBinder.Eval(dataItem, "Price");
            decimal sale = DataBinder.Eval(dataItem, "SalePercent") != DBNull.Value
                ? Convert.ToDecimal(DataBinder.Eval(dataItem, "SalePercent"))
                : 0;

            decimal finalPrice = sale > 0
                ? price * (1 - sale / 100)
                : price;

            if (sale > 0)
            {
                return $@"
                <span class='old-price'>${price:F2}</span>
                <span class='new-price'>${finalPrice:F2}</span>";
            }

            return $"${price:F2}";
        }

        protected void lnkSignIn_Click(object sender, EventArgs e)
        {
            Response.Redirect(Session["UserLoggedIn"] is User
                ? "~/Presentation/myaccount.aspx"
                : "~/Presentation/LogIn.aspx");
        }

        protected void lnkFavorites_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Presentation/Favorites.aspx");
        }

        protected void lnkCart_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Presentation/Cart.aspx");
        }
    }
}