# Quick Reference - GestPipe Class Diagram

## Tóm tắt nhanh các Class chính

### 📁 User Management (Quản lý người dùng)

```
User (MongoDB Collection: users)
├── Id: string
├── Email: string
├── PasswordHash: string
├── AccountStatus: string
├── CreatedAt: DateTime
└── [Other fields...]

UserProfile (MongoDB Collection: userprofiles)
├── Id: string
├── UserId: string → User.Id
├── FullName: string
├── Occupation: string
└── [Other fields...]

Otp (MongoDB Collection: otps)
├── Email: string (Primary Key)
├── OtpCode: string
├── Purpose: string (registration, resetpassword)
├── ExpiresAt: DateTime
└── IsExpired(): bool
```

**Quan hệ**: User 1-1 UserProfile

---

### 🖐️ Gesture Management (Quản lý cử chỉ)

```
GestureType (MongoDB Collection: gesturetypes)
├── Id: string
├── TypeName: Dictionary<string, string>
└── Code: Dictionary<string, string>

DefaultGesture (MongoDB Collection: defaultgestures)
├── Id: string
├── VersionId: string
├── GestureTypeId: string → GestureType.Id
├── PoseLabel: string
├── Accuracy: double
└── VectorData: VectorData (embedded)
    ├── Fingers: int[]
    ├── MainAxisX: double
    ├── MainAxisY: double
    ├── DeltaX: double
    └── DeltaY: double

UserGestureConfig (MongoDB Collection: usergestureconfigs)
├── Id: string
├── UserId: string → User.Id
├── GestureTypeId: string → GestureType.Id
├── PoseLabel: string
├── Accuracy: double
└── VectorData: VectorData (embedded)

TrainingGesture (MongoDB Collection: traininggestures)
├── Id: string
├── UserId: string → User.Id
├── PoseLabel: string
├── TotalTrain: int
├── CorrectTrain: int
├── Accuracy: double
└── VectorData: VectorData (embedded)

UserGestureRequest (MongoDB Collection: usergesturerequests)
├── Id: string
├── UserId: string → User.Id
├── UserGestureConfigId: string → UserGestureConfig.Id
├── GestureTypeId: string → GestureType.Id
├── PoseLabel: string
└── Status: Dictionary<string, string>
```

**Quan hệ**:
- GestureType 1-nhiều DefaultGesture
- GestureType 1-nhiều UserGestureConfig
- User 1-nhiều UserGestureConfig
- User 1-nhiều TrainingGesture
- UserGestureConfig 1-nhiều UserGestureRequest

---

### 📚 Content Management (Quản lý nội dung)

```
Category (MongoDB Collection: categories)
├── Id: string
└── Name: Dictionary<string, string>

Topic (MongoDB Collection: topics)
├── Id: string
├── Title: Dictionary<string, string>
├── CategoryId: string → Category.Id
├── Description: string
└── Difficulty: string

Session (MongoDB Collection: sessions)
├── Id: string
├── UserId: string → User.Id
├── CategoryId: string → Category.Id
├── TopicId: string → Topic.Id
├── Records: Dictionary<string, int>
├── Duration: double
└── CreatedAt: DateTime
```

**Quan hệ**:
- Category 1-nhiều Topic
- Category 1-nhiều Session
- Topic 1-nhiều Session
- User 1-nhiều Session

---

## 🔧 Service Layer (Lớp dịch vụ)

### Authentication & User Services
```
IAuthService → AuthService
├── RegisterAsync()
├── LoginAsync()
├── VerifyOtpAsync()
├── GoogleLoginAsync()
├── ForgotPasswordAsync()
├── ResetPasswordAsync()
└── LogoutAsync()

IOtpService → OtpService
├── GenerateOtpAsync()
├── ValidateOtpAsync()
└── MarkOtpAsVerifiedAsync()

IEmailService → EmailService
├── SendVerificationEmailAsync()
└── SendPasswordResetEmailAsync()

IProfileService → ProfileService
├── GetProfileAsync()
└── UpdateProfileAsync()

IUserService → UserService
```

### Gesture Services
```
IGestureTypeService → GestureTypeService
├── GetAllGestureTypesAsync()
└── GetGestureTypeByIdAsync()

IDefaultGestureService → DefaultGestureService
├── GetDefaultGesturesAsync()
└── GetDefaultGestureByIdAsync()

IUserGestureConfigService → UserGestureConfigService
├── GetUserGesturesAsync()
├── CreateUserGestureAsync()
└── UpdateUserGestureAsync()

ITrainingGestureService → TrainingGestureService
├── GetTrainingGesturesAsync()
└── CreateTrainingGestureAsync()

IUserGestureRequestService → UserGestureRequestService
├── GetUserRequestsAsync()
├── CreateRequestAsync()
└── UpdateRequestStatusAsync()

IGestureInitializationService → GestureInitializationService
├── InitializeUserGesturesAsync()
└── GetUserGestureStatsAsync()
```

### Content Services
```
CategoryService
├── GetAllCategoriesAsync()
└── GetCategoryByIdAsync()

TopicService
├── GetAllTopicsAsync()
├── GetTopicByIdAsync()
└── GetTopicsByCategoryAsync()

SessionService
├── GetUserSessionsAsync()
├── CreateSessionAsync()
└── GetSessionByIdAsync()
```

---

## 🎮 Controller Layer (Lớp điều khiển)

### API Endpoints Structure

```
AuthController (api/auth)
├── POST /register
├── POST /login
├── POST /validate-otp
├── POST /resend-otp
├── POST /google-login
├── POST /forgot-password
├── POST /reset-password
├── POST /logout
└── GET /gestures/stats

ProfileController (api/profile)
├── GET /
└── PUT /

UserController (api/user)

GestureTypeController (api/gesturetype)
├── GET /
└── GET /{id}

DefaultGestureController (api/defaultgesture)

UserGestureConfigController (api/usergestureconfig)
├── GET /
├── POST /
└── PUT /{id}

TrainingGestureController (api/traininggesture)
├── GET /
└── POST /

UserGestureRequestController (api/usergesturerequest)
├── GET /
└── POST /

CategoryController (api/category)
├── GET /
└── GET /{id}

TopicController (api/topic)
├── GET /
└── GET /category/{categoryId}

SessionController (api/session)
├── GET /
└── POST /
```

---

## 🔗 Dependency Flow (Luồng phụ thuộc)

```
Client Request
    ↓
Controller (uses DTOs)
    ↓
Service Interface (business logic)
    ↓
Service Implementation
    ↓
MongoDB Collections (Domain Models)
    ↓
Response (DTOs)
    ↓
Client
```

---

## 📊 Key Patterns (Các mẫu thiết kế)

1. **Repository Pattern**: MongoDB collections as repositories
2. **Service Layer Pattern**: Business logic in services
3. **Dependency Injection**: All dependencies injected via constructor
4. **DTO Pattern**: Separate DTOs for data transfer
5. **Value Object Pattern**: VectorData embedded in entities

---

## 🗂️ MongoDB Collections Summary

| Collection Name         | Primary Entity          | Key Relationships |
|------------------------|-------------------------|-------------------|
| users                  | User                    | → userprofiles (1-1) |
| userprofiles           | UserProfile             | → users (1-1) |
| otps                   | Otp                     | None |
| gesturetypes           | GestureType             | → defaultgestures (1-*) |
| defaultgestures        | DefaultGesture          | → gesturetypes (1-1) |
| usergestureconfigs     | UserGestureConfig       | → users (1-1), → gesturetypes (1-1) |
| traininggestures       | TrainingGesture         | → users (1-1) |
| usergesturerequests    | UserGestureRequest      | → users, → usergestureconfigs |
| categories             | Category                | → topics (1-*) |
| topics                 | Topic                   | → categories (1-1) |
| sessions               | Session                 | → users, → categories, → topics |

---

## 💡 Tips for Drawing

### Bước 1: Vẽ Domain Models
1. Bắt đầu với User và UserProfile (1-1)
2. Thêm các Gesture classes với VectorData
3. Thêm Category, Topic, Session
4. Vẽ các mối quan hệ

### Bước 2: Vẽ Services
1. Vẽ các Interface services
2. Vẽ các Implementation classes
3. Kết nối implementations với interfaces (dashed line with triangle)

### Bước 3: Vẽ Controllers
1. Vẽ các Controller classes
2. Kết nối Controllers với Services (dashed dependency arrow)

### Bước 4: Tùy chọn - Vẽ DTOs
1. Nhóm DTOs theo chức năng
2. Vẽ dependencies từ Controllers

---

## 🎨 Color Coding (for diagrams)

- **Blue** 🔵: Domain Entities
- **Green** 🟢: Service Layer
- **Yellow** 🟡: Controllers
- **Gray** ⚪: DTOs
- **Pink** 🟣: Value Objects

