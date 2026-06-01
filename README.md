# 環境構築手順

## Visual Studio のインストール

### インストール

- Visual Studio 2026 Community
- 参考URL
  - https://learn.microsoft.com/ja-jp/visualstudio/install/install-visual-studio?view=visualstudio

### インストール時のオプション

#### ASP.NET と Web 開発

- ASP.NET と Web 開発
  - ☑ .NET Framework プロジェクトと項目テンプレート

#### .NET デスクトップ開発

- ☑ .NET デスクトップ開発

#### データ関連

- ☑ データの保存と処理

> ※ インストールに1時間程度かかる場合があります

### プロジェクト作成

新しいプロジェクトを作成し、以下を選択します。

- ASP.NET Web Application (.NET Framework)

---

## SQL Server のインストール

### ダウンロード

- 公式サイト
  - https://www.microsoft.com/ja-jp/sql-server/sql-server-downloads

### インストール設定

| 項目 | 設定値 |
|--------|--------|
| Install Type | Basic |
| Install Location | Default |

### SSMS インストール

- SQL Server Management Studio (SSMS 22)

---

## SQL Server の設定

### 1. SQL Server 認証を有効化

SSMS から SQL Server に接続し、以下を設定します。

1. サーバーを右クリック
2. 「プロパティ」
3. 「セキュリティ」
4. 「サーバー認証」

以下を選択

- SQL Server 認証モードと Windows 認証モード

---

### 2. sa ユーザーの設定

1. セキュリティ
2. ログイン
3. sa
4. プロパティ

#### パスワード設定

- 任意のパスワードを設定

#### 状態タブ

- ログイン：有効

---

### SQL Server の再起動

タスクマネージャー → サービス

- `SQLEXPRESS`

を再起動

---

### 接続確認

```cmd
sqlcmd -S localhost\SQLEXPRESS01 -U sa -P sa
```

---

## Git

リポジトリ

- https://github.com/t-koudu/WebApplication1

---

# データベース構築

## DB作成

```sql
CREATE DATABASE SampleDB;
GO
```

---

## ユーザー作成

### ログインユーザー

- ユーザー名：appUser
- パスワード：appUser123

```sql
CREATE LOGIN appUser
WITH PASSWORD = 'appUser123';
GO
```

---

### データベースユーザー作成

```sql
USE SampleDB;
GO

CREATE USER appUser
FOR LOGIN appUser;
GO
```

### 権限付与

```sql
ALTER ROLE db_owner
ADD MEMBER appUser;
GO
```

---

### 接続確認

```cmd
sqlcmd -S localhost\SQLEXPRESS01 -U appUser -P appUser123 -d SampleDB
```

---

# テーブル構成

## ER構成

- 顧客マスタ（Customers）
- 商品マスタ（Products）
- 受注ヘッダ（OrderHeaders）
- 受注明細（OrderDetails）

---

## 顧客マスタ

```sql
CREATE TABLE Customers (
    CustomerId      INT IDENTITY(1,1) PRIMARY KEY,
    CustomerCode    VARCHAR(10) NOT NULL UNIQUE,
    CustomerName    NVARCHAR(100) NOT NULL,
    Address1        NVARCHAR(200),
    Tel             VARCHAR(20),
    CreatedAt       DATETIME2 NOT NULL DEFAULT GETDATE()
);
```

---

## 商品マスタ

```sql
CREATE TABLE Products (
    ProductId       INT IDENTITY(1,1) PRIMARY KEY,
    ProductCode     VARCHAR(20) NOT NULL UNIQUE,
    ProductName     NVARCHAR(100) NOT NULL,
    UnitPrice       DECIMAL(10,2) NOT NULL,
    CreatedAt       DATETIME2 NOT NULL DEFAULT GETDATE()
);
```

---

## 受注ヘッダ

```sql
CREATE TABLE OrderHeaders (
    OrderId         INT IDENTITY(1,1) PRIMARY KEY,
    OrderNo         VARCHAR(20) NOT NULL UNIQUE,
    CustomerId      INT NOT NULL,
    OrderDate       DATE NOT NULL,
    CreatedAt       DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_OrderHeaders_Customers
        FOREIGN KEY(CustomerId)
        REFERENCES Customers(CustomerId)
);
```

---

## 受注明細

```sql
CREATE TABLE OrderDetails (
    OrderDetailId   INT IDENTITY(1,1) PRIMARY KEY,
    OrderId         INT NOT NULL,
    ProductId       INT NOT NULL,
    Quantity        INT NOT NULL,
    UnitPrice       DECIMAL(10,2) NOT NULL,

    CONSTRAINT FK_OrderDetails_OrderHeaders
        FOREIGN KEY(OrderId)
        REFERENCES OrderHeaders(OrderId),

    CONSTRAINT FK_OrderDetails_Products
        FOREIGN KEY(ProductId)
        REFERENCES Products(ProductId)
);
```

---

# テストデータ

## 顧客マスタ

```sql
INSERT INTO Customers
(CustomerCode, CustomerName, Address1, Tel)
VALUES
('C001', N'株式会社ABC', N'東京都千代田区1-1-1', '03-1111-1111'),
('C002', N'株式会社XYZ', N'東京都新宿区2-2-2', '03-2222-2222'),
('C003', N'テスト商事', N'東京都渋谷区3-3-3', '03-3333-3333');
```

---

## 商品マスタ

```sql
INSERT INTO Products
(ProductCode, ProductName, UnitPrice)
VALUES
('P001', N'ノートPC', 120000),
('P002', N'モニター', 35000),
('P003', N'キーボード', 5000),
('P004', N'マウス', 2000),
('P005', N'プリンター', 45000);
```

---

## 受注ヘッダ

```sql
INSERT INTO OrderHeaders
(OrderNo, CustomerId, OrderDate)
VALUES
('ORD0001', 1, '2026-06-01'),
('ORD0002', 2, '2026-06-02'),
('ORD0003', 3, '2026-06-03');
```

---

## 受注明細

```sql
INSERT INTO OrderDetails
(OrderId, ProductId, Quantity, UnitPrice)
VALUES
(1, 1, 2, 120000),
(1, 2, 3, 35000),
(1, 3, 5, 5000),
(2, 1, 1, 120000),
(2, 4, 10, 2000),
(3, 5, 2, 45000);
```

---

# サンプル帳票出力SQL

```sql
SELECT
    oh.OrderNo,
    oh.OrderDate,
    c.CustomerCode,
    c.CustomerName,
    c.Address1,
    p.ProductCode,
    p.ProductName,
    od.Quantity,
    od.UnitPrice,
    od.Quantity * od.UnitPrice AS Amount
FROM OrderHeaders oh
INNER JOIN Customers c
    ON c.CustomerId = oh.CustomerId
INNER JOIN OrderDetails od
    ON od.OrderId = oh.OrderId
INNER JOIN Products p
    ON p.ProductId = od.ProductId
WHERE oh.OrderNo = 'ORD0001'
ORDER BY od.OrderDetailId;
```
