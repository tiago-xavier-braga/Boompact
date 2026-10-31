# Boompact

An online multiplayer arcade racing game. Players drive cars around an arena while bombs are
handed out at random. A car carrying a bomb passes it on by crashing into a car without one.
When the round timer runs out, whoever still holds a bomb loses.

> **Status:** early setup. The project, addons and car models are in place; no gameplay yet.
> See the [game design document](docs/gdd.md) for the planned features.

## Documentation

- [Game design document](docs/gdd.md): the rules and the planned features.

## Requirements

- Godot `4.7` (GL Compatibility renderer)
- [LeafTween](https://github.com/tiago-xavier-braga/LeafTween), the tween library, already
  included in `addons/leaf_tween/`
- Save Kit, the save system shared with the other projects, already included in
  `addons/save_kit/`

## Run locally

1. Install [Godot 4.7](https://godotengine.org/download).
2. Open `project.godot` in the editor.
3. Press **F5** to play.

## License

This is a private repository. The source code is proprietary; all rights reserved.
[LeafTween](https://github.com/tiago-xavier-braga/LeafTween), in `addons/leaf_tween/`, is
licensed under MIT.

Third-party assets keep their own licenses:

- The 3D models in `assets/models/car_kit/` and the texture
  `assets/textures/txt_car_kit_colormap.png` are from Kenney's
  [Car Kit](https://www.kenney.nl) and released under CC0 (see
  [kenney_license.txt](assets/models/car_kit/kenney_license.txt)).
