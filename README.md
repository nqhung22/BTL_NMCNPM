# IT Shoes — ASP.NET Web Forms + SQL Server

Dự án bán giày dùng C#, ASP.NET Web Forms và .NET Framework 4.7.2.
Database sử dụng **Microsoft SQL Server LocalDB** (không cần cài đặt SQL Server đầy đủ).

## Kết nối

Cấu hình nằm trong `BTL_LapTrinhWeb/Web.config`, mục `connectionStrings/MyDB`:

```xml
<add name="MyDB"
     connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=IT_Shoes_DB;Integrated Security=True"
     providerName="System.Data.SqlClient" />
```

Database tên `IT_Shoes_DB` chạy trên **SQL Server LocalDB** (đi kèm Visual Studio, không cần cài thêm).

## Khởi tạo database

Chạy file `database/IT_Shoes_SQLServer.sql` để tạo database và dữ liệu mẫu:

**Cách 1 — Dùng SQL Server Management Studio (SSMS) hoặc Azure Data Studio:**
1. Mở SSMS, kết nối tới `(localdb)\MSSQLLocalDB`
2. Mở file `database/IT_Shoes_SQLServer.sql`
3. Nhấn **Execute (F5)**

**Cách 2 — Dùng PowerShell / sqlcmd:**
```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -i ".\database\IT_Shoes_SQLServer.sql"
```

Script tự động tạo database nếu chưa có, có thể chạy lại nhiều lần mà không lỗi.

| Bảng | Nội dung |
| --- | --- |
| `dbo.categories` | Danh mục: Men, Women, Kids, Sale |
| `dbo.users` | Tài khoản, mật khẩu băm PBKDF2 và thông tin cá nhân |
| `dbo.products` | Sản phẩm, giá, giảm giá, ảnh, size và tồn kho |
| `dbo.cart` | Giỏ hàng theo người dùng, sản phẩm và size |
| `dbo.orders` | Đơn hàng và thông tin giao hàng |
| `dbo.order_items` | Chi tiết sản phẩm tại thời điểm đặt hàng |
| `dbo.reviews` | Đánh giá sản phẩm |

## Chạy website

1. Cài **Visual Studio 2022** với workload **ASP.NET and web development** và **.NET Framework 4.7.2 targeting pack**.
2. SQL Server LocalDB đã có sẵn khi cài Visual Studio — không cần cài thêm.
3. Khởi tạo database theo hướng dẫn trên.
4. Mở `BTL_LapTrinhWeb.sln`, restore NuGet packages rồi **Build → Rebuild Solution**.
5. Chạy bằng IIS Express và mở `/Presentation/Trangchu.aspx`.

URL mặc định: `http://localhost:50891/Presentation/Trangchu.aspx`

Tài khoản quản trị mẫu: **`admin` / `12345678`**

## Cấu trúc dự án

```
BTL_LapTrinhWeb/
├── DAL/            # Data Access Layer (DBHelper, Repository)
├── BLL/            # Business Logic Layer (Manager)
├── Models/         # Entity classes
├── Services/       # Service layer
├── Presentation/   # ASPX pages
├── Assets/
│   ├── css/        # Stylesheet cho từng trang
│   └── images/     # Hình ảnh sản phẩm và icon
└── Web.config      # Cấu hình kết nối database
```
