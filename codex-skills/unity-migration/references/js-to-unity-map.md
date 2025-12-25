# Shelter 2250: JS → Unity mapping (core)

This is a pragmatic mapping to get to a playable Unity prototype quickly (simulation parity first).

## Core runtime objects

- `js/game.js` → `GameController` (MonoBehaviour) + `GameTickService` (pure C#)
  - Keep *render* and *simulation tick* separated.
  - “Speed buttons” should only change a single `TimeScale` variable that the tick uses.

- `js/resources.js` → `ResourceSystem` (pure C#)
  - Keep resources as a dictionary keyed by enum:
    - `ResourceType.Power/Water/Food/Medical/Research/Metal/Parts/Caps`
  - Store: `{ current, max, production }` as a struct/class.

- `js/shelter.js` + `js/room.js` → `ShelterGrid` (pure C#) + `RoomInstance` (data) + `RoomView` (MonoBehaviour)
  - `RoomTypeDefinition` (ScriptableObject): name/icon/color/cost/size/production rules
  - `RoomInstance`: id, type, x,y,w,h, level, building progress, assigned dweller ids

- `js/dweller.js` → `Dweller` (data) + `DwellerSystem` (pure C#) + `DwellerView` (MonoBehaviour later)
  - Data: stats, health/maxHealth, happiness, hunger/thirst, radiation, fatigue, traits, status
  - System: tickAll(penalties, speedMultiplier)

## UI

- `index.html` + `css/style.css` → one of:
  - UGUI: Canvas + prefabs (fastest)
  - UI Toolkit: UXML/USS (more scalable, slightly higher setup)
  - Hybrid: HUD (UGUI), panels (UI Toolkit)

Keep UI passive: it should render state from `GameState` and call commands (build/assign/upgrade).

## Save/Load

- LocalStorage save → `SaveService`
  - PC/Mac: `Application.persistentDataPath` + JSON file
  - WebGL: PlayerPrefs (size limits) or IndexedDB plugin later

## IDs and references

- JS uses `Date.now()+Math.random()` ids. In Unity:
  - Use `Guid.NewGuid().ToString()` or a monotonic `int` id generator stored in save.
  - Never store direct object references in save; store ids.

## Suggested folder structure (Unity project)

- `Assets/_Project/Scripts/Core` (tick, state, save)
- `Assets/_Project/Scripts/Gameplay` (rooms, shelter, dwellers)
- `Assets/_Project/Scripts/UI` (views, presenters)
- `Assets/_Project/Data` (ScriptableObjects)
- `Assets/_Project/Prefabs`
- `Assets/_Project/Scenes`

