# Claude Code instructions: Family Time Night Activities prototype

## Mission
Extend this **unofficial, clean-room Unity C# example** of optional nighttime activities. The key goal is: **complete a Wolf Mama campfire bonding minigame once per run to gain exactly one extra chance against an otherwise fatal Wolf Mama failure**. Other optional night activities give modest, capped rewards.

## Context and boundaries
- Family Time's **official itch.io page** lists Unity as its engine: https://sgthale.itch.io/family-time
- This sample uses C# because C# is Unity's standard scripting language.
- **Do not pretend** to know the game's internal source, class names, Unity version, death/strike thresholds, save schema, or assets.
- Do not copy, reverse-engineer, decompile, or redistribute closed-source game files, game assets, or third-party artwork. Work only in the supplied standalone files.
- Everything here is an opt-in design proposal; no endorsement or partnership is implied.

## Tasks for further Claude Code development
1. Review `Runtime/NightActivityService.cs` for deterministic state transitions and save/reload loopholes.
2. Add a full Unity scene-based demonstration with primitive shapes, TextMeshPro only if already installed, and no copyrighted assets. Preserve a basic OnGUI fallback.
3. Add tests for null/malformed/old save data, repeat clicks, transitioning between nights, game restarts, and invalid minigame results.
4. Provide an adapter interface and *fake host game* for `isNight`, `nightNumber`, save/load, `onFatalWolfMamaConsequence`, hunting output, food inventory, and pup morale. These are abstractions, not official API names.
5. Run the Unity Edit Mode suite when Unity Editor is available; document editor version and actual pass/fail output. Do not invent successful test results.
6. If the original developer supplies a public API or explicit integration specification, add a separate adapter in a new folder with source citations, rather than changing the independent core rules.

## Acceptance criteria
- One and only one romance-earned rescue per run, even across save/reload.
- Unsuccessful minigames do not grant rewards or consume the chance to retry.
- Rewards cannot be farmed by repeatedly clicking within a single night.
- No effect on unrelated fatal events.
- Second chance does not silently replenish food, housing, or other requirements.
- All test claims reflect actual execution evidence.
- `README.md` remains clear that this is unofficial and not a working mod.

## Example Claude Code task
"Build a nicer zero-asset Unity demo for the Wolf Mama nighttime romance/extra-life system in this repository. Keep gameplay state in NightActivityService, add host adapters only in Demo, and add deterministic NUnit coverage. Do not invent official Family Time APIs. Report exactly which tests you executed."
