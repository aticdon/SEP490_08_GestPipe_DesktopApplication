# Hướng dẫn sử dụng tài liệu Class Diagram
# How to Use Class Diagram Documentation

## 📚 Tài liệu đã tạo / Documentation Created

Dự án này đã được phân tích và tạo ra các tài liệu sau:

### 1. CLASS_DIAGRAM_GUIDE.md (Tài liệu chính)
**40KB - Hướng dẫn đầy đủ**

📖 **Nội dung:**
- Tổng quan hệ thống (Overview)
- Phân tích chi tiết tất cả các class (Domain Models, Services, Controllers)
- Mối quan hệ giữa các class (Relationships)
- Các mẫu thiết kế được sử dụng (Design Patterns)
- Hướng dẫn vẽ class diagram từng bước
- PlantUML code đầy đủ cho toàn bộ hệ thống
- PlantUML code chi tiết cho từng module

🎯 **Khi nào dùng:**
- Khi cần hiểu toàn bộ kiến trúc hệ thống
- Khi cần vẽ class diagram hoàn chỉnh
- Khi học về các design patterns trong dự án
- Khi cần PlantUML code để render diagram

---

### 2. QUICK_REFERENCE.md (Tham khảo nhanh)
**8.6KB - Tra cứu nhanh**

📖 **Nội dung:**
- Tóm tắt nhanh tất cả các class
- Cấu trúc dữ liệu MongoDB collections
- Danh sách API endpoints
- Bảng tổng hợp quan hệ
- Tips vẽ diagram
- Color coding guide

🎯 **Khi nào dùng:**
- Khi cần tra cứu nhanh một class
- Khi cần xem cấu trúc MongoDB
- Khi cần biết các API endpoints
- Khi cần tham khảo nhanh quan hệ giữa các class

---

### 3. diagrams/ (Thư mục PlantUML)
**4 file .puml + 1 README**

📁 **File bao gồm:**

#### a) full-class-diagram.puml (11KB)
- Class diagram đầy đủ cho toàn bộ hệ thống
- Bao gồm: Models, Services, Controllers
- Tất cả relationships

#### b) user-auth-module.puml (5.3KB)
- Module User & Authentication
- User, UserProfile, Otp
- Auth services và controllers

#### c) gesture-module.puml (5.9KB)
- Module Gesture Management  
- Tất cả các gesture entities
- VectorData value object

#### d) content-session-module.puml (2.8KB)
- Module Content & Session
- Category, Topic, Session

#### e) README.md (2.7KB)
- Hướng dẫn sử dụng các file PlantUML
- Cách render diagram
- Giải thích ký hiệu UML

🎯 **Khi nào dùng:**
- Khi cần render class diagram thành hình ảnh
- Khi muốn chia nhỏ diagram theo module
- Khi sử dụng PlantUML tools

---

## 🚀 Cách sử dụng / How to Use

### Scenario 1: Tôi muốn hiểu toàn bộ hệ thống
**→ Đọc CLASS_DIAGRAM_GUIDE.md từ đầu đến cuối**
1. Phần "Tổng quan" để hiểu big picture
2. Phần "Các thành phần chính" để hiểu chi tiết
3. Phần "Mối quan hệ giữa các lớp" để hiểu cách các class kết nối
4. Phần "Các mẫu thiết kế" để hiểu architecture patterns

### Scenario 2: Tôi cần vẽ class diagram
**→ Sử dụng PlantUML code trong CLASS_DIAGRAM_GUIDE.md hoặc files .puml**
1. Copy PlantUML code
2. Paste vào https://www.plantuml.com/plantuml/uml/
3. Hoặc dùng VS Code extension "PlantUML"
4. Export thành PNG/SVG

### Scenario 3: Tôi chỉ cần tra cứu một class cụ thể
**→ Dùng QUICK_REFERENCE.md**
1. Tìm class trong phần tương ứng (User/Gesture/Content)
2. Xem cấu trúc và thuộc tính
3. Xem quan hệ với các class khác

### Scenario 4: Tôi muốn diagram cho một module cụ thể
**→ Dùng file .puml trong thư mục diagrams/**
- User/Auth module: `user-auth-module.puml`
- Gesture module: `gesture-module.puml`
- Content module: `content-session-module.puml`

### Scenario 5: Tôi cần biết API endpoints
**→ Xem phần "Controller Layer" trong QUICK_REFERENCE.md**

---

## 🎨 Render PlantUML Diagrams

### Online (Không cần cài đặt)
```
1. Truy cập: https://www.plantuml.com/plantuml/uml/
2. Copy nội dung file .puml
3. Paste vào editor
4. Click "Submit" để xem diagram
5. Download PNG/SVG nếu cần
```

### VS Code (Recommended)
```
1. Cài extension "PlantUML" (jebbs.plantuml)
2. Mở file .puml
3. Nhấn Alt+D để preview
4. Right-click → "Export Current Diagram" để save
```

### Command Line
```bash
# Cài PlantUML
# Ubuntu/Debian
sudo apt-get install plantuml

# macOS
brew install plantuml

# Render diagram
cd diagrams/
plantuml full-class-diagram.puml
# Output: full-class-diagram.png
```

---

## 📖 Giải thích ký hiệu UML / UML Notation

### Relationships (Mối quan hệ)
```
A ———— B          Association (liên kết)
A ◆———— B         Composition (chứa, quan hệ mạnh)
A ◇———— B         Aggregation (chứa, quan hệ yếu)
A ——▶ B           Navigation/Directed association
A - - -▶ B         Dependency (phụ thuộc)
A ————▷ B         Inheritance (kế thừa)
A - - -▷ B         Implementation (triển khai interface)
```

### Multiplicity (Bội số)
```
1              Đúng 1
0..1           0 hoặc 1
*              Nhiều (0 trở lên)
1..*           1 trở lên
n..m           Từ n đến m
```

### Visibility (Phạm vi truy cập)
```
+ public       Public
- private      Private
# protected    Protected
~ package      Package/Internal
```

---

## 🏗️ Kiến trúc hệ thống / System Architecture

```
┌─────────────────────────────────────────────┐
│          GestPipe Application               │
│                                             │
│  ┌─────────────────────────────────────┐   │
│  │      Desktop Application (WPF)      │   │
│  │      - ApiClient                    │   │
│  │      - SocketServer                 │   │
│  │      - Views, Models, Services      │   │
│  └────────────┬────────────────────────┘   │
│               │ HTTP/WebSocket             │
│  ┌────────────▼────────────────────────┐   │
│  │    Backend API (ASP.NET Core)       │   │
│  │                                     │   │
│  │  ┌──────────────────────────────┐  │   │
│  │  │   Controllers (API Layer)    │  │   │
│  │  │   - AuthController           │  │   │
│  │  │   - UserGestureConfigCtrl    │  │   │
│  │  │   - SessionController        │  │   │
│  │  └───────────┬──────────────────┘  │   │
│  │              │                      │   │
│  │  ┌───────────▼──────────────────┐  │   │
│  │  │   Service Layer              │  │   │
│  │  │   - IAuthService → Auth...   │  │   │
│  │  │   - IGestureService → ...    │  │   │
│  │  │   - Business Logic           │  │   │
│  │  └───────────┬──────────────────┘  │   │
│  │              │                      │   │
│  │  ┌───────────▼──────────────────┐  │   │
│  │  │   Domain Models              │  │   │
│  │  │   - User, UserProfile        │  │   │
│  │  │   - Gesture entities         │  │   │
│  │  │   - Category, Topic, Session │  │   │
│  │  └───────────┬──────────────────┘  │   │
│  └──────────────┼──────────────────────┘   │
│                 │                           │
│  ┌──────────────▼──────────────────────┐   │
│  │         MongoDB Database            │   │
│  │  - users                            │   │
│  │  - usergestureconfigs               │   │
│  │  - sessions                         │   │
│  │  - [other collections]              │   │
│  └─────────────────────────────────────┘   │
└─────────────────────────────────────────────┘
```

---

## 📊 Module Overview / Tổng quan các Module

### 1️⃣ User Management Module
- **Entities**: User, UserProfile, Otp
- **Services**: AuthService, UserService, ProfileService, OtpService, EmailService
- **Controllers**: AuthController, UserController, ProfileController
- **Features**: Registration, Login, OTP verification, Google OAuth, Password reset

### 2️⃣ Gesture Management Module
- **Entities**: GestureType, DefaultGesture, UserGestureConfig, TrainingGesture, UserGestureRequest
- **Value Object**: VectorData
- **Services**: GestureTypeService, DefaultGestureService, UserGestureConfigService, TrainingGestureService, GestureInitializationService
- **Controllers**: GestureTypeController, DefaultGestureController, UserGestureConfigController, TrainingGestureController
- **Features**: Gesture configuration, Training, User requests, Default gestures

### 3️⃣ Content & Session Module
- **Entities**: Category, Topic, Session
- **Services**: CategoryService, TopicService, SessionService
- **Controllers**: CategoryController, TopicController, SessionController
- **Features**: Content categorization, Topic management, Session tracking

---

## 💡 Tips cho người vẽ diagram

### Khi vẽ bằng tay hoặc tool khác:

1. **Bắt đầu từ giữa**: Vẽ class chính (User, GestureType) ở giữa
2. **Mở rộng ra ngoài**: Thêm các class liên quan xung quanh
3. **Sử dụng màu sắc**:
   - 🔵 Xanh dương: Domain entities
   - 🟢 Xanh lá: Services
   - 🟡 Vàng: Controllers
   - ⚪ Xám: DTOs
   - 🟣 Hồng: Value objects

4. **Chia nhỏ diagram**: Nếu quá phức tạp, chia thành 3 diagrams riêng biệt theo module

5. **Chú thích quan trọng**: Thêm notes cho các điểm đặc biệt (VectorData embedded, Dictionary cho đa ngôn ngữ, etc.)

---

## ❓ FAQ

**Q: Tôi nên bắt đầu từ đâu?**
A: Bắt đầu với QUICK_REFERENCE.md để có cái nhìn tổng quan, sau đó đọc CLASS_DIAGRAM_GUIDE.md để hiểu chi tiết.

**Q: Làm sao để render PlantUML diagram?**
A: Cách nhanh nhất là dùng https://www.plantuml.com/plantuml/uml/ hoặc cài VS Code extension "PlantUML".

**Q: Tôi chỉ quan tâm đến Gesture module, nên xem file nào?**
A: Mở file `diagrams/gesture-module.puml` hoặc đọc phần "1.2 Gesture Management" trong CLASS_DIAGRAM_GUIDE.md.

**Q: Diagram có thể chỉnh sửa không?**
A: Có, tất cả file .puml đều là text và có thể chỉnh sửa. Sau khi sửa, render lại để xem kết quả.

**Q: Có thể export diagram sang format khác không?**
A: Có, PlantUML hỗ trợ export sang PNG, SVG, PDF, và nhiều format khác.

---

## 📞 Liên hệ / Contact

Nếu có thắc mắc về tài liệu hoặc cần hỗ trợ thêm, vui lòng tạo issue trong repository.

---

**Chúc bạn vẽ class diagram thành công! / Happy diagram drawing! 🎨**

