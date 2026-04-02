# Quan-ly-trung-tam-ngoai-ngu

Frontend skeleton cho do an "Xay dung he thong website quan ly trung tam ngoai ngu" bang ASP.NET Core MVC (.NET 8).

## Da hoan thanh

- Public pages:
  - Trang chu
  - Gioi thieu trung tam
  - Danh sach khoa hoc
  - Chi tiet khoa hoc
  - Danh sach lop hoc dang mo
  - Tin tuc / bai viet
  - Chi tiet bai viet
  - Lien he
  - Dang nhap
  - Dang ky
  - Quen mat khau
  - Ho so ca nhan
- Dashboard theo vai tro:
  - Admin
  - Staff / Giao vu
  - Teacher
- Khu vuc quan tri da co skeleton module:
  - Tai khoan
  - Hoc vien
  - Giao vien
  - Khoa hoc
  - Lop hoc
  - Ghi danh
  - Hoc phi / Bien nhan
  - Buoi hoc
  - Diem danh
  - Diem so
  - Bao cao / thong ke
- Shared UI:
  - Sidebar dashboard
  - Breadcrumb
  - Table + pagination mock
  - Form mock
  - Detail page mock
  - Toast / modal xac nhan
  - Chart.js mock

## Cau truc chinh

- `Quan-ly-trung-tam-ngoai-ngu/Controllers`
  - Public controllers
- `Quan-ly-trung-tam-ngoai-ngu/Areas/Admin`
  - Dashboard va cac module admin
- `Quan-ly-trung-tam-ngoai-ngu/Areas/Staff`
  - Dashboard va module giao vu
- `Quan-ly-trung-tam-ngoai-ngu/Areas/Teacher`
  - Dashboard va module giao vien
- `Quan-ly-trung-tam-ngoai-ngu/Services/Mocks`
  - `MockDataService`
  - `DemoAuthService`
- `Quan-ly-trung-tam-ngoai-ngu/ViewModels`
  - ViewModels cho public, dashboard, modules
- `Quan-ly-trung-tam-ngoai-ngu/Views/Shared`
  - Layout, partials, module views dung chung

## Tai khoan demo

- `admin@demo.com` / `123456`
- `staff@demo.com` / `123456`
- `teacher@demo.com` / `123456`

## Trang thai hien tai

- Du lieu dang dung `MockDataService`, chua ket noi SQL Server.
- Dang nhap dang dung session mock, chua dung Identity.
- Cac form CRUD hien tai phuc vu demo giao dien, chua luu du lieu that.
- Nhieu cho da duoc danh dau theo tinh than:
  - `// TODO: connect database later`

## Huong noi database sau

1. Thay `MockDataService` bang service/repository that.
2. Noi `Account`, `Student`, `Teacher`, `Course`, `Class`, `Enrollment`, `Receipt`, `Attendance`, `ExamResult` vao EF Core.
3. Thay mock login bang auth/authorization that.
4. Bien cac form CRUD thanh POST/PUT/DELETE xu ly du lieu that.

## Chay project

```bash
dotnet build .\Quan-ly-trung-tam-ngoai-ngu\Quan-ly-trung-tam-ngoai-ngu.csproj
dotnet run --project .\Quan-ly-trung-tam-ngoai-ngu\Quan-ly-trung-tam-ngoai-ngu.csproj
```
