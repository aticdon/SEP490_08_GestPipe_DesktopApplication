# Tóm tắt Giao nội dung / Delivery Summary

## Yêu cầu / Requirements

Đọc code trong module Auth và vẽ **class diagram chỉ cho tính năng Login**.
Bao gồm các lớp, interface, DTO, repository, service, và controller **liên quan trực tiếp đến login**.
Sử dụng cú pháp PlantUML chuẩn và có stereotype, nhóm theo package.

## Sản phẩm giao / Deliverables

### 1. LoginFeature_ClassDiagram.puml
File PlantUML chứa class diagram hoàn chỉnh với:

#### Các thành phần (Components):
- **1 Controller**: AuthController
- **4 Interfaces**: IAuthService, IOtpService, IEmailService, IUserService
- **4 Service Implementations**: AuthService, OtpService, EmailService, UserService
- **4 DTOs**: LoginDto, GoogleLoginDto, AuthResponseDto, UserDto
- **3 Entity Models**: User, UserProfile, Otp
- **4 Settings Classes**: JwtSettings, GoogleSettings, MongoDbSettings, SmtpSettings

#### Tổ chức (Organization):
- ✅ 6 packages được định nghĩa rõ ràng
- ✅ Stereotypes cho từng loại class (<<Controller>>, <<Service>>, <<Interface>>, <<DTO>>, <<Entity>>, <<Configuration>>)
- ✅ Color coding theo loại component
- ✅ Relationship arrows với labels rõ ràng (uses, implements, manages)

#### Các quan hệ (Relationships):
- Dependency relationships (uses): Controller → Services, Service → Service
- Implementation relationships: Service → Interface
- Data access relationships: Service → MongoDB Collections
- Data flow: Controller → DTO → Service → Entity

### 2. LOGIN_CLASS_DIAGRAM_README.md
Tài liệu hướng dẫn chi tiết bằng tiếng Việt và tiếng Anh với:
- Tổng quan về diagram
- Giải thích từng package
- Mối quan hệ giữa các components
- Luồng đăng nhập (Normal login & Google OAuth)
- Stereotypes và color coding
- Công nghệ sử dụng
- Hướng dẫn xem diagram

## Các tính năng Login được mô tả / Login Features Described

### 1. Normal Login (Email/Password)
- AuthController.Login() nhận LoginDto
- AuthService xác thực credentials
- Kiểm tra email verification status
- Tạo JWT token
- Cập nhật last login time
- Trả về AuthResponseDto với token

### 2. Google OAuth Login
- AuthController.GoogleLogin() nhận GoogleLoginDto với IdToken
- AuthService validate Google token
- Tạo hoặc cập nhật User
- Tạo UserProfile nếu là user mới
- Tạo JWT token
- Trả về AuthResponseDto với token

### 3. Supporting Features (hỗ trợ login)
- OTP verification (cho email chưa verify)
- Email notifications
- User management
- JWT token generation
- Password hashing với BCrypt

## Công nghệ và Pattern / Technologies and Patterns

- **Architecture**: Layered Architecture (Controller → Service → Repository/Data Access)
- **Design Patterns**: 
  - Dependency Injection
  - Repository Pattern (via MongoDB Collections)
  - DTO Pattern
  - Service Layer Pattern
- **Authentication**: JWT + OAuth2 (Google)
- **Database**: MongoDB
- **Security**: BCrypt password hashing

## Cách xem diagram / How to View

### Online (nhanh nhất):
1. Mở http://www.plantuml.com/plantuml/uml/
2. Copy nội dung file `LoginFeature_ClassDiagram.puml`
3. Paste vào editor
4. Xem kết quả

### Hoặc sử dụng PlantText:
1. Mở https://www.planttext.com/
2. Paste code và xem diagram

### VS Code:
1. Cài extension "PlantUML"
2. Mở file `.puml`
3. Press Alt+D để preview

## Điểm nổi bật / Highlights

✅ **Đầy đủ**: Bao gồm TẤT CẢ components liên quan trực tiếp đến Login  
✅ **Rõ ràng**: Stereotypes và colors giúp phân biệt loại components  
✅ **Chuẩn UML**: Sử dụng cú pháp PlantUML standard  
✅ **Có tổ chức**: Grouped by packages logic  
✅ **Chi tiết**: Methods và properties quan trọng được liệt kê  
✅ **Relationship**: Mọi dependency và implementation được thể hiện  
✅ **Documented**: README đầy đủ bằng 2 ngôn ngữ  
✅ **Flow**: Có mô tả luồng xử lý login  

## Ghi chú / Notes

- Diagram tập trung vào **Login feature** đúng như yêu cầu
- Bỏ qua các feature không liên quan (Register, ForgotPassword, ResetPassword flow details)
- Nhưng vẫn bao gồm các interface methods để thể hiện contract đầy đủ
- MongoDB được thể hiện qua IMongoCollection relationships
- AutoMapper được đề cập trong notes nhưng không vẽ chi tiết vì không phải business logic của Login

---

**Files trong Pull Request:**
1. `LoginFeature_ClassDiagram.puml` - Class diagram hoàn chỉnh
2. `LOGIN_CLASS_DIAGRAM_README.md` - Tài liệu hướng dẫn chi tiết
3. `.gitignore` - Updated để loại trừ code_analysis folder

**Ngày hoàn thành**: 2025-11-10
