# Asset Credits and Usage

Every third-party asset used in Chronicles of the Lost Dungeon, where it came from, its license, and what it is used for in the game. Only files that are actually used are kept in the repository.

## Art

| Asset | Author | Source | License | Used for |
|---|---|---|---|---|
| FREE Knight 2D Pixel Art | Mattz Art | https://xzany.itch.io/free-knight-2d-pixel-art | Free for personal and commercial projects | Player character; recoloured and scaled up for the Lost King boss |
| Monsters Creatures Fantasy | LuizMelo | https://luizmelo.itch.io/monsters-creatures-fantasy | CC0 1.0 | Enemies: Goblin (Warrior), Flying Eye (Archer), Skeleton with shield (Guardian), Mushroom (boss minions) |
| GothicVania Church Pack | ansimuz | https://ansimuz.itch.io/gothicvania-church-pack | Free assets: personal and commercial use, modification allowed | Crypt and Throne environments, backgrounds, props |
| Free Dungeon Platformer Game Tileset Pixel Art | CraftPix | https://free-game-assets.itch.io/free-dungeon-platformer-game-tileset-pixel-art | CraftPix free license (commercial use allowed) | Catacombs, Mines and Library tiles and props |
| UI Pack: Pixel Adventure | Kenney | https://kenney.nl/assets/ui-pack-pixel-adventure | CC0 1.0 | Menu panels, buttons, sliders |
| Game Icons | Kenney | https://kenney.nl/assets/game-icons | CC0 1.0 | Settings, pause and level select icons |

## Audio

| Asset | Author | Source | License | Used for |
|---|---|---|---|---|
| Dungeon Ambience | yd | https://opengameart.org/content/dungeon-ambience | CC0 1.0 | Exploration music in every chamber |
| Heavy Dungeon | MintoDog | https://opengameart.org/node/181405 | CC0 1.0 | Boss fight music |
| Spooky Dungeon | Memoraphile (You're Perfect Studio) | https://opengameart.org/node/87564 | CC0 1.0 | Main menu and level select music |
| Impact Sounds | Kenney | https://kenney.nl/assets/impact-sounds | CC0 1.0 | Sword hits, enemy hits, blocks |
| RPG Audio | Kenney | https://kenney.nl/assets/rpg-audio | CC0 1.0 | Footsteps, chests, doors, coins, sword swings |
| Interface Sounds | Kenney | https://kenney.nl/assets/interface-sounds | CC0 1.0 | UI clicks, hover, denied, toggles |
| Music Jingles | Kenney | https://kenney.nl/assets/music-jingles | CC0 1.0 | Level complete and game over stingers |

## Fonts

| Asset | Author | Source | License | Used for |
|---|---|---|---|---|
| Kenney Fonts | Kenney | https://kenney.nl/assets/kenney-fonts | CC0 1.0 | All UI text |

## Pixel Scale Standard

All sprites follow one rule so every pack renders at the same visual scale:

- **16 pixels per unit is the base.** One 16px tile is exactly one Unity unit, and every tile grid in the game is 1 x 1 units.
- Packs drawn at a different scale are imported from a folder whose name ends in `_ppuNN` (for example `Tiles/CraftPixDungeon_ppu32`). The editor tool `PixelArtImporter` reads that tag and sets the matching Pixels Per Unit, so a 32px tile still fills one unit.
- Every texture under `Assets/_Project/Art/` is imported automatically as a Sprite with Point filtering, no compression, no mipmaps and Full Rect mesh. This keeps pixels sharp on Windows, WebGL and Android.
- The Knight, GothicVania and LuizMelo art are all drawn at the same pixel size, so they are imported at 16 PPU unchanged. The hero stands about 2.25 tiles tall.
- The CraftPix pack is drawn at 4x finer detail (64px tiles). Its tiles, props and items were downscaled 4x with box filtering and a hard alpha threshold so one CraftPix pixel matches one Knight pixel. Its parallax backgrounds were downscaled 2x, since distant layers can stay slightly finer.
- Kenney UI icons are smooth, not pixel art, and are only used on the UI Canvas, so their pixel density does not affect the world.
- The camera uses the URP Pixel Perfect Camera with Assets Pixels Per Unit set to 16.

## Imported Files

| Folder in project | Pack | Files | PPU | Notes |
|---|---|---|---|---|
| `Assets/_Project/Art/Sprites/Characters/Knight` | FREE Knight 2D Pixel Art | 12 | 16 | Original pixels, 112x96 frames |
| `Assets/_Project/Art/Tiles/GothicVania` | GothicVania Church Pack | 3 | 16 | Original pixels, 16px tiles |
| `Assets/_Project/Art/Sprites/FX` | GothicVania Church Pack | 2 | 16 | Original pixels |
| `Assets/_Project/Art/Tiles/CraftPixRock, Art/Tiles/CraftPixLava` | Free Dungeon Platformer Tileset (CraftPix) | 50 | 16 | Downscaled 4x (64px to 16px) |
| `Assets/_Project/Art/Sprites/Props` | Free Dungeon Platformer Tileset (CraftPix) | 113 | 16 | Downscaled 4x |
| `Assets/_Project/Art/Backgrounds` | Free Dungeon Platformer Tileset (CraftPix) | 14 | 16 | Parallax layers, downscaled 2x |
| `Assets/_Project/Art/UI/Panels` | UI Pack: Pixel Adventure (Kenney) | 2 | 16 | Packed tilesheets, 16px and 8px grids |
| `Assets/_Project/Art/UI/Icons` | Game Icons (Kenney) | 24 | 16 | White 2x icons for UI Images |
| `Assets/_Project/Fonts` | Kenney Fonts | 3 | n/a | Kenney Pixel, Mini and High |
| `Assets/_Project/Audio/SFX/Combat` | Impact Sounds (Kenney) | 10 | n/a | Original .ogg |
| `Assets/_Project/Audio/SFX/Combat` | RPG Audio (Kenney) | 5 | n/a | Original .ogg |
| `Assets/_Project/Audio/SFX/World` | RPG Audio (Kenney) | 14 | n/a | Original .ogg |
| `Assets/_Project/Audio/SFX/UI` | Interface Sounds (Kenney) | 12 | n/a | Original .ogg |
| `Assets/_Project/Audio/Jingles` | Music Jingles (Kenney) | 4 | n/a | Original .ogg |
| `Assets/_Project/Art/Sprites/Enemies` | Monsters Creatures Fantasy (base pack and Version 1.3) | 28 | 16 | Original pixels, 150x150 frames; includes projectile, bomb and sword sprites |
| `Assets/_Project/Audio/Music` | Dungeon Ambience, Heavy Dungeon, Spooky Dungeon (OpenGameArt) | 3 | n/a | Renamed by use: Exploration, Boss, Menu |

