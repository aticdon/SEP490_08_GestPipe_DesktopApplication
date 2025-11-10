# Class Diagram - Login Feature (Auth Module)

## Tổng quan / Overview

Tài liệu này mô tả **Class Diagram** cho **tính năng Login** trong module Auth của hệ thống GestPipe Desktop Application. Diagram được vẽ bằng PlantUML và bao gồm tất cả các lớp, interface, DTO, và service liên quan trực tiếp đến chức năng đăng nhập.

This document describes the **Class Diagram** for the **Login Feature** in the Auth module of the GestPipe Desktop Application system. The diagram is created using PlantUML and includes all classes, interfaces, DTOs, and services directly related to login functionality.

## Cấu trúc / Structure

Diagram được tổ chức theo các package sau:

The diagram is organized into the following packages:

### 1. **Controllers Package** (Lớp điều khiển)
- **AuthController**: Controller chính xử lý các HTTP request liên quan đến authentication
  - Xử lý login thông thường (email/password)
  - Xử lý Google OAuth login
  - Validate input và trả về HTTP responses

### 2. **Services.Interfaces Package** (Interface của các dịch vụ)
Các interface định nghĩa contract cho các service:
- **IAuthService**: Interface cho authentication service
- **IOtpService**: Interface cho OTP verification service
- **IEmailService**: Interface cho email notification service
- **IUserService**: Interface cho user management service

### 3. **Services.Implementation Package** (Triển khai các dịch vụ)
Các class triển khai business logic:
- **AuthService**: 
  - Triển khai IAuthService
  - Xử lý logic đăng nhập (normal & Google OAuth)
  - Tạo JWT token
  - Hash password với BCrypt
  - Tương tác với MongoDB
- **OtpService**: Quản lý mã OTP cho verification
- **EmailService**: Gửi email thông báo
- **UserService**: Quản lý thông tin user

### 4. **Models.DTOs Package** (Data Transfer Objects)
Các DTO dùng để truyền dữ liệu:
- **LoginDto**: Chứa email và password cho login thông thường
- **GoogleLoginDto**: Chứa IdToken cho Google OAuth
- **AuthResponseDto**: Response trả về sau khi authenticate
- **UserDto**: DTO đại diện cho user data

### 5. **Models Package** (Domain Models)
Các entity models ánh xạ với MongoDB:
- **User**: Entity chính của người dùng
- **UserProfile**: Thông tin chi tiết của người dùng
- **Otp**: Entity lưu trữ mã OTP

### 6. **Models.Settings Package** (Configuration Classes)
Các class cấu hình:
- **JwtSettings**: Cấu hình JWT token
- **GoogleSettings**: Cấu hình Google OAuth
- **MongoDbSettings**: Cấu hình MongoDB connection
- **SmtpSettings**: Cấu hình SMTP email

## Các mối quan hệ chính / Main Relationships

### Dependency Injection (uses relationship)
- AuthController sử dụng IAuthService, IOtpService, IEmailService
- AuthService sử dụng IOtpService, IEmailService
- Các service implementations sử dụng Settings classes

### Implementation (implements relationship)
- AuthService implements IAuthService
- OtpService implements IOtpService
- EmailService implements IEmailService
- UserService implements IUserService

### Data Flow
1. **AuthController** nhận HTTP request với LoginDto hoặc GoogleLoginDto
2. **AuthController** gọi **IAuthService.LoginAsync()** hoặc **GoogleLoginAsync()**
3. **AuthService** thực hiện:
   - Validate credentials
   - Kiểm tra User trong MongoDB
   - Tạo JWT token
   - Trả về AuthResponseDto
4. **AuthController** trả về HTTP response

## Luồng đăng nhập / Login Flow

### 1. Normal Login (Email/Password)
```
Client → AuthController.Login(LoginDto)
       → IAuthService.LoginAsync(LoginDto, ipAddress, userAgent)
       → AuthService validates credentials
       → AuthService generates JWT token
       → Returns AuthResponseDto
       ← Client receives token
```

### 2. Google OAuth Login
```
Client → AuthController.GoogleLogin(GoogleLoginDto)
       → IAuthService.GoogleLoginAsync(idToken, ipAddress, userAgent)
       → AuthService validates Google token
       → AuthService creates/updates User
       → AuthService generates JWT token
       → Returns AuthResponseDto
       ← Client receives token
```

## Các stereotype được sử dụng / Stereotypes Used

- **<<Controller>>**: Các lớp controller trong MVC pattern
- **<<Interface>>**: Các interface định nghĩa contract
- **<<Service>>**: Các lớp service chứa business logic
- **<<DTO>>**: Data Transfer Objects
- **<<Entity>>**: Domain models/entities
- **<<Configuration>>**: Configuration/Settings classes

## Color Coding

- 🟢 **Controllers** (Green): #E8F5E9
- 🔵 **Services** (Blue): #E3F2FD
- 🟡 **Interfaces** (Yellow): #FFF9C4
- 🔴 **DTOs** (Pink): #FCE4EC
- 🟣 **Models** (Purple): #F3E5F5
- 🟢 **Settings** (Teal): #E0F2F1

## Công nghệ sử dụng / Technologies Used

- **ASP.NET Core**: Web API framework
- **MongoDB**: NoSQL database
- **JWT**: JSON Web Token for authentication
- **BCrypt**: Password hashing
- **Google OAuth**: Third-party authentication
- **AutoMapper**: Object mapping
- **Dependency Injection**: IoC pattern

## File Diagram

File PlantUML: `LoginFeature_ClassDiagram.puml`

## Cách xem diagram / How to View the Diagram

### Online Viewers:
1. PlantUML Online Server: http://www.plantuml.com/plantuml/uml/
2. PlantText: https://www.planttext.com/

### IDE Plugins:
- Visual Studio Code: PlantUML extension
- IntelliJ IDEA: PlantUML integration plugin
- Eclipse: PlantUML plugin

### Command Line:
```bash
plantuml LoginFeature_ClassDiagram.puml
```

## Ghi chú / Notes

- Diagram tập trung vào **Login feature** và bỏ qua các features khác như Register, ForgotPassword, ResetPassword
- Các method liên quan đến Login được highlight trong các interface và class
- MongoDB collections được thể hiện thông qua các dependency relationship
- Dependency Injection pattern được sử dụng xuyên suốt architecture

---

**Tác giả / Author**: GitHub Copilot  
**Ngày tạo / Created**: 2025-11-10  
**Phiên bản / Version**: 1.0
