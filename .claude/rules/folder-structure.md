# Folder Structure

This is a single Godot 4.7 game. Top-level folders are split by file type, with a
subfolder per feature inside each.

```
res://
├── addons/                # third-party and shared plugins, never edited in place
├── assets/                # media, grouped by kind
│   ├── materials/
│   ├── models/
│   ├── textures/
│   ├── audio/
│   └── fonts/
├── scenes/                # .tscn, one subfolder per feature
│   ├── levels/            # e.g. lvl_main.tscn
│   ├── <feature>/
│   └── ui/
├── scripts/               # .gd, one subfolder per feature
│   ├── <feature>/
│   └── ui/
├── resources/             # custom .tres data, one subfolder per feature
├── docs/                  # gdd.md, roadmap.md, design notes
├── icon.svg
└── project.godot
```

Rules:

- A feature is one gameplay or system area (e.g. `player/`, `enemies/`,
  `inventory/`). Name it after what it is, not after the scene type.
- Scenes go in `scenes/<feature>/`, scripts in `scripts/<feature>/`. The
  feature folder name matches in both (`scenes/<feature>/<thing>.tscn` +
  `scripts/<feature>/<thing>.gd`).
- Level scenes live in `scenes/levels/` and are prefixed `lvl_`.
- Materials are prefixed `mat_`, textures `txt_`, and live under `assets/`.
- Custom resources (`.tres` data defined by a `Resource` script) go in
  `resources/`; the script that defines them stays in `scripts/`.
- Addons are copied from their source repository; fix them there and copy them
  again instead of editing `addons/` here.
- Project documentation lives in `docs/`.
- Don't create empty folders "for the future". A folder only exists once it
  has content — this includes the subfolders above.
- Build output (web export) is never committed; it goes in `build/`, which is
  git-ignored.
