# IT Shoes — ASP.NET Web Forms + PostgreSQL

Dự án bán giày dùng C#, ASP.NET Web Forms và .NET Framework 4.7.2.
**Dự án cần kết nối database.** Mã nguồn ban đầu dùng SQL Server và gọi các stored procedure, nhưng không kèm file SQL, bản backup hoặc dữ liệu database. README cũ mô tả lưu trong RAM không còn đúng với mã nguồn.

Hiện dự án đã chuyển sang PostgreSQL bằng Npgsql 8.0.9. Tài khoản, sản phẩm, giỏ hàng, đơn hàng và chi tiết đơn hàng được lưu trong PostgreSQL. Session chỉ giữ trạng thái đăng nhập và danh sách yêu thích.

## Kết nối

Cấu hình nằm trong `BTL_LapTrinhWeb/Web.config`, mục `connectionStrings/MyDB`:

```text
Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=1;Search Path=itshoes
```

Thông tin này tương ứng với URL `jdbc:postgresql://localhost:5432/postgres` của bạn. Ứng dụng C# dùng Npgsql, nên không dùng trực tiếp URL JDBC.

Toàn bộ bảng của dự án nằm trong **schema `itshoes`** của database **`postgres`**. Các bảng đang có trong schema `public` thuộc ứng dụng khác được giữ nguyên.

## Khởi tạo database

Để gửi cho người khác, dùng **`database/IT_Shoes_PostgreSQL.zip`**, gồm file SQL gộp `IT_Shoes_PostgreSQL.sql` và hướng dẫn `HUONG_DAN_DATABASE.txt`. File SQL gộp có thể chạy trực tiếp bằng Query Tool của pgAdmin trong database `postgres`.
Gói này chứa cấu trúc và dữ liệu mẫu; cần gửi kèm mã nguồn hiện tại cùng ảnh trong `Assets/images` để chạy website.

Trong pgAdmin, chọn database `postgres`, mở **Query Tool** và chạy lần lượt:

1. `database/01_schema.sql`: tạo schema, 7 bảng, khóa ngoại, chỉ mục và ràng buộc dữ liệu.
2. `database/02_seed.sql`: thêm 4 danh mục, 40 sản phẩm mẫu và tài khoản admin.

Hoặc chạy từ thư mục gốc của dự án bằng PowerShell (điều chỉnh đường dẫn `psql.exe` theo máy):

```powershell
& 'D:\PostgreSQL\bin\psql.exe' -h localhost -p 5432 -U postgres -d postgres -W -v ON_ERROR_STOP=1 -f .\database\01_schema.sql -f .\database\02_seed.sql
```

Nhập mật khẩu PostgreSQL `1` khi được hỏi. Có thể chạy lại các script; dữ liệu mẫu đã có sẽ không bị ghi đè. Đây là script khởi tạo, không phải công cụ nâng cấp một schema cũ có cấu trúc khác.

Dữ liệu sản phẩm là **dữ liệu demo mới**, được tạo theo các ảnh có sẵn trong thư mục `Assets/images`: 18 sản phẩm Men, 13 Women và 9 Kids. Đây không phải bản khôi phục database gốc.

| Bảng | Nội dung |
| --- | --- |
| `itshoes.categories` | Danh mục: Sneakers, Lifestyles/Casual, Sports, Boots |
| `itshoes.users` | Tài khoản, mật khẩu băm PBKDF2 và thông tin cá nhân |
| `itshoes.products` | Sản phẩm, giá, giảm giá, ảnh, size và tồn kho |
| `itshoes.cart` | Giỏ hàng theo người dùng, sản phẩm và size |
| `itshoes.orders` | Đơn hàng và thông tin giao hàng |
| `itshoes.order_items` | Chi tiết sản phẩm tại thời điểm đặt hàng |
| `itshoes.reviews` | Bảng dự phòng theo model Review; giao diện đánh giá hiện chưa có xử lý nghiệp vụ |

Lưu đơn hàng, chi tiết đơn hàng và xóa giỏ hàng được thực hiện trong cùng một transaction. Thông tin hàng đã mua được giữ lại khi sản phẩm được sửa hoặc xóa.

## Chạy website

1. Cài PostgreSQL, Visual Studio có workload **ASP.NET and web development**, và .NET Framework 4.7.2 targeting pack.
2. Khởi tạo database theo hướng dẫn trên.
3. Mở `BTL_LapTrinhWeb.sln`, restore NuGet packages rồi **Build → Rebuild Solution**.
4. Chạy bằng IIS Express và mở `/Presentation/Trangchu.aspx`.

Cổng IIS Express của dự án là `50891`: `http://localhost:50891/Presentation/Trangchu.aspx`.
Cổng cũ `49852` nằm trong dải cổng Windows dành riêng trên máy này và gây lỗi khởi động `0x80070020`.
Nếu Visual Studio đang mở khi cấu hình được cập nhật, mở lại solution trước khi nhấn F5.
Trong **Project → Properties → Web**, `Project Url` phải là `http://localhost:50891/`.
Chọn **Specific Page** và nhập `Presentation/Trangchu.aspx` để F5 mở đúng trang chủ; thiết lập này được Visual Studio lưu trong file `.csproj.user` trên máy.

Tài khoản quản trị mẫu: **`admin` / `12345678`**. Đây là tài khoản website, khác với tài khoản PostgreSQL `postgres` / `1`. Đăng nhập admin cũng được xác thực từ database.

## Kiểm tra kết nối và nghiệp vụ database

Build dự án trước, sau đó chạy:

```powershell
.\database\Test-Database.ps1
```

Bài kiểm tra tạo schema riêng có tên `itshoes_test_<mã ngẫu nhiên>`, kiểm tra khởi tạo lặp lại, đăng nhập, đăng ký, đổi mật khẩu, cập nhật hồ sơ, CRUD sản phẩm, giỏ hàng, lưu đơn và rollback khi dữ liệu không hợp lệ. Schema thử nghiệm được xóa khi kết thúc; dữ liệu thật của ứng dụng được giữ nguyên.

Nếu máy chưa có Web Application targets, cần bổ sung workload ASP.NET trong Visual Studio Installer trước khi build bình thường. Phần kiểm tra PowerShell cần .NET Framework 4.7.2 targeting pack.

Tham khảo tương thích thư viện: [Npgsql 9.0 release notes — tiếp tục hỗ trợ .NET Framework qua nhánh 8.0](https://www.npgsql.org/doc/release-notes/9.0.html).
