# Boompact

An online multiplayer arcade racing game: players drive cars around an arena
while bombs are handed out at random. A car carrying a bomb passes it on by
crashing into a car without one. When the round timer runs out, whoever still
holds a bomb loses. Built in Unity with Netcode for GameObjects.

**Status: discontinued.** Development stopped at the MVP stage. The match loop
runs end to end over Relay, but the game was never released: the web build, ads
and single-round elimination were not finished.

## Requirements

Unity **6000.1.9f1**: Universal Render Pipeline, Input System, Netcode for GameObjects.
The project must be linked to a Unity Cloud project with **Relay** and **Lobby** enabled.
XaviEssencials is pulled in
as a git package (scene references, scene bundles, logger).

## Running

Open the project in Unity and press Play from `Assets/Level/Scenes/MainMenu.unity`.
One instance hosts: it creates a Relay allocation and a public Lobby, then loads
`Environment`. The others join from the room list. Use Multiplayer Play Mode to
run several players from one editor.

The match starts once `MinPlayersInMatch` players are connected and
`StartDelayAfterMinPlayers` has passed. Player count, match length and end-screen
delays live in `Assets/Level/ScriptableObjects/Services/HostSettings.asset`.

## Controls

| Action | Keys |
|---|---|
| Drive | `W` `A` `S` `D` / arrow keys / triggers + left stick |
| Handbrake | `Space` / gamepad east button |
| Camera | mouse / right stick / touch drag |

## Layout

```
Assets/
  Art/          meshes, materials, textures, animations, fonts
  Audio/        sound effects
  Code/         gameplay scripts and shader graphs (toon, outline)
  Level/        scenes, prefabs, ScriptableObjects (host settings, car database, user session)
  Settings/     render pipeline, input, lighting, multiplayer and build profiles
  ThirdParty/   LeanTween, PROMETEO car controller, vehicle and environment packs
```

Organised by resource type. The host runs the match (`MatchController`,
`TeamController`, `CarSpawnController`) and pushes state to clients through RPCs;
each car is a networked object owned by its player.

## What's implemented

- Host and join through Unity Relay, with a Lobby room list
- Car selection carried into the match through a `UserSession` ScriptableObject
- Server-side car spawning, one spawn point per player
- Custom car physics, follow camera, engine and effect sounds
- Random bomb distribution to half of the players at match start
- Bomb transfer on collision, with a cooldown after receiving one
- Match timer, match-over banner and per-player win/lose screen
- Disconnect handling: clients return to the menu when the host leaves

## License

Proprietary, © XaviGames. See [LICENSE.txt](./LICENSE.txt).
