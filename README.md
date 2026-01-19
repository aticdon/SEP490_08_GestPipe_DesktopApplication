# 🖐️ GestPipe: AI-Powered Hand Gesture Control System

> **Transform presentations into immersive experiences** — Control PowerPoint slides and manipulate 3D objects using real-time hand gestures, no hardware required.

[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%204.7.2-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Python](https://img.shields.io/badge/Python-3.x-3776AB?logo=python&logoColor=white)](https://www.python.org/)
[![MediaPipe](https://img.shields.io/badge/MediaPipe-Computer%20Vision-00C4CC?logo=google)](https://mediapipe.dev/)
[![MongoDB](https://img.shields.io/badge/MongoDB-Database-47A248?logo=mongodb&logoColor=white)](https://www.mongodb.com/)

---

## 🎯 Impact

**GestPipe** is a full-stack desktop application that revolutionizes presentation delivery by enabling touchless interaction through computer vision. Perfect for:
- 🎓 Educational presentations where instructors need hands-free control
- 🏢 Professional demos showcasing 3D models and product prototypes
- ♿ Accessibility scenarios requiring non-traditional input methods
- 🚀 Tech showcases demonstrating AI/ML capabilities

---

## ✨ Key Features

### 🎮 Real-Time Gesture Recognition
- **10-finger hand tracking** using MediaPipe pose estimation
- **Custom gesture training** — Create personalized gestures tailored to your style
- **Static & dynamic gestures** — Detect both hand poses and motion patterns
- **Sub-100ms latency** for responsive presentation control

### 📊 PowerPoint Integration
- **Navigate slides** (next/previous) with intuitive hand swipes
- **Zoom control** using pinch gestures
- **3D model manipulation** — Rotate and inspect embedded 3D objects with hand movements
- **Seamless interop** with Microsoft Office via COM automation

### 👤 Multi-User Support
- **User profiles** with authentication and personalized gesture configurations
- **Training sessions** with accuracy metrics and progress tracking
- **Cloud sync** via Google Drive for gesture data portability
- **Multi-language UI** (English/Vietnamese)

### 📈 ML Training Pipeline
- **Gesture data collection** with real-time camera feedback
- **Vector-based feature extraction** (finger states + movement deltas)
- **Model training** with performance analytics
- **CSV import/export** for dataset management

---

## 🛠️ Technology Stack

### Frontend (Desktop Application)
- **C# WinForms** (.NET Framework 4.7.2) — Rich desktop UI
- **WebView2** — Embedded 3D model viewer
- **HelixToolkit + SharpDX** — 3D graphics rendering
- **AForge** — Video capture and processing
- **Guna.UI2 & SunnyUI** — Modern UI components

### Backend (REST API)
- **ASP.NET Core 8.0** — RESTful API server
- **MongoDB** — NoSQL database for gesture data and user profiles
- **JWT Authentication** — Secure API access
- **Google Drive API** — Cloud storage integration
- **AutoMapper** — DTO mapping

### Computer Vision
- **Python 3.x** — ML processing engine
- **MediaPipe** — Hand landmark detection
- **OpenCV** — Image processing pipeline
- **TCP Sockets** — Real-time C#/Python IPC

### Architecture Pattern
```
[Camera Feed] → [Python MediaPipe] → [TCP Socket] → [C# Desktop App] → [PowerPoint COM API]
                      ↓                                      ↓
              [Gesture Vectors]                    [3D Model Renderer]
                      ↓                                      ↓
              [.NET API Server] ←→ [MongoDB Database] ←→ [Google Drive]
```

---

## 🚀 Demo Highlights

### Gesture Control in Action
1. **👆 Point** → Select slide or 3D object
2. **👋 Swipe Left/Right** → Navigate slides
3. **✊ Fist** → Pause/resume
4. **🤏 Pinch & Spread** → Zoom in/out on content
5. **🔄 Circular Motion** → Rotate 3D models

### Custom Gesture Training
- Record 30+ samples per gesture for robust recognition
- Real-time accuracy feedback during training
- Export trained models for sharing across devices

---

## 📁 Project Structure

```
GestPipe/
├── GestPipePowerPoint/          # Desktop WinForms Application
│   ├── Views/                   # UI Forms (Login, Presentation, Training)
│   ├── Services/                # Business logic and gesture processing
│   ├── PythonProcessManager.cs  # Python subprocess orchestration
│   ├── SocketServer.cs          # TCP communication layer
│   └── ApiClient.cs             # Backend API integration
│
├── GestPipe.Backend/            # ASP.NET Core REST API
│   ├── Controllers/             # API endpoints (Auth, Gestures, Sessions)
│   ├── Services/                # Core services (Drive sync, gesture config)
│   ├── Models/                  # MongoDB data models
│   └── Validators/              # Request validation logic
│
└── Python Scripts/              # (External dependency)
    ├── hybrid_realtime_pipeline/
    ├── collect_data_update.py   # Gesture data collection
    ├── training_session_ml.py   # ML model training
    └── test_gesture_recognition.py  # Live recognition engine
```

---

## 🎓 Learning Outcomes

This project demonstrates expertise in:
- ✅ **Full-stack development** (C# desktop + REST API + Python ML)
- ✅ **Computer vision pipelines** (MediaPipe integration, pose estimation)
- ✅ **Inter-process communication** (TCP sockets, async messaging)
- ✅ **3D graphics programming** (SharpDX, WebView2)
- ✅ **Office automation** (PowerPoint COM interop)
- ✅ **Cloud integration** (Google Drive API, OAuth)
- ✅ **Database design** (MongoDB schema, user management)
- ✅ **Real-time systems** (low-latency gesture recognition)

---

## 💡 Technical Challenges Solved

- **Cross-language integration** — Seamless C#/Python communication via socket protocol
- **Real-time performance** — Optimized pipeline for <100ms gesture-to-action latency
- **Gesture accuracy** — Robust recognition with per-user training and vector normalization
- **3D rendering** — Integrated WebView2 and SharpDX for smooth model manipulation
- **Multi-threading** — Asynchronous socket listeners for concurrent video/command streams

---

## 🔮 Future Enhancements

- [ ] Support for additional presentation software (Google Slides, Keynote)
- [ ] Multi-hand gesture recognition for collaborative presentations
- [ ] Web-based version using WebRTC and TensorFlow.js
- [ ] Voice command integration for hybrid control
- [ ] Pre-trained gesture packs for instant setup

---

## 👨‍💻 About

**GestPipe** was developed as a capstone project (SEP490) showcasing the intersection of computer vision, desktop application development, and machine learning. It demonstrates production-ready software engineering practices including:
- Modular architecture with clear separation of concerns
- Comprehensive error handling and logging
- User-centric design with internationalization
- Scalable backend with cloud integration

---

## 📄 License

This project is part of an academic portfolio. Please contact the repository owner for usage permissions.

---

<div align="center">
  <strong>⭐ Star this repo if you find it interesting!</strong>
  <br>
  <em>Built with passion for innovation in human-computer interaction</em>
</div>
