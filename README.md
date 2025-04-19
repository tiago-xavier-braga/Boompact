# 💣 Boompact

**Boompact** is an online multiplayer arcade racing game focused on fast-paced, chaotic, and explosive matches.  
Players drive cars inside an arena and must pass bombs to others through collisions before the timer runs out —  
or explode!

## 🎮 Gameplay

- Players start the match with bombs randomly assigned.
- A player holding a bomb must collide with another car to transfer it.
- When the round timer ends, anyone still holding a bomb is eliminated.
- The game ends immediately after the first and only round.
- Matches are designed to be quick, energetic, and suitable for casual competition.

## 📦 Technologies Used

- **Unity Matchmaker**
- **Unity Hosting**

## 🌐 Platforms

Boompact will be published on online web game platforms with monetization based on non-intrusive ads.  
An Android version may be considered depending on the game's performance on web platforms.

## 🧪 Development Status

> The project is currently under MVP development, with full focus on the online multiplayer system.  
> Source code may be made available publicly for technical evaluation and interview purposes only.

## ⚙️ Local Testing Setup

Boompact includes a flexible configuration system that allows local client/server simulation without relying on 
Unity Lobby, Relay, or Authentication services.

### 🧰 `ServicesSettings` ScriptableObject

A custom configuration asset located at:

```
Xavi Games > Services > ServicesSettings
```

Contains the following options:

- `BuildType`: sets whether the build is a Client or Server
- `BuildServiceType` and `ClientServiceType`: choose between **Local** or **Cloud**
- `QueueName`: matchmaking queue name
- `TestServerIP`: IP for local testing (default: `127.0.0.1`)
- `TestServerPort`: port used for local server (default: `7777`)

> ⚠️ Local multiplayer works with multiple Unity instances. No external services required.

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

---

> Made with 🚗💣 by XaviGames – 2025
