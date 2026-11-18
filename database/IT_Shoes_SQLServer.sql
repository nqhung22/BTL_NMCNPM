-- =====================================================================
-- IT SHOES - MICROSOFT SQL SERVER SETUP SCRIPT
-- Generated from PostgreSQL database on 2026-11-17
-- Target database: IT_Shoes_DB
-- Compatible with: SQL Server 2016+, SQL Server Express, SQL Server LocalDB
-- =====================================================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'IT_Shoes_DB')
BEGIN
    CREATE DATABASE [IT_Shoes_DB];
END
GO

USE [IT_Shoes_DB];
GO

-- 1. DROP EXISTING TABLES IN REVERSE ORDER OF DEPENDENCY
IF OBJECT_ID('dbo.reviews', 'U') IS NOT NULL DROP TABLE dbo.reviews;
IF OBJECT_ID('dbo.order_items', 'U') IS NOT NULL DROP TABLE dbo.order_items;
IF OBJECT_ID('dbo.orders', 'U') IS NOT NULL DROP TABLE dbo.orders;
IF OBJECT_ID('dbo.cart', 'U') IS NOT NULL DROP TABLE dbo.cart;
IF OBJECT_ID('dbo.products', 'U') IS NOT NULL DROP TABLE dbo.products;
IF OBJECT_ID('dbo.users', 'U') IS NOT NULL DROP TABLE dbo.users;
IF OBJECT_ID('dbo.categories', 'U') IS NOT NULL DROP TABLE dbo.categories;
GO

-- 2. CREATE TABLES

-- Categories
CREATE TABLE dbo.categories (
    categoryid INT IDENTITY(1,1) PRIMARY KEY,
    categoryname NVARCHAR(100) NOT NULL UNIQUE
);
GO

-- Users
CREATE TABLE dbo.users (
    userid INT IDENTITY(1,1) PRIMARY KEY,
    username NVARCHAR(100) NOT NULL,
    password NVARCHAR(MAX) NOT NULL,
    surname NVARCHAR(100) NULL,
    name NVARCHAR(100) NULL,
    email NVARCHAR(254) NULL,
    phone NVARCHAR(30) NULL,
    quequan NVARCHAR(200) NULL,
    address1 NVARCHAR(MAX) NULL,
    address2 NVARCHAR(MAX) NULL,
    postal NVARCHAR(20) NULL,
    city NVARCHAR(100) NULL,
    country NVARCHAR(100) NULL,
    createdat DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT CK_users_username CHECK (LEN(LTRIM(RTRIM(username))) > 0)
);
CREATE UNIQUE INDEX users_username_unique ON dbo.users (username);
GO

-- Products
CREATE TABLE dbo.products (
    productid INT IDENTITY(1,1) PRIMARY KEY,
    productname NVARCHAR(200) NOT NULL,
    price DECIMAL(12,2) NOT NULL CHECK (price >= 0),
    description NVARCHAR(MAX) NULL,
    image NVARCHAR(MAX) NULL,
    stock INT NOT NULL DEFAULT 0 CHECK (stock >= 0),
    categoryid INT NOT NULL REFERENCES dbo.categories(categoryid),
    image1 NVARCHAR(MAX) NULL,
    image2 NVARCHAR(MAX) NULL,
    image3 NVARCHAR(MAX) NULL,
    image4 NVARCHAR(MAX) NULL,
    gender NVARCHAR(20) NOT NULL CHECK (gender IN ('Men', 'Women', 'Kids')),
    size NVARCHAR(100) NULL,
    salepercent DECIMAL(5,2) NOT NULL DEFAULT 0 CHECK (salepercent BETWEEN 0 AND 100),
    createdat DATETIME NOT NULL DEFAULT GETDATE()
);
CREATE INDEX products_category_gender ON dbo.products (categoryid, gender);
GO

-- Cart
CREATE TABLE dbo.cart (
    cartid INT IDENTITY(1,1) PRIMARY KEY,
    userid INT NOT NULL REFERENCES dbo.users(userid) ON DELETE CASCADE,
    productid INT NOT NULL REFERENCES dbo.products(productid) ON DELETE CASCADE,
    size INT NOT NULL DEFAULT 0 CHECK (size >= 0),
    quantity INT NOT NULL CHECK (quantity > 0),
    CONSTRAINT UQ_cart_user_prod_size UNIQUE (userid, productid, size)
);
GO

-- Orders
CREATE TABLE dbo.orders (
    orderid INT IDENTITY(1,1) PRIMARY KEY,
    ordernumber NVARCHAR(80) NOT NULL UNIQUE,
    userid INT NULL REFERENCES dbo.users(userid) ON DELETE SET NULL,
    username NVARCHAR(100) NOT NULL,
    orderdate DATETIME NOT NULL DEFAULT GETDATE(),
    totalamount DECIMAL(14,2) NOT NULL CHECK (totalamount >= 0),
    address NVARCHAR(MAX) NULL,
    city NVARCHAR(100) NULL,
    postal NVARCHAR(20) NULL,
    paymentmethod NVARCHAR(100) NULL
);
CREATE INDEX orders_user_date ON dbo.orders (userid, orderdate DESC);
GO

-- Order Items
CREATE TABLE dbo.order_items (
    orderitemid INT IDENTITY(1,1) PRIMARY KEY,
    orderid INT NOT NULL REFERENCES dbo.orders(orderid) ON DELETE CASCADE,
    productid INT NOT NULL,
    productname NVARCHAR(200) NOT NULL,
    price DECIMAL(12,2) NOT NULL CHECK (price >= 0),
    salepercent DECIMAL(5,2) NOT NULL DEFAULT 0 CHECK (salepercent BETWEEN 0 AND 100),
    productnumber NVARCHAR(100) NULL,
    size INT NOT NULL DEFAULT 0,
    quantity INT NOT NULL CHECK (quantity > 0),
    image NVARCHAR(MAX) NULL
);
CREATE INDEX order_items_order ON dbo.order_items (orderid);
GO

-- Reviews
CREATE TABLE dbo.reviews (
    reviewid INT IDENTITY(1,1) PRIMARY KEY,
    userid INT NOT NULL REFERENCES dbo.users(userid) ON DELETE CASCADE,
    productid INT NOT NULL REFERENCES dbo.products(productid) ON DELETE CASCADE,
    rating INT NOT NULL CHECK (rating BETWEEN 1 AND 5),
    comment NVARCHAR(MAX) NOT NULL,
    createdat DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_reviews_user_product UNIQUE (userid, productid)
);
GO

-- 3. INSERT SEED DATA

-- Categories
SET IDENTITY_INSERT dbo.categories ON;
INSERT INTO dbo.categories (categoryid, categoryname) VALUES 
(1, N'Sneakers'), 
(2, N'Lifestyles/Casual'), 
(3, N'Sports'), 
(4, N'Boots');
SET IDENTITY_INSERT dbo.categories OFF;
GO

-- Users
SET IDENTITY_INSERT dbo.users ON;
INSERT INTO dbo.users (userid, username, password, surname, name, email, phone, quequan, address1, address2, postal, city, country, createdat) VALUES 
(1, N'admin', N'PBKDF2-SHA256$100000$jDhA+0IB6ux6lreHiqBPHA==$jyF2iS96150kDb9LNC33AITMcTiI2qWreg0X6EOgIVA=', N'IT Shoes', N'Admin', N'admin@example.com', NULL, NULL, NULL, NULL, NULL, NULL, N'Vietnam', GETDATE()),
(3, N'friedwc', N'PBKDF2-SHA256$100000$gNj9o4WHlwX/06xEtb4rtA==$gUX6ZXbSV1rmQ1s/DDKtvaGwLWXalAAyIyRGzl4eWbQ=', N'Đức', N'Đức', N'a@gmail.com', N'0123456789', N'Vĩnh Phúc', N'abc', N'abc', N'12', N'Ha Noi', N'Viet Nam', GETDATE());
SET IDENTITY_INSERT dbo.users OFF;
GO

-- Products
SET IDENTITY_INSERT dbo.products ON;
INSERT INTO dbo.products (productid, productname, price, description, image, stock, categoryid, image1, image2, image3, image4, gender, size, salepercent) VALUES
(1, N'Men Sneakers 01', 79.9, N'San pham mau IT Shoes - Men Sneakers 01', N'~/Assets/images/Men/shoes_man_1.png', 50, 1, N'~/Assets/images/Men/anh1_back.png', N'~/Assets/images/Men/anh1_left.jpg', N'~/Assets/images/Men/anh1_top.png', N'~/Assets/images/Men/anh1_under.png', N'Men', N'36,37,38,39,40,41,42,43', 0),
(2, N'Men Casual 02', 129.0, N'San pham mau IT Shoes - Men Casual 02', N'~/Assets/images/Men/shoes_man_2.png', 50, 2, N'~/Assets/images/Men/anh2_back.png', N'~/Assets/images/Men/anh2_left.png', N'~/Assets/images/Men/anh2_top.png', N'~/Assets/images/Men/anh2_under.png', N'Men', N'36,37,38,39,40,41,42,43', 0),
(3, N'Men Sports 03', 299.0, N'San pham mau IT Shoes - Men Sports 03', N'~/Assets/images/Men/shoes_man_3.png', 50, 3, N'~/Assets/images/Men/anh3_back.png', N'~/Assets/images/Men/anh3_left.png', N'~/Assets/images/Men/anh3_top.png', N'~/Assets/images/Men/anh3_under.png', N'Men', N'36,37,38,39,40,41,42,43', 20),
(4, N'Men Boots 04', 549.0, N'San pham mau IT Shoes - Men Boots 04', N'~/Assets/images/Men/shoes_man_4.png', 50, 4, N'~/Assets/images/Men/anh4_back.png', N'~/Assets/images/Men/anh4_left.png', N'~/Assets/images/Men/anh4_top.png', N'~/Assets/images/Men/anh4_under.png', N'Men', N'36,37,38,39,40,41,42,43', 0),
(5, N'Men Sneakers 05', 79.9, N'San pham mau IT Shoes - Men Sneakers 05', N'~/Assets/images/Men/shoes_man_5.png', 50, 1, N'~/Assets/images/Men/anh5_back.png', N'~/Assets/images/Men/anh5_left.png', N'~/Assets/images/Men/anh5_top.png', N'~/Assets/images/Men/anh5_under.png', N'Men', N'36,37,38,39,40,41,42,43', 0),
(6, N'Men Casual 06', 129.0, N'San pham mau IT Shoes - Men Casual 06', N'~/Assets/images/Men/shoes_man_6.png', 50, 2, N'~/Assets/images/Men/anh6_back.png', N'~/Assets/images/Men/anh6_left.png', N'~/Assets/images/Men/anh6_top.png', N'~/Assets/images/Men/anh6_under.png', N'Men', N'36,37,38,39,40,41,42,43', 20),
(7, N'Men Sports 07', 299.0, N'San pham mau IT Shoes - Men Sports 07', N'~/Assets/images/Men/shoes_man_7.png', 50, 3, N'~/Assets/images/Men/anh7_back.png', N'~/Assets/images/Men/anh7_left.png', N'~/Assets/images/Men/anh7_top.png', N'~/Assets/images/Men/anh7_under.png', N'Men', N'36,37,38,39,40,41,42,43', 0),
(8, N'Men Boots 08', 549.0, N'San pham mau IT Shoes - Men Boots 08', N'~/Assets/images/Men/shoes_man_8.png', 50, 4, N'~/Assets/images/Men/anh8_back.png', N'~/Assets/images/Men/anh8_left.png', N'~/Assets/images/Men/anh8_top.png', N'~/Assets/images/Men/anh8_under.png', N'Men', N'36,37,38,39,40,41,42,43', 0),
(9, N'Men Sneakers 09', 79.9, N'San pham mau IT Shoes - Men Sneakers 09', N'~/Assets/images/Men/shoes_man_9.png', 50, 1, N'~/Assets/images/Men/anh9_back.png', N'~/Assets/images/Men/anh9_left.png', N'~/Assets/images/Men/anh9_top.png', N'~/Assets/images/Men/anh9_under.png', N'Men', N'36,37,38,39,40,41,42,43', 20),
(10, N'Men Casual 10', 129.0, N'San pham mau IT Shoes - Men Casual 10', N'~/Assets/images/Men/shoes_man_10.png', 50, 2, N'~/Assets/images/Men/anh10_back.png', N'~/Assets/images/Men/anh10_left.png', N'~/Assets/images/Men/anh10_top.png', N'~/Assets/images/Men/anh10_under.png', N'Men', N'36,37,38,39,40,41,42,43', 0),
(11, N'Men Sports 11', 299.0, N'San pham mau IT Shoes - Men Sports 11', N'~/Assets/images/Men/shoes_man_11.png', 50, 3, N'~/Assets/images/Men/anh11_back.png', N'~/Assets/images/Men/anh11_left.png', N'~/Assets/images/Men/anh11_top.png', N'~/Assets/images/Men/anh11_under.png', N'Men', N'36,37,38,39,40,41,42,43', 0),
(12, N'Men Boots 12', 549.0, N'San pham mau IT Shoes - Men Boots 12', N'~/Assets/images/Men/shoes_man_12.png', 50, 4, N'~/Assets/images/Men/anh12_back.png', N'~/Assets/images/Men/anh12_left.png', N'~/Assets/images/Men/anh12_top.png', N'~/Assets/images/Men/anh12_under.png', N'Men', N'36,37,38,39,40,41,42,43', 20),
(13, N'Men Sneakers 13', 79.9, N'San pham mau IT Shoes - Men Sneakers 13', N'~/Assets/images/Men/shoes_man_13.png', 50, 1, N'~/Assets/images/Men/anh13_back.png', N'~/Assets/images/Men/anh13_left.png', N'~/Assets/images/Men/anh13_top.png', N'~/Assets/images/Men/anh13_under.png', N'Men', N'36,37,38,39,40,41,42,43', 0),
(14, N'Men Casual 14', 129.0, N'San pham mau IT Shoes - Men Casual 14', N'~/Assets/images/Men/shoes_man_14.png', 50, 2, N'~/Assets/images/Men/anh14_back.png', N'~/Assets/images/Men/anh14_left.png', N'~/Assets/images/Men/anh14_top.png', N'~/Assets/images/Men/anh14_under.png', N'Men', N'36,37,38,39,40,41,42,43', 0),
(15, N'Men Sports 15', 299.0, N'San pham mau IT Shoes - Men Sports 15', N'~/Assets/images/Men/shoes_man_15.png', 50, 3, N'~/Assets/images/Men/anh15_back.png', N'~/Assets/images/Men/anh15_left.png', N'~/Assets/images/Men/anh15_top.png', N'~/Assets/images/Men/anh15_under.png', N'Men', N'36,37,38,39,40,41,42,43', 20),
(16, N'Men Boots 16', 549.0, N'San pham mau IT Shoes - Men Boots 16', N'~/Assets/images/Men/shoes_man_16.png', 50, 4, N'~/Assets/images/Men/anh16_back.png', N'~/Assets/images/Men/anh16_left.png', N'~/Assets/images/Men/anh16_top.png', N'~/Assets/images/Men/anh16_under.png', N'Men', N'36,37,38,39,40,41,42,43', 0),
(17, N'Men Sneakers 17', 79.9, N'San pham mau IT Shoes - Men Sneakers 17', N'~/Assets/images/Men/shoes_man_17.png', 50, 1, N'~/Assets/images/Men/anh17_back.png', N'~/Assets/images/Men/anh17_left.png', N'~/Assets/images/Men/anh17_top.png', N'~/Assets/images/Men/anh17_under.png', N'Men', N'36,37,38,39,40,41,42,43', 0),
(18, N'Men Casual 18', 129.0, N'San pham mau IT Shoes - Men Casual 18', N'~/Assets/images/Men/shoes_man_18.png', 50, 2, N'~/Assets/images/Men/anh18_back.png', N'~/Assets/images/Men/anh18_left.png', N'~/Assets/images/Men/anh18_top.png', N'~/Assets/images/Men/anh18_under.png', N'Men', N'36,37,38,39,40,41,42,43', 20),
(19, N'Women Sneakers 01', 79.9, N'San pham mau IT Shoes - Women Sneakers 01', N'~/Assets/images/Women/shoes_women1.png', 50, 1, N'~/Assets/images/Women/anh1_back.png', N'~/Assets/images/Women/anh1_left.png', N'~/Assets/images/Women/anh1_top.png', N'~/Assets/images/Women/anh1_under.png', N'Women', N'36,37,38,39,40,41,42,43', 0),
(20, N'Women Casual 02', 129.0, N'San pham mau IT Shoes - Women Casual 02', N'~/Assets/images/Women/shoes_women2.png', 50, 2, N'~/Assets/images/Women/anh2_back.png', N'~/Assets/images/Women/anh2_left.png', N'~/Assets/images/Women/anh2_top.png', N'~/Assets/images/Women/anh2_under.png', N'Women', N'36,37,38,39,40,41,42,43', 0),
(21, N'Women Sports 03', 299.0, N'San pham mau IT Shoes - Women Sports 03', N'~/Assets/images/Women/shoes_women3.png', 50, 3, N'~/Assets/images/Women/anh3_back.png', N'~/Assets/images/Women/anh3_left.png', N'~/Assets/images/Women/anh3_top.png', N'~/Assets/images/Women/anh3_under.png', N'Women', N'36,37,38,39,40,41,42,43', 20),
(22, N'Women Boots 04', 549.0, N'San pham mau IT Shoes - Women Boots 04', N'~/Assets/images/Women/shoes_women4.png', 50, 4, N'~/Assets/images/Women/anh4_back.png', N'~/Assets/images/Women/anh4_left.png', N'~/Assets/images/Women/anh4_top.png', N'~/Assets/images/Women/anh4_under.png', N'Women', N'36,37,38,39,40,41,42,43', 0),
(23, N'Women Sneakers 05', 79.9, N'San pham mau IT Shoes - Women Sneakers 05', N'~/Assets/images/Women/shoes_women5.png', 50, 1, N'~/Assets/images/Women/anh5_back.png', N'~/Assets/images/Women/anh5_left.png', N'~/Assets/images/Women/anh5_top.png', N'~/Assets/images/Women/anh5_under.png', N'Women', N'36,37,38,39,40,41,42,43', 0),
(24, N'Women Casual 06', 129.0, N'San pham mau IT Shoes - Women Casual 06', N'~/Assets/images/Women/shoes_women6.png', 50, 2, N'~/Assets/images/Women/anh6_back.png', N'~/Assets/images/Women/anh6_left.png', N'~/Assets/images/Women/anh6_top.png', N'~/Assets/images/Women/anh6_under.png', N'Women', N'36,37,38,39,40,41,42,43', 20),
(25, N'Women Sports 07', 299.0, N'San pham mau IT Shoes - Women Sports 07', N'~/Assets/images/Women/shoes_women7.png', 50, 3, N'~/Assets/images/Women/anh7_back.png', N'~/Assets/images/Women/anh7_left.png', N'~/Assets/images/Women/anh7_top.png', N'~/Assets/images/Women/anh7_under.png', N'Women', N'36,37,38,39,40,41,42,43', 0),
(26, N'Women Boots 08', 549.0, N'San pham mau IT Shoes - Women Boots 08', N'~/Assets/images/Women/shoes_women8.png', 50, 4, N'~/Assets/images/Women/anh8_back.png', N'~/Assets/images/Women/anh8_left.png', N'~/Assets/images/Women/anh8_top.png', N'~/Assets/images/Women/anh8_under.png', N'Women', N'36,37,38,39,40,41,42,43', 0),
(27, N'Women Sneakers 09', 79.9, N'San pham mau IT Shoes - Women Sneakers 09', N'~/Assets/images/Women/shoes_women9.png', 50, 1, N'~/Assets/images/Women/anh9_back.png', N'~/Assets/images/Women/anh9_left.png', N'~/Assets/images/Women/anh9_top.png', N'~/Assets/images/Women/anh9_under.png', N'Women', N'36,37,38,39,40,41,42,43', 20),
(28, N'Women Casual 10', 129.0, N'San pham mau IT Shoes - Women Casual 10', N'~/Assets/images/Women/shoes_women10.png', 50, 2, N'~/Assets/images/Women/anh10_back.png', N'~/Assets/images/Women/anh10_left.png', N'~/Assets/images/Women/anh10_top.png', N'~/Assets/images/Women/anh10_under.png', N'Women', N'36,37,38,39,40,41,42,43', 0),
(29, N'Women Sports 11', 299.0, N'San pham mau IT Shoes - Women Sports 11', N'~/Assets/images/Women/shoes_women11.png', 50, 3, N'~/Assets/images/Women/anh11_back.png', N'~/Assets/images/Women/anh11_left.png', N'~/Assets/images/Women/anh11_top.png', N'~/Assets/images/Women/anh11_under.png', N'Women', N'36,37,38,39,40,41,42,43', 0),
(30, N'Women Boots 12', 549.0, N'San pham mau IT Shoes - Women Boots 12', N'~/Assets/images/Women/shoes_women12.png', 50, 4, N'~/Assets/images/Women/anh12_back.png', N'~/Assets/images/Women/anh12_left.png', N'~/Assets/images/Women/anh12_top.png', N'~/Assets/images/Women/anh12_under.png', N'Women', N'36,37,38,39,40,41,42,43', 20),
(31, N'Women Sneakers 13', 79.9, N'San pham mau IT Shoes - Women Sneakers 13', N'~/Assets/images/Women/shoes_women13.png', 50, 1, N'~/Assets/images/Women/shoes_women13.png', N'~/Assets/images/Women/shoes_women13.png', N'~/Assets/images/Women/shoes_women13.png', N'~/Assets/images/Women/shoes_women13.png', N'Women', N'36,37,38,39,40,41,42,43', 0),
(32, N'Kids Sneakers 01', 79.9, N'San pham mau IT Shoes - Kids Sneakers 01', N'~/Assets/images/Kids/shoes_kid1.png', 50, 1, N'~/Assets/images/Kids/anh1_back.png', N'~/Assets/images/Kids/anh1_left.png', N'~/Assets/images/Kids/anh1_top.png', N'~/Assets/images/Kids/anh1_under.png', N'Kids', N'36,37,38,39,40,41,42,43', 0),
(33, N'Kids Casual 02', 129.0, N'San pham mau IT Shoes - Kids Casual 02', N'~/Assets/images/Kids/shoes_kid2.png', 50, 2, N'~/Assets/images/Kids/anh2_back.png', N'~/Assets/images/Kids/anh2_left.png', N'~/Assets/images/Kids/anh2_top.png', N'~/Assets/images/Kids/anh2_under.png', N'Kids', N'36,37,38,39,40,41,42,43', 0),
(34, N'Kids Sports 03', 299.0, N'San pham mau IT Shoes - Kids Sports 03', N'~/Assets/images/Kids/shoes_kid3.png', 50, 3, N'~/Assets/images/Kids/anh3_back.png', N'~/Assets/images/Kids/anh3_left.png', N'~/Assets/images/Kids/anh3_top.png', N'~/Assets/images/Kids/anh3_under.png', N'Kids', N'36,37,38,39,40,41,42,43', 20),
(35, N'Kids Boots 04', 549.0, N'San pham mau IT Shoes - Kids Boots 04', N'~/Assets/images/Kids/shoes_kid4.png', 50, 4, N'~/Assets/images/Kids/anh4_back.png', N'~/Assets/images/Kids/anh4_left.png', N'~/Assets/images/Kids/anh4_top.png', N'~/Assets/images/Kids/anh4_under.png', N'Kids', N'36,37,38,39,40,41,42,43', 0),
(36, N'Kids Sneakers 05', 79.9, N'San pham mau IT Shoes - Kids Sneakers 05', N'~/Assets/images/Kids/shoes_kid5.png', 50, 1, N'~/Assets/images/Kids/anh5_back.png', N'~/Assets/images/Kids/anh5_left.png', N'~/Assets/images/Kids/anh5_top.png', N'~/Assets/images/Kids/anh5_under.png', N'Kids', N'36,37,38,39,40,41,42,43', 0),
(37, N'Kids Casual 06', 129.0, N'San pham mau IT Shoes - Kids Casual 06', N'~/Assets/images/Kids/shoes_kid6.png', 50, 2, N'~/Assets/images/Kids/anh6_back.png', N'~/Assets/images/Kids/anh6_left.png', N'~/Assets/images/Kids/anh6_top.png', N'~/Assets/images/Kids/anh6_under.png', N'Kids', N'36,37,38,39,40,41,42,43', 20),
(38, N'Kids Sports 07', 299.0, N'San pham mau IT Shoes - Kids Sports 07', N'~/Assets/images/Kids/shoes_kid7.png', 50, 3, N'~/Assets/images/Kids/anh7_back.png', N'~/Assets/images/Kids/anh7_left.png', N'~/Assets/images/Kids/anh7_top.png', N'~/Assets/images/Kids/anh7_under.png', N'Kids', N'36,37,38,39,40,41,42,43', 0),
(39, N'Kids Boots 08', 549.0, N'San pham mau IT Shoes - Kids Boots 08', N'~/Assets/images/Kids/shoes_kid8.png', 50, 4, N'~/Assets/images/Kids/anh8_back.png', N'~/Assets/images/Kids/anh8_left.png', N'~/Assets/images/Kids/anh8_top.png', N'~/Assets/images/Kids/anh8_under.png', N'Kids', N'36,37,38,39,40,41,42,43', 0),
(40, N'Kids Sneakers 09', 79.9, N'San pham mau IT Shoes - Kids Sneakers 09', N'~/Assets/images/Kids/shoes_kid9.png', 50, 1, N'~/Assets/images/Kids/anh9_back.png', N'~/Assets/images/Kids/anh9_left.png', N'~/Assets/images/Kids/anh9_top.png', N'~/Assets/images/Kids/anh9_under.png', N'Kids', N'36,37,38,39,40,41,42,43', 20);
SET IDENTITY_INSERT dbo.products OFF;
GO

-- Orders
SET IDENTITY_INSERT dbo.orders ON;
INSERT INTO dbo.orders (orderid, ordernumber, userid, username, orderdate, totalamount, address, city, postal, paymentmethod) VALUES
(1, N'ORD-20261117-fc7b8ee9aa4c415780d2f1eccd26e10a', 3, N'friedwc', '2026-11-17 22:49:41', 312.71, N'abc', N'Ha Noi', N'12', N'COD'),
(2, N'ORD-20261117-c27fa1585de6455da192b1e6459e8c3d', 3, N'friedwc', '2026-11-17 22:56:25', 73.51, N'abc', N'Ha Noi', N'12', N'COD'),
(3, N'ORD-20261117-1accca753c8e4ed4848df9b4cafdba5b', 3, N'friedwc', '2026-11-17 23:09:26', 448.10, N'abc', N'Ha Noi', N'12', N'COD'),
(4, N'ORD-20261117-20a67bc3097441238044f290ec0f8e59', 3, N'friedwc', '2026-11-17 23:10:26', 103.20, N'abc', N'Ha Noi', N'12', N'COD'),
(5, N'ORD-20261117-512507dd365f411b8c1bed450fe44475', 3, N'friedwc', '2026-11-17 23:11:06', 577.10, N'abc', N'Ha Noi', N'12', N'COD');
SET IDENTITY_INSERT dbo.orders OFF;
GO

-- Order Items
SET IDENTITY_INSERT dbo.order_items ON;
INSERT INTO dbo.order_items (orderitemid, orderid, productid, productname, price, salepercent, productnumber, size, quantity, image) VALUES
(1, 1, 1, N'Men Sneakers 01', 79.90, 8.00, NULL, 36, 1, N'~/Assets/images/Men/shoes_man_1.png'),
(2, 1, 3, N'Men Sports 03', 299.00, 20.00, NULL, 43, 1, N'~/Assets/images/Men/shoes_man_3.png'),
(3, 2, 1, N'Men Sneakers 01', 79.90, 8.00, NULL, 36, 1, N'~/Assets/images/Men/shoes_man_1.png'),
(4, 3, 2, N'Men Casual 02', 129.00, 0.00, NULL, 38, 1, N'~/Assets/images/Men/shoes_man_2.png'),
(5, 3, 19, N'Women Sneakers 01', 79.90, 0.00, NULL, 43, 1, N'~/Assets/images/Women/shoes_women1.png'),
(6, 3, 34, N'Kids Sports 03', 299.00, 20.00, NULL, 43, 1, N'~/Assets/images/Kids/shoes_kid3.png'),
(7, 4, 24, N'Women Casual 06', 129.00, 20.00, NULL, 39, 1, N'~/Assets/images/Women/shoes_women6.png'),
(8, 5, 32, N'Kids Sneakers 01', 79.90, 0.00, NULL, 41, 1, N'~/Assets/images/Kids/shoes_kid1.png'),
(9, 5, 20, N'Women Casual 02', 129.00, 0.00, NULL, 39, 1, N'~/Assets/images/Women/shoes_women2.png'),
(10, 5, 33, N'Kids Casual 02', 129.00, 0.00, NULL, 42, 1, N'~/Assets/images/Kids/shoes_kid2.png'),
(11, 5, 3, N'Men Sports 03', 299.00, 20.00, NULL, 41, 1, N'~/Assets/images/Men/shoes_man_3.png');
SET IDENTITY_INSERT dbo.order_items OFF;
GO

PRINT N'IT_Shoes_DB initialized successfully for SQL Server!';
