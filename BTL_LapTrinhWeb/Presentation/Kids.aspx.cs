using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using BTL_LapTrinhWeb.Models;
using BTL_LapTrinhWeb.BLL;

namespace BTL_LapTrinhWeb
{
    public partial class Kids : System.Web.UI.Page
    {
        private ProductManager _pm = new ProductManager();

        private string CurrentSubCategory
        {
            get => ViewState["CurrentSubCategory"] as string ?? "All";
            set => ViewState["CurrentSubCategory"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserLoggedIn"] is User user)
            {
                lnkSignIn.Text = user.Name;
            }

            if (!IsPostBack)
            {
                txtSearch.Text = Request.QueryString["search"] ?? "";
                LoadProducts();
            }
        }

        private void LoadProducts()
        {
            string category = CurrentSubCategory;
            string sort = ddlSort.SelectedValue;
            string price = ddlFilter.SelectedValue;
            string keyword = txtSearch.Text.Trim();

            // Truyền tham số "Kids" để lọc đúng giới tính trong DB
            var data = _pm.GetProducts(
                "Kids",
                false,
                category,
                keyword,
                price,
                sort
            );

            rptProducts.DataSource = data;
            rptProducts.DataBind();
        }

        protected void btnCategory_Click(object sender, EventArgs e)
        {
            CurrentSubCategory = ((LinkButton)sender).CommandArgument;
            LoadProducts();
        }

        protected void ddlSortFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadProducts();
        }

        protected void btnSearch_Click(object sender, ImageClickEventArgs e)
        {
            LoadProducts();
        }

        protected void lnkSignIn_Click(object sender, EventArgs e)
        {
            Response.Redirect(Session["UserLoggedIn"] is User
                ? "~/Presentation/myaccount.aspx"
                : "~/Presentation/LogIn.aspx");
        }

        protected void lnkFavorites_Click(object sender, EventArgs e)
        {
            Response.Redirect(Session["UserLoggedIn"] != null
                ? "~/Presentation/Favorites.aspx"
                : "~/Presentation/LogIn.aspx");
        }

        protected void lnkCart_Click(object sender, EventArgs e)
        {
            Response.Redirect(Session["UserLoggedIn"] != null
                ? "~/Presentation/Cart.aspx"
                : "~/Presentation/LogIn.aspx");
        }

        protected void btnAddToFavorites_Click(object sender, ImageClickEventArgs e)
        {
            var button = (ImageButton)sender;
            var favorites = Session["Favorites"] as List<string> ?? new List<string>();
            string key = $"Product-{button.CommandArgument}";

            if (!favorites.Remove(key))
            {
                favorites.Add(key);
            }

            Session["Favorites"] = favorites;
            LoadProducts();
        }

        protected bool IsFavorite(string productId, string page)
        {
            var favorites = Session["Favorites"] as List<string>;
            return favorites != null && favorites.Contains($"Product-{productId}");
        }

        protected void btnBuyNow_Click(object sender, EventArgs e)
        {
            string productId = ((LinkButton)sender).CommandArgument;
            Response.Redirect($"~/Presentation/Product_Detail.aspx?id={productId}");
        }

        public string GetPriceDisplay(object dataItem)
        {
            var p = (Products)dataItem;
            decimal finalPrice = _pm.GetFinalPrice(p);
            if (p.SalePercent > 0)
            {
                return $"<span class='old-price'>${p.Price:F2}</span> <span class='new-price'>${finalPrice:F2}</span>";
            }
            return $"<span>${p.Price:F2}</span>";
        }
    }
}