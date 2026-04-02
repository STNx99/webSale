# webSale Railway Guide

Hướng dẫn deploy Railway-only cho app và MySQL.

## 1) Railway MySQL

Tạo file env Railway từ mẫu:

- `.env.railway.example` -> copy thành biến môi trường trong Railway

Biến cần thiết trên Railway:

```env
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080
MYSQL_ROOT_PASSWORD=replace_with_your_railway_mysql_password
MYSQL_PUBLIC_URL=mysql://root:${MYSQL_ROOT_PASSWORD}@${RAILWAY_TCP_PROXY_DOMAIN}:${RAILWAY_TCP_PROXY_PORT}/railway
```

App sẽ đọc trực tiếp `MYSQL_PUBLIC_URL` nếu bạn cấu hình theo kiểu Railway.
Nếu bạn muốn dùng URL đầy đủ theo biến môi trường khác, app cũng hỗ trợ `MYSQL_URL`:

```env
MYSQL_URL=mysql://root:password@host:3306/railway
```

## 2) Import schema lên Railway

File schema đã tạo sẵn:

- `railway-schema.sql`

Chạy script import:

```powershell
$env:MYSQL_ROOT_PASSWORD="your-railway-password"
$env:MYSQL_PUBLIC_URL="mysql://root:$env:MYSQL_ROOT_PASSWORD@your-host:3306/railway"

powershell -ExecutionPolicy Bypass -File scripts/push-railway-schema.ps1
```

## 3) Build và push Docker image

Build image:

```powershell
docker build -t stnx99/websale:latest .
```

Push lên Docker Hub:

```powershell
docker push stnx99/websale:latest
```

## Notes

- App đã cấu hình MySQL only, không dùng SQL Server nữa.
- Seed roles, admin user và mock catalog sẽ chạy khi app khởi động.
- Railway sẽ cần `MYSQL_PUBLIC_URL` hoặc bộ `MYSQLHOST/MYSQLPORT/MYSQLDATABASE/MYSQLUSER/MYSQLPASSWORD`.
