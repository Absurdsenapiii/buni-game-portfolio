using UnityEngine;
using Buni.FanPrototype.FamilyTimeNightActivities;

/// <summary>
/// Standalone IMGUI proof of concept. Attach to an empty GameObject in a NEW Unity scene.
/// No Family Time assets, classes, saves, or binaries are required.
/// </summary>
public sealed class WolfMamaNightActivitiesDemo : MonoBehaviour
{
    private const string SaveKey = "BUNI_NIGHT_ACTIVITIES_PROTOTYPE_V1";
    private NightActivityService _service;
    private int _nightNumber = 1;
    private bool _atNight = true;
    private int _strikes;
    private int _food;
    private int _morale;
    private int _nextHuntBonus;
    private string _message = "Play a nighttime activity to earn a reward.";

    // A deliberately tiny example choice minigame, not official character dialogue.
    private static readonly string[][] Choices = {
        new[] { "Invite Mama to the campfire", "Walk away" },
        new[] { "Help care for the pups", "Ignore the pups" },
        new[] { "Share a quiet moment", "Demand an instant reward" }
    };
    private int _romanceStep = -1;
    private int _romanceScore;

    private void Awake() { NewRun(); }

    private void NewRun()
    {
        _service = new NightActivityService();
        _nightNumber = 1;
        _atNight = true;
        _strikes = 0;
        _food = 0;
        _morale = 0;
        _nextHuntBonus = 0;
        _romanceStep = -1;
        _message = "New run. Night 1.";
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(16, 16, Mathf.Min(Screen.width - 32, 560), Screen.height - 32), GUI.skin.box);
        GUILayout.Label("BUNI GAME LAB | UNOFFICIAL UNITY C# PROTOTYPE");
        GUILayout.Label("Night " + _nightNumber + " | " + (_atNight ? "Nighttime" : "Daytime"));
        var state = _service.Snapshot();
        GUILayout.Label("Romance earned: " + state.RomanceRewardEarned +
                        " | Forgiveness available: " + state.ForgivenessAvailable +
                        " | Spent: " + state.ForgivenessSpent);
        GUILayout.Label("Example strikes: " + _strikes + "/3 | Food: " + _food +
                        " | Pup morale: " + _morale + " | Next hunt bonus: " + _nextHuntBonus + "%");
        GUILayout.Label(_message);
        GUILayout.Space(8);

        if (_romanceStep >= 0)
        {
            // IMGUI can redraw after a click; capture the step before mutating it.
            int shownStep = _romanceStep;
            GUILayout.Label("Campfire bonding | Choice " + (shownStep + 1) + " of " + Choices.Length);
            for (int i = 0; i < Choices[shownStep].Length; i++)
            {
                int selected = i;
                if (GUILayout.Button(Choices[shownStep][i])) PickRomanceChoice(selected);
            }
            if (GUILayout.Button("Cancel minigame")) _romanceStep = -1;
        }
        else
        {
            if (GUILayout.Button("Romance Wolf Mama (once per run)"))
            {
                if (!_atNight) _message = "This activity requires nighttime.";
                else if (state.RomanceRewardEarned) _message = "Already earned during this run.";
                else { _romanceStep = 0; _romanceScore = 0; }
            }
            if (GUILayout.Button("Cook together (once per night)")) Complete(NightActivityType.Cooking);
            if (GUILayout.Button("Practice hunting (once per night)")) Complete(NightActivityType.HuntingPractice);
            if (GUILayout.Button("Play with pups (once per night)")) Complete(NightActivityType.PlayingWithPups);
        }

        GUILayout.Space(8);
        if (GUILayout.Button("Simulate neglect consequence (demo threshold = 3)")) Neglect();
        if (GUILayout.Button("Use next-hunt bonus"))
        {
            _nextHuntBonus = _service.ConsumeHuntingBonusPercent();
            _message = "Example next-hunt bonus used: " + _nextHuntBonus + "%";
        }
        if (GUILayout.Button(_atNight ? "Go to day" : "Advance to next night"))
        {
            _atNight = !_atNight;
            if (_atNight) _nightNumber++;
            _romanceStep = -1;
            _message = _atNight ? "New night. Activities reset except romance." : "Daytime.";
        }
        if (GUILayout.Button("Save example state")) Save();
        if (GUILayout.Button("Load example state")) Load();
        if (GUILayout.Button("Reset / new run")) NewRun();
        GUILayout.EndArea();
    }

    private void PickRomanceChoice(int selected)
    {
        if (selected == 0) _romanceScore++;
        _romanceStep++;
        if (_romanceStep < Choices.Length) return;
        _romanceStep = -1;
        var result = _service.TryComplete(NightActivityType.Romance, _atNight, _nightNumber, _romanceScore >= 2);
        _message = result.Message;
    }

    private void Complete(NightActivityType activity)
    {
        // Button press stands in for success for OTHER activities; replace with real minigames.
        var result = _service.TryComplete(activity, _atNight, _nightNumber, true);
        if (result.Success)
        {
            _food += result.Reward.FoodPortions;
            _morale += result.Reward.PupMorale;
        }
        _message = result.Message;
    }

    private void Neglect()
    {
        _strikes++;
        if (_strikes < 3) { _message = "Illustrative warning " + _strikes + "/3."; return; }
        if (_service.TryForgiveFatalWolfMamaFailure())
        {
            _strikes = 2; // Demo only: survive this failure, but the next may end the run.
            _message = "Wolf Mama forgives this fatal mistake! Your one extra chance is spent.";
        }
        else
        {
            _message = "Example run ended. Press Reset / new run.";
            _strikes = 3;
        }
    }

    [System.Serializable]
    private sealed class DemoSave
    {
        public NightActivitySaveData NightActivities;
        public int NightNumber;
        public bool AtNight;
        public int Strikes;
        public int Food;
        public int Morale;
    }

    private void Save()
    {
        var save = new DemoSave {
            NightActivities = _service.Snapshot(), NightNumber = _nightNumber,
            AtNight = _atNight, Strikes = _strikes, Food = _food, Morale = _morale
        };
        PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(save));
        PlayerPrefs.Save();
        _message = "Demo save written. Not the game's official save file.";
    }

    private void Load()
    {
        if (!PlayerPrefs.HasKey(SaveKey)) { _message = "No demo save exists."; return; }
        try
        {
            var save = JsonUtility.FromJson<DemoSave>(PlayerPrefs.GetString(SaveKey));
            if (save == null || save.NightActivities == null || save.NightNumber < 1)
                throw new System.InvalidOperationException("Invalid save data");
            _service = new NightActivityService(save.NightActivities);
            _nightNumber = save.NightNumber;
            _atNight = save.AtNight;
            _strikes = save.Strikes;
            _food = save.Food;
            _morale = save.Morale;
            _romanceStep = -1;
            _message = "Demo save loaded.";
        }
        catch (System.Exception error)
        {
            _message = "Could not load demo save: " + error.Message;
        }
    }
}
