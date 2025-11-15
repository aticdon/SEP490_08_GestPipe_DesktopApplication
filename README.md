# SEP490_08_GestPipe_DesktopApplication

## 📖 Tài liệu Class Diagram / Class Diagram Documentation

Dự án này đã được phân tích đầy đủ để hỗ trợ vẽ Class Diagram. Các tài liệu sau đây có sẵn:

### 📚 Tài liệu chính / Main Documentation

1. **[HOW_TO_USE.md](./HOW_TO_USE.md)** - ⭐ **BẮT ĐẦU TẠI ĐÂY / START HERE** ⭐
   - Hướng dẫn sử dụng tất cả tài liệu
   - Giải thích khi nào dùng tài liệu nào
   - FAQ và tips

2. **[CLASS_DIAGRAM_GUIDE.md](./CLASS_DIAGRAM_GUIDE.md)** - Hướng dẫn đầy đủ (40KB)
   - Phân tích chi tiết tất cả các class
   - Mối quan hệ giữa các class
   - Design patterns được sử dụng
   - PlantUML code đầy đủ
   - Hướng dẫn vẽ diagram từng bước

3. **[QUICK_REFERENCE.md](./QUICK_REFERENCE.md)** - Tham khảo nhanh (8.6KB)
   - Tóm tắt nhanh các class
   - MongoDB collections structure
   - API endpoints
   - Tips vẽ diagram

### 🎨 PlantUML Diagrams

Thư mục **[diagrams/](./diagrams/)** chứa các file PlantUML:
- `full-class-diagram.puml` - Class diagram đầy đủ
- `user-auth-module.puml` - Module User & Authentication
- `gesture-module.puml` - Module Gesture Management
- `content-session-module.puml` - Module Content & Session

**Cách render**: Copy code vào https://www.plantuml.com/plantuml/uml/ hoặc dùng VS Code extension "PlantUML"

---

## 🏗️ Kiến trúc hệ thống / System Architecture

Hệ thống GestPipe bao gồm:
- **Backend API**: ASP.NET Core Web API với MongoDB
- **Desktop Application**: WPF Application
- **Architecture**: Service Layer Pattern + Dependency Injection + Repository Pattern

### Module chính:
1. **User Management**: User, UserProfile, Authentication, OTP
2. **Gesture Management**: Gesture configuration, Training, Requests
3. **Content & Session**: Categories, Topics, Session tracking

---

## 🚀 Quick Start để vẽ Class Diagram

1. Đọc **HOW_TO_USE.md** để hiểu tài liệu nào phù hợp với bạn
2. Đọc **QUICK_REFERENCE.md** để có cái nhìn tổng quan
3. Sử dụng PlantUML code trong **CLASS_DIAGRAM_GUIDE.md** hoặc files `.puml` để render diagram
4. Tùy chỉnh diagram theo nhu cầu của bạn

---

## 📞 Support

Nếu có thắc mắc về tài liệu class diagram, vui lòng tạo issue trong repository.
