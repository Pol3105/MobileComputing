# Samurai Defense 🏯🧟

**Samurai Defense** is a 2D action-strategy mobile game developed in Unity.

---

## 🛠️ Technologies
* **Engine:** Unity (2D)
* **Language:** C#
* **Input:** Unity New Input System

---

## 📅 Development Diary

### Current Status: Final Version (1.0) - Game Completed! 🚀

The project features a complete and polished core game loop: Tutorial -> 5-Level Campaign with progressive difficulty and content -> Upgrades Shop (Dojo) -> Final Boss Battle -> Cinematic Credits Scene and New Game option.

### Phase 1: Core Systems & Animation 🎮🎨
* **New Input System:** Full implementation of Unity's modern input system.
* **Movement:** The character (Samurai) moves from left to right using a keyboard (WASD/Arrows) or Gamepad.
* **Code Logic:** Modified the `SamuraiControl.cs` script to handle player inputs and character states.
* **Sprites & Physics:** Imported the Samurai *sprites* and adjusted the **Box Colliders** and ground boundaries to ensure the character walks properly on the terrain.
* **Animation System:** Implemented **Idle** and **Run** states managed by the `Walking` boolean in the Animator.
* **Basic Combat System:** Integrated the sword slash animation and configured transition arrows. Implemented code logic to "freeze" the character's movement while attacking.
* **Camera:** Created the `CamaraMovements.cs` script for a Smooth Follow camera.

---

### Phase 2: Core Loop Completed, Automation, and Feedback 🏹

In this phase, actual combat was defined. The game evolved from manual controls to a smart "Auto-Battler" system, and visual feedback systems (health bars, damage) were integrated.

#### 1. Bug Fixes and Polish 🛠️
* ✅ **Cooldowns:** Fixed the "Double Attack" bug by implementing wait times.
* ✅ **Multi-Hit:** Used lists to prevent a single attack from damaging multiple enemies multiple times in a single frame.
* ✅ **Pivots:** Adjusted sprites to "Bottom Center" to fix visual jumping during animations.

#### 2. Combat System: Smart "Auto-Battler" ⚔️🏹
* **Automation:** Manual attacking was removed. The Samurai now manages combat autonomously:
    * **Priority 1 (Short Range):** If it detects an enemy at close range -> **Sword**.
    * **Priority 2 (Long Range):** If no one is close, it checks long range -> **Bow**.
* **Auto-Aiming (Trigonometry):** Implemented `Mathf.Atan2` so arrows calculate the exact trajectory towards the enemy's chest, compensating for height differences.
* **Action Locking:** The character only attacks if standing still and the enemy is truly in front of them (dot product), preventing "sliding" across the floor.

#### 3. Advanced Artificial Intelligence (AI) 🧠
* **Improved Sensors:** Zombies use a filtered `OverlapCircleAll` to ignore the player's sensors (like the attack hitbox) and only stop at the physical body.
* **Health Bars (World Space UI):** Each enemy has its own floating health bar that moves with it.
* **Horde Physics:** Adjusted the *Collision Matrix* so enemies overlap with each other (preventing chaotic pushing) but still collide with the player.

#### 4. Environment and Defense ⛩️💀
* **Sacred Tower:** Defendable object with a health system and Sprite swapping (destruction) upon falling.
* **Target Logic:** Enemies identify the tower as a priority target.

---

### Phase 3: Level Management, Economy, and Allies 🌊💰👨‍🌾

In this phase, the action prototype was transformed into a full strategy game, adding resource management, allied units, and game structure.

#### 1. Game Structure (Game Loop) 🌊
* **Wave System:** Configurable `GeneradorEnemigos` script with wave lists (number of zombies and spawn times).
* **Victory Logic:** Implemented `WaitUntil` to wait for the last enemy to die before declaring victory.
* **Scene Flow:** Created **Main Menu**, **Game** screen, and **Victory/Defeat** panels with level reset functionality.

#### 2. Economy and Shop 🪙
* **Coin System:** Enemies drop money upon death.
* **UI:** Real-time coin counter.
* **Summoning:** UI button to buy allies (Farmers) if the player has enough balance.

#### 3. Allies System (Farmers) 👨‍🌾
* **Ally AI:** Created the `GranjeroIA` script, which detects zombies and fights in melee combat.
* **Interaction:** Zombies now recognize allies as valid targets and attack them.
* **Health Bars:** Implemented floating UI (World Space) for allies as well.
* **Logic Fixes:** Fixed "shooting at corpses" issues (checking `estaMuerto` before attacking) and death physics (disabling gravity).

#### 4. Audio and Feedback 🔊
* **Sound Effects (SFX):** Integrated audio for sword slashes and arrow shots.
* **Music (BGM):** Distinct audio tracks for the Menu and Battle.
* **Dramatic Silence:** Logic in the `GameManager` to stop the music upon winning or losing.

---

### Phase 4: Archers, Kamikaze Ghosts, and AI Improvements 🏹👻✨

In this phase, strategic depth was added by introducing ranged units and flying "kamikaze" enemies, forcing the player to better manage their defenses.

#### 1. New Allied Unit: Archer 🏹
* **State Machine:** Implemented `Run` ↔ `Idle` ↔ `Attack` logic. The unit stops automatically when detecting enemies, shoots, and moves forward again.
* **Animation Events:** A professional solution for shooting. **Animation Events** were implemented to instantiate the arrow on the exact *frame* of the animation.
* **Projectile Physics:** Calculated rotation and direction so arrows fly toward the target.

#### 2. New Enemy: Kamikaze Ghost 👻
* **Flying Unit:** An enemy that ignores collisions with other ground units.
* **Suicidal Behavior:** Basic AI that moves straight toward the target and deals immediate damage upon impact, destroying itself in the process.
* **Counterplay:** Introduces the actual need to use Archers to shoot them down before they reach the tower or the player.

#### 3. Advanced Enemy AI (Refactoring) 🧠
* **Multi-Target Radar:** Updated the `ZombiIA` script to detect three priorities: **Tower**, **Samurai**, and **Allies**.
* **Target Management:** Zombies now dynamically switch targets if their current victim dies or escapes.

#### 4. Economy 🎨💰
* **Shop Updated:** Modified `GeneradorAliados` to include purchasing Archers (Cost: 15 coins).

---

### Phase 5: Visual Feedback, Bosses, Advanced Waves, and Full UI 🩸🧟‍♂️🌊⏸️

In this phase, the complete game cycle was finalized, improving the impact sensation (*Game Juice*), adding a final challenge (Boss), rewriting the wave system, and finishing all user interfaces and scene navigation.

#### 1. Visual Feedback (Game Juice) 🩸
* **Blood System:** Implemented particles upon taking damage. Now, every hit on an enemy instantiates a visual effect that disappears after a few seconds.

#### 2. The Final Boss 💪
* **Giant Variant:** Created a new "ZombiBoss" enemy (1.5x Scale).
* **Buffed Stats:** Adjusted health (x5), high damage, and reduced speed to create a "tank".

#### 3. Flight Mechanic (Ghost) 👻
* **Height Sensor:** `VueloFantasma` script using a Raycast to detect the ground.
* **Dynamic Hovering:** Automatic vertical adjustment to climb ramps without standard gravity physics.

#### 4. Waves System 2.0 🌊
* **Parallel Spawning:** New logic that allows simultaneous spawning of different enemy types.
* **Tag Detection:** The system now waits for the screen to be cleared by detecting the `Enemigo` Tag.

#### 5. User Interface (UI) and Flow 🖥️
* **State Management:** Victory, Defeat, and Pause screens with time freezing (`Time.timeScale`).
* **Navigation:** Configured `Build Settings` to connect the Menu and Game scenes.

---

### Phase 6: Mobile Controls, Balancing, and Visual Art 🕹️⚖️🎨

The focus of this phase was transforming the project into a final Android product by implementing touch controls, balancing difficulty, and drastically improving aesthetics.

#### 1. Touch Controls (Mobile Ready) 📱
* **Virtual Joystick:** Implemented the `On-Screen Stick` component from the New Input System.
* **Codeless Integration:** Mapped the Joystick to the `Player/Move` action (Gamepad Left Stick), allowing the Samurai to move on mobile without modifying its original script.
* **Controller UI:** Visual design of the joystick (Knob and Background) on the Canvas.

#### 2. Art and Environment (Background) 🌸
* **Parallax/Static Background:** Imported 2D art (Feudal Japan) to replace the gray background.
* **Sorting Layers:** Created and configured rendering layers (`Fondo` vs `Default`) to ensure the environment stays behind the characters.
* **Invisible Ground:** Design technique that hides the physical ground's SpriteRenderer (`Floor`), allowing the player to interact with the collider while visually "walking" on the drawn path in the background.

#### 3. Game Balancing (Game Design) ⚖️
* **Difficulty Curve:** Adjusted key variables to make the game fun but challenging.
    * **Boss:** Massive health increase and coin reward.
    * **Ghosts:** High speed and damage, but low health ("Kamikaze" style enemies).
    * **Economy:** Adjusted unit costs (cheap Farmer, expensive Archer) and starting coins.

#### 4. Typography and UI Aesthetics ✍️
* **TextMeshPro Font Assets:** Imported and "baked" (SDF) stylized Japanese fonts to improve the visual quality of titles and menus.

---

### Phase 7: Game Juice, Visual Effects, and Pro UI 🏆🎆✨

The goal of this phase was to elevate visual quality ("Game Juice") and user experience (UX). Cinematic systems, persistent particle effects, and a totally revamped thematic interface were implemented.

#### 1. Cinematic Camera System 🎥
* **Dynamic Zoom:** Implemented automatic camera sequences:
    * **Intro:** Smooth zoom-in on the Samurai when starting the match.
    * **Outro:** Panoramic zoom-out upon winning or losing to show the final state of the battlefield.
* **Real-Time Management:** Used `Time.unscaledDeltaTime` in camera scripts to allow fluid animations even when the game is technically paused (`TimeScale = 0`).

#### 2. Visual Effects (VFX) and Feedback 🎇
* **Fireworks System:** Created the `LanzadorFestivo` and `ProyectilFestivo` logic.
    * **Custom Physics:** Rockets with straight trajectories and dynamic rotation so the tip always faces its flight direction.
    * **Ignore Pause:** Configured `Particle Systems` to `Unscaled Time` mode so explosions continue animating behind the Victory menu.
* **UI Animations:** Entry effects ("Pop-up" with bounce) for Victory and Defeat banners.

#### 3. Total Interface Renovation (HUD) 🎨
* **Thematic Aesthetics:** Replaced placeholder buttons with "Samurai" style assets (wood, scrolls, golden frames).
* **Iconography:** Integrated coin icons into unit costs and the global counter to unify the visual language.
* **Clean Layout:** Reorganized the Canvas, clearly separating the gameplay HUD, Pause Menu, and End Game screens.
* **Victory Screen:** High-impact design with a thematic logo, particles, and integrated buttons.

#### 4. Pause System ⏸️
* **Game Logic:** Full implementation of `PausarJuego()` and `ReanudarJuego()` managing `Time.timeScale`.
* **UI Integration:** Functional pause menu with options to resume or return to the main menu.

---

### Phase 8: Interactive Tutorial, Dialogues, and Ghosts 📜👻

In this phase, we focused on the new player experience (Onboarding) and adding strategic depth with aerial enemies and combat physics corrections.

* **Tutorial Manager:** Created a manager script that controls game flow through phases (Intro -> Combat -> Buy Farmer -> Horde -> Buy Archer).
* **Dialogue System:** Implemented an "Ancient Scroll" UI that pauses the game (`Time.timeScale`) to give instructions to the player.
* **Flying Enemies (Ghosts):** `VueloFantasma` script using Raycast to hover at a constant height above the terrain's relief.
* **Archer AI Improvement:** Aim correction by calculating angles and gravity-less physics for arrows.

---

### Phase 9: Meta-Game, Upgrade System (Dojo), and Level 1 ⛩️📈

In this phase, the game was transformed from a single-level experience into a full progression system, implementing persistence and an upgrade shop.

* **Save System (`DatosJugador`):** Static script based on `PlayerPrefs` to save progress and the new currency: "Upgrade Points" (Tokens).
* **The Dojo:** Permanent upgrade shop with evolving graphics and a "Respec" (Reset) mechanic to recover points.
* **Stat Scaling:** Allies now read their saved data when starting a match to dynamically modify health, damage, and range.
* **First Campaign Level (Level 1):** Created the `Nivel_1` scene with anti-farming protection for delivering upgrade points upon completion.

---

### Phase 10: Campaign Design, New Troops, and Final Boss 🥷👑🎬

In this final major development phase, the game was completed. The exact progression for all 5 levels was designed, advanced troops were integrated, interfaces were revamped, and an epic narrative and playable conclusion was created.

#### 1. Level Progression (Full Campaign) 🗺️
* **Global Interface Adjustments:** Reviewed, anchored (Anchors), and unified interfaces (health bars, buttons, menus) across all levels to ensure consistency on multiple resolutions.
* **Level 2 (The Tank):** Introduced the **Ronin**, a new melee allied unit with massive area damage and a multi-target radar, ideal for stopping large hordes.
* **Level 3 (Ranged Threat):** Implemented a new background scenario. Introduced **Zombie Archers**, forcing the player to adapt their defenses.
* **Level 4 (Explosions):** Unlocked the **Ninja**, the ultimate allied unit. Throws bombs with Area of Effect damage and features a smart "Anti-Air Filter" (ignores ghosts to clear ground zombies).
* **Dynamic Scaling:** Enemies increase their stats (health and damage) depending on the level they appear in, using the `EscalarEstadisticas(nivel)` function.

#### 2. Final Battle: The Ice King (Level 5) 👑🧊
* **"The Awakening" Mechanic:** The Boss enters in a dormant state. He has a timer before advancing, but will "awaken" instantly if an ally enters his detection range.
* **Immovable Physics:** Used `RigidbodyConstraints2D` to lock the Boss's X-axis while attacking or sleeping, preventing zombie hordes from pushing him.
* **Simultaneous Hordes:** The `GeneradorEnemigos` counts living enemies selectively (ignoring the Boss), allowing waves to keep flowing continuously while the boss is still alive.

#### 3. Narrative Closure and Final Polish 📜✨
* **Credits Scene:** Created a cinematic slow-motion transition (`Time.timeScale = 0.5f`) upon defeating the Ice King.
* **Farewell Dialogues:** The Mentor returns for a few final words of victory using the scroll system (adapted to the *New Input System*).
* **Hard Reset ("New Legend"):** Implemented a final button that uses `PlayerPrefs.DeleteAll()` and clears the upgrades shop to allow full replayability from scratch.

---

## 🚀 Future Possible Expansions

Even though the base game is fully complete (v1.0), the scalable architecture allows for easy content additions:
* 🔲 **Survival Mode:** An infinite level where waves generate procedurally and difficulty scales endlessly.
* 🔲 **New Environments:** Different biomes and terrain mechanics (e.g., areas that slow down characters).
* 🔲 **Active Abilities:** On-screen buttons allowing the player to manually cast spells (e.g., Arrow Rain) with cooldowns.
* 🔲 **Animation Improvements:** Fix minor animation bugs.

---