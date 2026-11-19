# Family Time: Night Activities | Unity C# Prototype

**Status: Unofficial fan-made technical concept / not an official mod / not integrated into Family Time.**

This independent example, published by **BUNI Game Lab / Buni Industries**, explores how optional nighttime activities could provide *small, meaningful rewards* in a wolf-family survival game. It was inspired by a community suggestion for **Family Time**, developed by **sgthale**, but uses **no game assets, source, private APIs, or character art**.

Official game: https://sgthale.itch.io/family-time | Steam: https://store.steampowered.com/app/5072600/

The developer lists **Unity** as the engine on itch.io. **Unity C#** is therefore a suitable illustrative language. **The developer's internal class names, architecture, version, and APIs are not public here**, so this sample must be adapted before any genuine integration.

## The idea

**Wolf Mama's Second Chance:** at night, complete a simple campfire bonding/romance minigame. Success gives **one forgiveness charge per run**. When an otherwise fatal Wolf Mama survival consequence occurs, the host game can consume the charge to *cancel that one consequence*. The romance reward cannot be earned again in the same run, even after loading a save. It does **not** protect against unrelated deaths.

Other example activities (all values are adjustable prototypes, not actual game statistics):

| Night activity | Illustrative reward | Limit |
| --- | --- | --- |
| Campfire romance with Wolf Mama | One fatal-failure forgiveness | Earn once per run |
| Cooking together | +2 food portions | Once per night |
| Hunting practice | +10% next-hunt bonus | Once per night; cannot stack pending bonuses |
| Playing with pups | +5 pup morale | Once per night |

The API receives nighttime status and a monotonically increasing night number *from the host*. This avoids assuming any particular current day/night implementation, save schema, or internal strike counter. The demo uses a **three-warning threshold purely as an example**, not a verified game constant.

## How to try the demo in Unity

1. Start a **new, empty Unity project** (a supported Unity version with C#). Do **not** copy files into the original game's installation.
2. Copy `Runtime/` and `Demo/` into `Assets/FamilyTimeNightActivities/` of your own test project.
3. Create an empty scene and add an empty GameObject named `NightActivitiesDemo`.
4. Attach `WolfMamaNightActivitiesDemo` to that GameObject.
5. Press **Play**. The Unity OnGUI screen lets you switch day/night, play the three-choice campfire activity, claim rewards, simulate fatal consequences, and save/load an example run.
6. For tests, also copy `Tests/` into `Assets/FamilyTimeNightActivities/`, install/enable the Unity Test Framework, and run the Edit Mode suite in **Window > General > Test Runner**.

No prefabs, commercial asset packs, game binaries, networking, or external dependencies are required. The demo is deliberately plain so the underlying rules are easy to inspect.

## Integration pseudocode (NOT a real game hook)

```csharp
// Create once per run or restore from an existing save snapshot.
var activities = new NightActivityService(savedNightActivityState);

// Only after the real minigame validates success:
var result = activities.TryComplete(
    NightActivityType.Romance, isCurrentlyNight, currentNightNumber, minigamePassed);
if (result.Success) SaveRun(activities.Snapshot());

// In the host game's own FINAL Wolf Mama failure branch, before ending the run:
if (wolfMamaConsequenceWouldEndRun && activities.TryForgiveFatalWolfMamaFailure())
{
    SaveRun(activities.Snapshot()); // Persist consumption before resuming.
    ShowWolfMamaMercyScene();
    // Host game decides how to reduce/retain strikes; no new food or shelter is granted.
    return;
}

// Otherwise continue the game's existing failure handling.
```

**Important:** `SaveRun`, `ShowWolfMamaMercyScene`, `isCurrentlyNight`, `currentNightNumber`, and `wolfMamaConsequenceWouldEndRun` are **placeholder integration points**, not actual Family Time APIs. The sample must not be described as a working game plugin.

## Files

- `Runtime/NightActivityService.cs`: standalone state machine and reward rules, independent of UnityEngine.
- `Demo/WolfMamaNightActivitiesDemo.cs`: Unity MonoBehaviour GUI, three-choice campfire romance minigame, simulated consequence, and demo save/load.
- `Tests/EditMode/NightActivityServiceTests.cs`: Unity Test Framework tests for one-use charge, eligibility, persistence, and anti-farming boundaries.
- `CLAUDE.md`: instructions for Claude Code to evolve this prototype safely.
- `LICENSE`: MIT license for **original code in this folder only**. It does not cover the game's characters or assets.

## Design boundaries / open questions for the developer

1. Should the romance interaction be repeatable for story purposes after the reward is already earned? (This code locks only earning the reward, not narrative encounters.)
2. At exactly which terminal Wolf Mama consequence should forgiveness trigger? The game owner knows the actual failure rules.
3. What should the developer do with warnings/strikes after mercy? The demo resets to one warning before its example threshold; the real game may differ.
4. What should save/load do for runs in progress, and when can the data safely be persisted atomically?
5. Which rewards and thresholds best preserve difficulty as the family grows?

## Validation and scope

The source and NUnit test cases are provided for review, but **Unity Editor compilation and playtesting have not been performed in this environment**. No claims are made about compatibility with actual Family Time builds. The code should be reviewed by its developer before use.

This is an independent, respectful suggestion, not an affiliation, endorsement, official contribution, or assertion of access to sgthale's project. No image, audio, text, or code from the commercial game is redistributed.
