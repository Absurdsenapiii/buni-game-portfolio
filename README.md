# BUNI / Game Portfolio

### Narrative games. Real systems. Honest progress.

A development showcase from **Buni Media Hub**, part of Buni Industries.

This portfolio documents the engineering behind **POTTAH!**, a work-in-progress Godot narrative game. It focuses on what has been built, what has been tested, and what still needs to earn a release.

**Status: in development** · **Engine: Godot 4.6.3 / GDScript** · **Current focus: Windows**

[Project case study](docs/pottah-case-study.md) · [Verification and release gates](docs/verification.md)

## Featured project: POTTAH!

A branching narrative prototype where choices, remembered events and replay state affect the story.

The engineering work covers:
- Branching dialogue and ending conditions.
- Save/load, migration and recovery from damaged or incomplete state.
- Replay-aware narrative, evidence interactions and a journal.
- Keyboard navigation, input handling and reduced-motion behavior.
- Separate music, voice and sound-effect controls, with safe missing-audio handling.
- Source and packaged-build regression tests.

The runtime uses local game data and its own save directory. It does not need an online AI service or backend to play.

## Engineering highlights

| Area | What the project explores |
| --- | --- |
| Story state | Keeping branching decisions, evidence and endings consistent |
| Save reliability | Recovering safely without inventing missing player history |
| Input | Preventing held keys or clicks from selecting unintended choices |
| Audio | Interrupting, replaying and stopping dialogue without stale playback |
| Testing | Checking both source behavior and packaged game data |

## Progress snapshot

**12 November 2026:** milestone 47 delivered; milestone 48 in testing at the time of this snapshot.

The milestone 47 development report records **4,581 physical runtime lines** and passing source, Python and packaged test suites. Those latest results have not yet been independently repeated. Line count describes scope, not game quality or completion.

An earlier independent checkpoint review reproduced passing source and packaged tests, then identified a validation-wrapper defect. That review also explicitly withheld release acceptance. See [verification](docs/verification.md) for the exact evidence boundaries.

## What is not finished

Production art/audio integration, listening and pacing review, fresh visual playthroughs, Windows/controller testing and release packaging are still open. An intermittent crash also remains under investigation.

There is **no finished-game download or store release in this repository**. No unverified completion percentage is presented.

## About this repository

This is a public portfolio and case study, not the game's source distribution. Private source, working builds, unpublished story files, artwork, audio, credentials and internal production material are not included.

No third-party endorsement or affiliation is claimed. A portfolio entry is not a licence to redistribute game content or third-party intellectual property.

**Build it. Test it. Show what the evidence supports.**
