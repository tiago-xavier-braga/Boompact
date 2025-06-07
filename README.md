# 🧱 Boompact – Dedicated Server Architecture (Unity + Netcode)

This repository documents the **dedicated server architecture** originally developed for **Boompact**,  
a real-time multiplayer arcade racing game by XaviGames.  

The project is presented here **as a case study and technical reference** for developers interested in  
Unity's multiplayer backend using **Unity Multiplay Hosting**, **Matchmaker**, and **Netcode for GameObjects**.

> 🎯 This repository is no longer the main game project. Instead, it serves as a **standalone showcase of the multiplayer server infrastructure** used in Boompact.

---

## ⚙️ Architecture Overview

This implementation uses a **dedicated server model**, with cloud-hosted headless builds deployed via **Unity Multiplay**.  
Clients connect using **Matchmaker**, and game state synchronization is handled with **Netcode for GameObjects (Netcode for GO)**.

### ✅ Key Features

- Matchmaking through Unity Matchmaker
- Headless server deployment with Unity Multiplay
- Scene management via custom `NetworkSceneLoader`
- Match lifecycle managed by `MatchController` and `BackfillController`
- Local test configuration with `ServicesSettings` ScriptableObject
- No dependency on Unity Relay or Authentication

---

## 📄 License

This project is protected by a **proprietary license** owned by XaviGames. Redistribution, commercial use, or 
modification by third parties is strictly prohibited without prior written consent.

Some environments or 3D assets may have been created by third-party contributors. In such cases, XaviGames retains
commercial usage rights based on written agreements or email-based licenses provided by the original authors.

For more information, see [LICENSE.txt](./LICENSE.txt).

## ✉️ Contact

Developed by **Tiago Xavier Braga**  
📧 xavigames.company@gmail.com  
📧 braga.taigoxavier@gmail.com  
🔗 [LinkedIn](https://www.linkedin.com/in/tiago-xavier-braga/)
