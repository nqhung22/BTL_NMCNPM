using BTL_LapTrinhWeb.Models;
using BTL_LapTrinhWeb.BLL;
using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BTL_LapTrinhWeb.Admin
{
    public partial class AdminAddProduct : System.Web.UI.Page
    {
        private ProductManager _pm = new ProductManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!(Session["UserLoggedIn"] is User user))
            {
                Response.Redirect("~/Presentation/LogIn.aspx");
                return;
            }

            if (!string.Equals(user.Username, "admin", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("~/Presentation/Trangchu.aspx");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string gender = ddlGender.SelectedValue;

                // FIX ID: Khớp chính xác với ID trong ảnh .aspx của bạn
                string mainImg = SaveFileAndGetPath(fuImageUrl, gender);
                string imgL = SaveFileAndGetPath(fuImageLeft, gender);
                string imgB = SaveFileAndGetPath(fuImageBack, gender);
                string imgT = SaveFileAndGetPath(fuImageTop, gender);
                string imgU = SaveFileAndGetPath(fuImageUnder, gender);

                // Kiểm tra dữ liệu đầu vào cơ bản
                if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtPrice.Text))
                {
                    lblErrorMessage.Text = "Vui lòng nhập tên và giá sản phẩm.";
                    lblErrorMessage.Visible = true;
                    return;
                }

                Products newProduct = new Products
                {
                    ProductName = txtName.Text.Trim(),
                    Price = decimal.Parse(txtPrice.Text),
                    Image = mainImg,
                    Image1 = imgL,
                    Image2 = imgB,
                    Image3 = imgT,
                    Image4 = imgU,
                    CategoryID = int.Parse(txtCategory.Text),
                    Gender = gender,
                    Description = txtDescription.Text.Trim(),
                    SalePercent = string.IsNullOrEmpty(txtSale.Text) ? 0 : decimal.Parse(txtSale.Text),
                    CreatedAt = DateTime.Now
                };

                // Lưu sản phẩm mới vào SQL Server.
                _pm.Insert(newProduct);

                Response.Redirect("~/Presentation/Admin.aspx");
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = "Lỗi hệ thống: " + ex.Message;
                lblErrorMessage.Visible = true;
            }
        }

        private string SaveFileAndGetPath(FileUpload fileUploadControl, string gender)
        {
            if (fileUploadControl != null && fileUploadControl.HasFile)
            {
                string extension = Path.GetExtension(fileUploadControl.FileName);
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
                if (!Array.Exists(allowedExtensions, allowed =>
                    string.Equals(allowed, extension, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException("Only JPG, PNG, and WebP product images are supported.");
                }

                if (fileUploadControl.PostedFile.ContentLength > 3 * 1024 * 1024)
                {
                    throw new InvalidOperationException("Product images must be no larger than 3 MB.");
                }

                string fileName = Guid.NewGuid().ToString("N") + extension.ToLowerInvariant();
                string folderPath = "~/Assets/images/" + gender + "/";
                string serverPath = Server.MapPath(folderPath);

                if (!Directory.Exists(serverPath))
                {
                    Directory.CreateDirectory(serverPath);
                }

                fileUploadControl.SaveAs(Path.Combine(serverPath, fileName));
                return folderPath + fileName;
            }
            return "";
        }
    }
}