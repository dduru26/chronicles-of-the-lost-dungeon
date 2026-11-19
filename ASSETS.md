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
| GothicVania Church music | Pascal Belisle | Included in the GothicVania Church Pack | Free use with credit | Background music |
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
- Characters are not stretched to fit. Their PPU is chosen so the hero stands about 1.75 tiles tall and enemies are sized relative to the hero.
- The camera uses the URP Pixel Perfect Camera with Assets Pixels Per Unit set to 16.

## Imported Files

| Folder in project | Pack | PPU |
|---|---|---|
