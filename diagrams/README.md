# GestPipe Class Diagrams

Thư mục này chứa các PlantUML class diagrams cho hệ thống GestPipe.

## Các file diagram

### 1. `full-class-diagram.puml`
- **Mô tả**: Class diagram đầy đủ cho toàn bộ hệ thống
- **Nội dung**: 
  - Tất cả Domain Models (User, Gesture, Content)
  - Service Layer (Interfaces và Implementations)
  - Controller Layer
  - Tất cả mối quan hệ giữa các class

### 2. `user-auth-module.puml`
- **Mô tả**: Class diagram cho module User & Authentication
- **Nội dung**:
  - User, UserProfile, Otp entities
  - Authentication DTOs
  - IAuthService, IOtpService, IEmailService, IProfileService
  - AuthController, ProfileController

### 3. `gesture-module.puml`
- **Mô tả**: Class diagram cho module Gesture Management
- **Nội dung**:
  - GestureType, DefaultGesture, UserGestureConfig, TrainingGesture entities
  - VectorData value object
  - Gesture-related services
  - Gesture controllers

### 4. `content-session-module.puml`
- **Mô tả**: Class diagram cho module Content & Session
- **Nội dung**:
  - Category, Topic, Session entities
  - CategoryService, TopicService, SessionService
  - Category, Topic, Session controllers

## Cách sử dụng

### Online
1. Truy cập https://www.plantuml.com/plantuml/uml/
2. Copy nội dung file .puml
3. Paste vào editor
4. Xem kết quả diagram

### Visual Studio Code
1. Cài đặt extension "PlantUML" (jebbs.plantuml)
2. Mở file .puml
3. Nhấn `Alt+D` để xem preview

### Command Line (yêu cầu Java và PlantUML JAR)
```bash
java -jar plantuml.jar full-class-diagram.puml
```

## Ký hiệu UML

### Mối quan hệ (Relationships)
- `A -- B`: Association (liên kết)
- `A *-- B`: Composition (chứa, quan hệ mạnh)
- `A o-- B`: Aggregation (chứa, quan hệ yếu)
- `A ..> B`: Dependency (phụ thuộc)
- `A --|> B`: Inheritance (kế thừa)
- `A ..|> B`: Implementation (triển khai interface)

### Multiplicity (Bội số)
- `1`: Đúng 1
- `*`: Nhiều (0 hoặc nhiều)
- `0..1`: 0 hoặc 1
- `1..*`: 1 hoặc nhiều

### Visibility (Phạm vi)
- `+`: Public
- `-`: Private
- `#`: Protected
- `~`: Package/Internal

### Stereotypes (Khuôn mẫu)
- `<<Entity>>`: Domain entity (màu xanh nhạt)
- `<<Service>>`: Service class (màu xanh lá)
- `<<Controller>>`: Controller class (màu vàng)
- `<<DTO>>`: Data Transfer Object (màu xám)
- `<<ValueObject>>`: Value object (màu hồng)

## Tham khảo

- [PlantUML Class Diagram](https://plantuml.com/class-diagram)
- [UML Class Diagram Tutorial](https://www.visual-paradigm.com/guide/uml-unified-modeling-language/uml-class-diagram-tutorial/)
- [Hướng dẫn chi tiết](../CLASS_DIAGRAM_GUIDE.md)
