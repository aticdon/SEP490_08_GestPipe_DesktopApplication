# Quick Start - Login Feature Class Diagram

## 🚀 Cách xem nhanh nhất / Fastest Way to View

### Bước 1: Copy nội dung file PlantUML
```bash
cat LoginFeature_ClassDiagram.puml
```

### Bước 2: Mở trình xem online
Truy cập: **http://www.plantuml.com/plantuml/uml/**

### Bước 3: Paste và xem
1. Paste nội dung vào ô text
2. Click "Submit" hoặc nhấn Ctrl+Enter
3. Xem diagram được render

## 📂 Files trong repository

```
.
├── LoginFeature_ClassDiagram.puml      # Diagram chính (PlantUML)
├── LOGIN_CLASS_DIAGRAM_README.md       # Documentation đầy đủ
├── DELIVERY_SUMMARY.md                 # Tóm tắt nội dung giao
├── QUICK_START.md                      # Hướng dẫn này
└── .gitignore                          # Git ignore file
```

## 🎨 Diagram Structure

### Packages (6 nhóm):
1. **Controllers** - Lớp xử lý HTTP requests
2. **Services.Interfaces** - Các interface định nghĩa contract
3. **Services.Implementation** - Triển khai business logic
4. **Models.DTOs** - Data Transfer Objects
5. **Models** - Domain entities (User, UserProfile, Otp)
6. **Models.Settings** - Configuration classes

### Components (20 thành phần):
- 1 Controller
- 4 Service Interfaces
- 4 Service Implementations
- 4 DTOs
- 3 Entity Models
- 4 Settings Classes

## 🔍 Main Flow - Normal Login

```
HTTP POST /api/auth/login
    ↓
[AuthController] receives LoginDto
    ↓
[IAuthService.LoginAsync()] validates credentials
    ↓
[AuthService] checks User in MongoDB
    ↓
[AuthService] verifies password (BCrypt)
    ↓
[AuthService] generates JWT token
    ↓
Returns AuthResponseDto with token
    ↓
HTTP 200 OK
```

## 🔐 Main Flow - Google Login

```
HTTP POST /api/auth/google-login
    ↓
[AuthController] receives GoogleLoginDto (IdToken)
    ↓
[IAuthService.GoogleLoginAsync()] validates Google token
    ↓
[AuthService] validates with Google API
    ↓
[AuthService] creates/finds User
    ↓
[AuthService] generates JWT token
    ↓
Returns AuthResponseDto with token
    ↓
HTTP 200 OK
```

## 🛠️ Technologies Stack

- **Backend**: ASP.NET Core Web API
- **Database**: MongoDB (NoSQL)
- **Authentication**: JWT + OAuth2 (Google)
- **Security**: BCrypt password hashing
- **Mapping**: AutoMapper
- **Pattern**: Dependency Injection, Layered Architecture

## 📚 Documentation Files

### 1. LoginFeature_ClassDiagram.puml
- **Loại**: PlantUML source code
- **Dòng**: 302 lines
- **Nội dung**: Complete class diagram với relationships

### 2. LOGIN_CLASS_DIAGRAM_README.md
- **Loại**: Markdown documentation
- **Dòng**: 165 lines
- **Ngôn ngữ**: Vietnamese & English
- **Nội dung**: Chi tiết về packages, components, relationships, flows

### 3. DELIVERY_SUMMARY.md
- **Loại**: Markdown summary
- **Dòng**: 124 lines
- **Nội dung**: Tổng hợp deliverables và highlights

## ⚡ Alternative Viewers

### PlantText (Simple UI)
https://www.planttext.com/

### Visual Studio Code
1. Install extension: "PlantUML"
2. Open `.puml` file
3. Press `Alt+D` to preview

### IntelliJ IDEA / Eclipse
Install PlantUML plugin from marketplace

## 📞 Support

Nếu có vấn đề với diagram hoặc cần thêm thông tin:
1. Xem `LOGIN_CLASS_DIAGRAM_README.md` để có hướng dẫn chi tiết
2. Xem `DELIVERY_SUMMARY.md` để hiểu tổng quan
3. Check PlantUML syntax tại: https://plantuml.com/class-diagram

---
**Note**: Diagram tập trung vào Login feature. Các features khác (Register, Reset Password) không được document chi tiết trong diagram này.
