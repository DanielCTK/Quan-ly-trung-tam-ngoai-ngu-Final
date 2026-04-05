# STAGE_2_AUDIT_REPORT

## 1. Tổng quan

- Mục tiêu audit: kiểm tra toàn bộ source hiện tại theo chuẩn giai đoạn 2 của đề tài "website quản lý trung tâm ngoại ngữ", không giả định code cũ đã đúng.
- Phạm vi kiểm tra:
  - kiến trúc ASP.NET Core MVC
  - tích hợp SQL Server + EF Core
  - authentication + authorization theo bảng `Accounts`
  - CRUD cho các module chính
  - dashboard lấy dữ liệu thật
  - luồng demo cuối kỳ
  - regression về route/layout chính
- Nguồn sự thật dùng để đối chiếu:
  - source trong workspace hiện tại
  - database thật `LanguageCenterDB`
  - file script SQL mới người dùng cung cấp: [script.ipynb](C:/Users/ADMIN/Documents/script.ipynb)
- Trạng thái tổng thể sau audit: project đã được đưa từ trạng thái "stage 2 chưa đạt chuẩn" sang trạng thái "đủ chạy stage 2 thật bằng SQL Server + EF Core + auth thật + demo flow end-to-end", nhưng vẫn còn một số nợ kỹ thuật kiến trúc cần ghi nhận trung thực.

## 2. Tóm tắt kết quả

- Phần đã đạt ngay từ đầu:
  - project ASP.NET Core MVC chạy được
  - UI/UX, area, dashboard, luồng màn hình và phần lớn view đã có sẵn
  - dữ liệu thật trong SQL Server đã tồn tại
  - nhiều CRUD và dashboard đã có logic đọc/ghi DB thật qua ADO.NET
- Phần thiếu/sai phát hiện ban đầu:
  - chưa dùng EF Core thật, chưa có `DbContext`
  - auth chưa dùng cookie/claims thật, còn phụ thuộc session + custom authorize
  - read service/auth service còn fallback mock khi DB lỗi
  - kiến trúc đang lẫn tên `MockDataService` dù đã đọc DB thật
  - transaction của module nhập điểm chưa tương thích execution strategy của SQL Server retry
  - link auth/logout trong layout public khi render trong area bị kế thừa area sai và dẫn tới 404
- Phần đã được bổ sung/sửa trong lần làm việc này:
  - thêm EF Core SQL Server và `ApplicationDbContext`
  - map entity đúng theo script SQL mới
  - chuyển DI sang service EF Core thật cho auth/read/write
  - login/logout bằng cookie authentication thật, vẫn giữ session để tương thích UI cũ
  - authorize đọc role từ claims thật, fallback session chỉ để tương thích
  - viết lại service CRUD EF Core cho Accounts, Students, Teachers, Courses, Classes, Enrollments, Receipts, Sessions, Attendances, ExamResults
  - viết lại read service dùng EF Core, bỏ fallback mock
  - sửa flow nhập điểm để chạy đúng với `SqlServerRetryingExecutionStrategy`
  - sửa route logout/login trong layout để không bị sai `asp-area`
- Mức độ hoàn thành cuối cùng:
  - theo nghiệp vụ giai đoạn 2 và khả năng demo: khoảng `95%`
  - theo chuẩn kiến trúc "sạch hoàn toàn" và coding requirements chặt nhất: chưa phải tuyệt đối `100%` do còn nợ kỹ thuật ở async/password hashing/tên service legacy

## 3. Ma trận đánh giá theo module

| Module | Trạng thái ban đầu | Vấn đề phát hiện | Hành động đã làm | Trạng thái cuối |
| ------ | ------------------ | ---------------- | ---------------- | --------------- |
| Auth | CẦN SỬA | Login đọc DB thật nhưng chưa dùng cookie auth; role check dựa session; logout area link có thể 404 | Thêm cookie auth, sign-in/sign-out bằng claims, sửa `DemoAuthorizeAttribute`, vá link logout/login area | DONE |
| Accounts | CẦN SỬA | CRUD dùng ADO.NET, chưa qua EF Core; chưa chuẩn kiến trúc stage 2 | Viết CRUD EF Core thật, giữ validation duplicate username/email/role | DONE |
| Students | CẦN SỬA | CRUD DB thật nhưng chưa qua EF Core, còn tên/service mock | Viết CRUD EF Core, giữ unique `StudentCode`, email, soft delete | DONE |
| Teachers | CẦN SỬA | CRUD DB thật nhưng chưa qua EF Core | Viết CRUD EF Core, giữ unique `TeacherCode`, email, soft delete | DONE |
| Courses | CẦN SỬA | CRUD DB thật nhưng chưa qua EF Core | Viết CRUD EF Core, validation `DurationHours`, `TuitionFee`, unique `CourseCode` | DONE |
| Classes | CẦN SỬA | CRUD DB thật nhưng chưa qua EF Core | Viết CRUD EF Core, validation course/teacher/date/capacity, soft delete | DONE |
| Enrollments | CẦN SỬA | CRUD DB thật nhưng chưa qua EF Core | Viết CRUD EF Core, chống trùng `(StudentId, ClassId)`, đọc `FinalFee` computed column | DONE |
| Receipts | CẦN SỬA | CRUD DB thật nhưng chưa qua EF Core | Viết CRUD EF Core, validate `PaymentMethod`, generate `ReceiptCode`, ghi DB thật | DONE |
| ClassSessions | CẦN SỬA | CRUD DB thật nhưng chưa qua EF Core | Viết CRUD EF Core, tạo/sửa/xóa session thật | DONE |
| Attendances | CẦN SỬA | CRUD DB thật nhưng chưa qua EF Core | Viết CRUD EF Core, check enrollment thuộc class session, update nếu record đã tồn tại | DONE |
| Exams | PARTIAL | Không có module exam độc lập theo kiểu tách riêng entity CRUD; đang gắn với luồng nhập điểm | Duy trì module `Exams` hiện có, lưu thật vào `Exams` + `ExamResults`, sửa transaction EF để flow chạy thật | PARTIAL |
| ExamResults | CẦN SỬA | CRUD DB thật nhưng chưa qua EF Core; transaction lỗi runtime khi bật retry strategy | Viết CRUD EF Core, auto `Pass/Fail`, fix execution strategy + transaction | DONE |
| Dashboard | CẦN SỬA | Dữ liệu lấy từ SQL thật nhưng qua service legacy/fallback mock | Viết `EfLanguageCenterReadService`, dashboard admin/staff/teacher đọc DB thật | DONE |
| Validation / Error handling | CẦN SỬA | Chưa có xử lý lỗi duplicate/fk nhất quán ở EF; runtime transaction bug ở exam | Thêm validation friendly trong service EF, bắt `DbUpdateException`, map duplicate/FK/not-null | DONE |

## 4. Những file đã tạo / sửa

### File tạo mới

- [ApplicationDbContext.cs](D:/ASP-24CNTT/Quan-ly-trung-tam-ngoai-ngu/Quan-ly-trung-tam-ngoai-ngu/Data/ApplicationDbContext.cs)
  - Tạo `DbContext` và entity mappings đúng theo schema SQL.
- [EfServiceMapper.cs](D:/ASP-24CNTT/Quan-ly-trung-tam-ngoai-ngu/Quan-ly-trung-tam-ngoai-ngu/Services/Ef/EfServiceMapper.cs)
  - Mapper và helper cho trạng thái, label, course metadata.
- [EfAuthService.cs](D:/ASP-24CNTT/Quan-ly-trung-tam-ngoai-ngu/Quan-ly-trung-tam-ngoai-ngu/Services/Ef/EfAuthService.cs)
  - Auth service dùng EF Core đọc bảng `Accounts`.
- [EfLanguageCenterReadService.cs](D:/ASP-24CNTT/Quan-ly-trung-tam-ngoai-ngu/Quan-ly-trung-tam-ngoai-ngu/Services/Ef/EfLanguageCenterReadService.cs)
  - Read service dùng EF Core cho dashboard/list/detail.
- [EfLanguageCenterManagementService.cs](D:/ASP-24CNTT/Quan-ly-trung-tam-ngoai-ngu/Quan-ly-trung-tam-ngoai-ngu/Services/Ef/EfLanguageCenterManagementService.cs)
  - CRUD service dùng EF Core cho các phân hệ stage 2.
- [STAGE_2_AUDIT_REPORT.md](D:/ASP-24CNTT/Quan-ly-trung-tam-ngoai-ngu/STAGE_2_AUDIT_REPORT.md)
  - Báo cáo audit tổng hợp.

### File sửa

- [Program.cs](D:/ASP-24CNTT/Quan-ly-trung-tam-ngoai-ngu/Quan-ly-trung-tam-ngoai-ngu/Program.cs)
  - Cấu hình `DbContext`, cookie auth, DI services EF.
- [Quan-ly-trung-tam-ngoai-ngu.csproj](D:/ASP-24CNTT/Quan-ly-trung-tam-ngoai-ngu/Quan-ly-trung-tam-ngoai-ngu/Quan-ly-trung-tam-ngoai-ngu.csproj)
  - Thêm package EF Core SQL Server/Design.
- [AccountController.cs](D:/ASP-24CNTT/Quan-ly-trung-tam-ngoai-ngu/Quan-ly-trung-tam-ngoai-ngu/Controllers/AccountController.cs)
  - Login/logout bằng cookie auth thật.
- [DemoAuthorizeAttribute.cs](D:/ASP-24CNTT/Quan-ly-trung-tam-ngoai-ngu/Quan-ly-trung-tam-ngoai-ngu/Infrastructure/DemoAuthorizeAttribute.cs)
  - Authorize theo claims role thật.
- [TeacherControllerBase.cs](D:/ASP-24CNTT/Quan-ly-trung-tam-ngoai-ngu/Quan-ly-trung-tam-ngoai-ngu/Areas/Teacher/Controllers/TeacherControllerBase.cs)
  - Lấy teacher hiện tại từ claims/session tương thích.
- [_Layout.cshtml](D:/ASP-24CNTT/Quan-ly-trung-tam-ngoai-ngu/Quan-ly-trung-tam-ngoai-ngu/Views/Shared/_Layout.cshtml)
  - Buộc link auth/logout về area rỗng để tránh 404.

## 5. Kiến trúc sau khi hoàn thiện

### DbContext

- `ApplicationDbContext` map đúng 11 bảng:
  - `Accounts`
  - `Students`
  - `Teachers`
  - `Courses`
  - `Classes`
  - `Enrollments`
  - `Receipts`
  - `ClassSessions`
  - `Attendances`
  - `Exams`
  - `ExamResults`
- Map các unique index và FK chính theo script SQL.
- `Enrollments.FinalFee` được map là computed column persisted.

### Entity models

- Entity tách riêng khỏi model/viewmodel UI.
- Length constraint đã được siết lại theo script SQL mới trong notebook.

### Services

- `EfAuthService`
  - đọc tài khoản từ `Accounts`
  - validate login theo `Username` hoặc `Email`
  - đăng ký học viên mới vào `Students`
- `EfLanguageCenterReadService`
  - cấp dữ liệu thật cho dashboard/list/detail
  - không fallback mock
- `EfLanguageCenterManagementService`
  - xử lý CRUD và validation nghiệp vụ
  - bắt lỗi duplicate/FK/not-null từ SQL Server

### Auth flow

- `AccountController.Login`
  - đọc account từ DB qua EF Core
  - tạo cookie auth với claims `NameIdentifier`, `Name`, `Email`, `Role`
  - đồng thời set session để không phá UI/layout cũ
- `AccountController.Logout`
  - sign-out cookie thật
  - clear session

### Role protection

- `DemoAuthorizeAttribute` giờ ưu tiên claims role thật từ cookie auth.
- Vẫn giữ fallback session để không làm vỡ luồng cũ.

### CRUD flow

- Controller giữ mỏng.
- Ghi/sửa/xóa đi qua `ILanguageCenterManagementService`.
- Dashboard/list/detail đọc qua `IMockDataService`, nhưng implementation runtime hiện là `EfLanguageCenterReadService` dùng DB thật.

### Dashboard flow

- Admin/Staff/Teacher dashboard hiện lấy số liệu thật từ DB.
- Không còn dùng seeded mock runtime.

## 6. Đối chiếu với yêu cầu giai đoạn 2

| Checklist | Trạng thái |
| --------- | ---------- |
| SQL Server thật | DONE |
| EF Core thật | DONE |
| Có `DbContext` | DONE |
| Entity model map đúng schema hiện có | DONE |
| Service layer rõ ràng | DONE |
| Không dùng mock data cho runtime stage 2 | DONE |
| Login từ bảng `Accounts` | DONE |
| Logout thật | DONE |
| Phân quyền Admin/Staff/Teacher | DONE |
| Chặn truy cập area/module theo role | DONE |
| CRUD Accounts | DONE |
| CRUD Students | DONE |
| CRUD Teachers | DONE |
| CRUD Courses | DONE |
| CRUD Classes | DONE |
| CRUD Enrollments | DONE |
| CRUD Receipts | DONE |
| CRUD ClassSessions | DONE |
| CRUD Attendances | DONE |
| CRUD Exams | PARTIAL |
| CRUD ExamResults | DONE |
| Dashboard dữ liệu thật | DONE |
| Demo flow cuối kỳ chạy được | DONE |
| Code rõ ràng, dễ giải thích | PARTIAL |
| Dùng async/await xuyên suốt | NOT DONE |

Giải thích cho `CRUD Exams = PARTIAL`:
- UI hiện có module `Exams`, và thao tác tạo/sửa thực sự lưu vào cả bảng `Exams` và `ExamResults`.
- Tuy nhiên chưa có controller/service contract tách riêng cho `Exam` như một module độc lập hoàn toàn khỏi `ExamResult`.

Giải thích cho `Code rõ ràng, dễ giải thích = PARTIAL`:
- Đủ dùng để demo và trình bày.
- Nhưng vẫn còn tên interface legacy như `IMockDataService`, dễ gây hiểu nhầm vì runtime hiện không còn mock.

## 7. Kết quả test

### Build result

- `dotnet build` sau chỉnh sửa: `0 Warning(s), 0 Error(s)`.

### Auth result

- Đã test login thật:
  - `admin / 123456` -> `/Admin`
  - `staff01 / 123456` -> `/Staff`
  - `t001 / 123456` -> `/Teacher`
- Đã test logout thật:
  - sau logout, truy cập `/Admin` bị redirect về `/Account/Login`
- Đã test chặn sai role:
  - dùng session admin truy cập `/Teacher` bị redirect về `/Admin`

### CRUD result

- Đã test create thật qua HTTP form + kiểm tra lại bằng SQL:
  - Teacher: pass
  - Course: pass
  - Class: pass
  - ClassSession: pass
  - Student: pass
  - Enrollment: pass
  - Receipt: pass
  - Attendance: pass
  - ExamResult: pass
- Đã có xác nhận lại bằng SQL cho từng bản ghi smoke test.

### Demo flow result

- Luồng đã test pass trên app đang chạy local:
  1. Admin login
  2. Admin tạo teacher
  3. Admin tạo course
  4. Admin tạo class
  5. Admin tạo class session
  6. Staff login
  7. Staff tạo student
  8. Staff tạo enrollment
  9. Staff tạo receipt
  10. Teacher login
  11. Teacher tạo attendance
  12. Teacher nhập điểm
  13. Admin mở dashboard

### Regression result

- `GET /` trả `200`
- route dashboard theo area vẫn hoạt động
- layout chính không vỡ
- link logout trong layout area đã được sửa về `/Account/Logout`

### Lỗi còn tồn tại nếu có

- Không còn blocker runtime trong luồng demo chính.
- Còn nợ kỹ thuật kiến trúc, không phải blocker demo:
  - đồng bộ tên interface/service legacy
  - chuẩn hóa async/await
  - password hashing

## 8. Dữ liệu và cấu hình cần biết

### Connection string

- Đang đặt tại:
  - [appsettings.json](D:/ASP-24CNTT/Quan-ly-trung-tam-ngoai-ngu/Quan-ly-trung-tam-ngoai-ngu/appsettings.json)
  - [appsettings.Development.json](D:/ASP-24CNTT/Quan-ly-trung-tam-ngoai-ngu/Quan-ly-trung-tam-ngoai-ngu/appsettings.Development.json)
- Key đang dùng:
  - `ConnectionStrings:LanguageCenterDb`

### Tài khoản test/dev đã dùng

- Admin:
  - `admin / 123456`
- Staff:
  - `staff01 / 123456`
- Teacher:
  - `t001 / 123456`

### Seed data có sẵn

- `Accounts`: admin, nhiều staff, nhiều teacher
- `Teachers`: từ `T001` đến `T010`
- `Students`: từ `S001` trở đi
- `Courses`, `Classes`, `Enrollments`, `Receipts`, `ClassSessions`, `Attendances`, `Exams`, `ExamResults`

### Module cần data tối thiểu để demo

- Class creation cần ít nhất:
  - 1 course
  - 1 teacher
- Enrollment cần:
  - 1 student
  - 1 class
- Receipt cần:
  - 1 enrollment
- Attendance cần:
  - 1 class session
  - 1 enrollment thuộc đúng class đó
- Exam result cần:
  - 1 enrollment

## 9. Nợ kỹ thuật / lưu ý cho giai đoạn 3

- `Accounts.PasswordHash` hiện đang lưu plain text theo script SQL/seed hiện tại.
  - Đây là điểm cần ưu tiên nâng cấp ở giai đoạn 3 nếu chuyển từ demo sang sản phẩm nghiêm túc.
- Service/controller hiện vẫn chủ yếu là synchronous.
  - Chưa đạt yêu cầu async/await xuyên suốt.
- Tên `IMockDataService` hiện không còn đúng bản chất runtime.
  - Nên đổi thành read service rõ nghĩa hơn để giảm nhầm lẫn.
- Repo vẫn còn ADO.NET services cũ trong `Services/Sql`.
  - Hiện không còn được DI sử dụng, nhưng nên dọn dẹp hoặc archive để giảm nhiễu khi bảo trì.
- Module `Exams` hiện đang theo hướng exam-result-centric.
  - Nếu giai đoạn 3 cần lịch kiểm tra độc lập, nên tách service/controller riêng cho `Exam`.
- UI/view text còn một số chỗ encoding cũ trong source.
  - Runtime vẫn render được ở nhiều trang, nhưng nên chuẩn hóa dần để tránh lỗi hiển thị trong báo cáo/chụp màn hình.

## 10. Kết luận cuối

- Project hiện tại **đã đạt trạng thái sẵn sàng cho demo giai đoạn 2**, với:
  - SQL Server thật
  - EF Core thật
  - auth thật theo bảng `Accounts`
  - role protection thật
  - CRUD chính ghi xuống DB thật
  - dashboard đọc dữ liệu thật
  - luồng demo xuyên suốt đã được test end-to-end
- Tuy nhiên, nếu hiểu "100% giai đoạn 2" theo nghĩa **nghiệp vụ và demo vận hành được**, thì có thể xem là **đã đủ điều kiện**.
- Nếu hiểu "100% giai đoạn 2" theo nghĩa **kiến trúc hoàn toàn sạch, async đầy đủ, bảo mật password đúng chuẩn, tách module Exams hoàn toàn độc lập**, thì **chưa đạt tuyệt đối**.
- Kết luận thực tế:
  - **Đủ điều kiện sang giai đoạn 3 về mặt nền tảng nghiệp vụ và khả năng demo/báo cáo.**
  - **Không nên coi đây là bản hoàn thiện kiến trúc cuối cùng; cần xử lý tiếp nợ kỹ thuật đã liệt kê ở mục 9.**
