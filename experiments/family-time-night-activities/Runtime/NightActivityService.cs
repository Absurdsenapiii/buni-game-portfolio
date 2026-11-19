using System;

namespace Buni.FanPrototype.FamilyTimeNightActivities
{
    public enum NightActivityType
    {
        Romance,
        Cooking,
        HuntingPractice,
        PlayingWithPups
    }

    // Plain serializable data. Unity JsonUtility serializes fields, not properties.
    [Serializable]
    public sealed class NightActivitySaveData
    {
        public int Version = 1;
        public string RunId = "";
        public bool RomanceRewardEarned;
        public bool ForgivenessAvailable;
        public bool ForgivenessSpent;
        public int LastCookingNight = -1;
        public int LastHuntingNight = -1;
        public int LastPupPlayNight = -1;
        public int PendingHuntingBonusPercent;

        public NightActivitySaveData Copy()
        {
            return (NightActivitySaveData)MemberwiseClone();
        }
    }

    // Rewards are returned to the host game, never silently applied to its inventory.
    public sealed class NightActivityReward
    {
        public bool ExtraLifeCharge;
        public int FoodPortions;
        public int PupMorale;
        public int HuntingBonusPercent;
    }

    public sealed class NightActivityResult
    {
        public readonly bool Success;
        public readonly string Message;
        public readonly NightActivityReward Reward;

        public NightActivityResult(bool success, string message, NightActivityReward reward = null)
        {
            Success = success;
            Message = message;
            Reward = reward ?? new NightActivityReward();
        }
    }

    /// <summary>
    /// Unofficial standalone rules model. It knows nothing about real Family Time code.
    /// The host supplies nighttime, night number, and an externally validated minigame result.
    /// </summary>
    public sealed class NightActivityService
    {
        private readonly NightActivitySaveData _state;

        public NightActivityService(NightActivitySaveData saved = null)
        {
            _state = saved == null ? new NightActivitySaveData() : saved.Copy();
            if (string.IsNullOrEmpty(_state.RunId))
                _state.RunId = Guid.NewGuid().ToString("N");

            // Never resurrect a consumed charge from inconsistent saved flags.
            if (_state.ForgivenessSpent || !_state.RomanceRewardEarned)
                _state.ForgivenessAvailable = false;
            if (_state.PendingHuntingBonusPercent < 0)
                _state.PendingHuntingBonusPercent = 0;
        }

        public NightActivitySaveData Snapshot() { return _state.Copy(); }

        /// <param name="atNight">Use the host game's own time-of-day system.</param>
        /// <param name="nightNumber">Positive, monotonic counter within a run. Not wall-clock time.</param>
        /// <param name="minigamePassed">True only after the host validates the actual minigame.</param>
        public NightActivityResult TryComplete(
            NightActivityType activity, bool atNight, int nightNumber, bool minigamePassed)
        {
            if (!atNight || nightNumber < 1)
                return Fail("Only available at night during an active run.");

            if (!Enum.IsDefined(typeof(NightActivityType), activity))
                return Fail("Unknown activity.");

            // Check eligibility before the minigame result; a failed attempt never grants a reward.
            switch (activity)
            {
                case NightActivityType.Romance:
                    if (_state.RomanceRewardEarned)
                        return Fail("Wolf Mama's second chance can only be earned once per run.");
                    break;
                case NightActivityType.Cooking:
                    if (_state.LastCookingNight >= nightNumber)
                        return Fail("Cooking reward already claimed this night.");
                    break;
                case NightActivityType.HuntingPractice:
                    if (_state.LastHuntingNight >= nightNumber)
                        return Fail("Hunting practice reward already claimed this night.");
                    if (_state.PendingHuntingBonusPercent > 0)
                        return Fail("Previous hunting bonus must be used before earning another.");
                    break;
                case NightActivityType.PlayingWithPups:
                    if (_state.LastPupPlayNight >= nightNumber)
                        return Fail("Pup-play reward already claimed this night.");
                    break;
            }

            if (!minigamePassed)
                return Fail("Minigame not completed successfully; no reward granted.");

            var reward = new NightActivityReward();
            switch (activity)
            {
                case NightActivityType.Romance:
                    _state.RomanceRewardEarned = true;
                    _state.ForgivenessAvailable = true;
                    reward.ExtraLifeCharge = true;
                    return Ok("Wolf Mama promises one second chance this run.", reward);
                case NightActivityType.Cooking:
                    _state.LastCookingNight = nightNumber;
                    reward.FoodPortions = 2;  // Illustrative balance value, not an official stat.
                    return Ok("Cooked together: +2 example food portions.", reward);
                case NightActivityType.HuntingPractice:
                    _state.LastHuntingNight = nightNumber;
                    _state.PendingHuntingBonusPercent = 10; // Illustrative, not official.
                    reward.HuntingBonusPercent = 10;
                    return Ok("Practice complete: next-hunt bonus queued.", reward);
                case NightActivityType.PlayingWithPups:
                    _state.LastPupPlayNight = nightNumber;
                    reward.PupMorale = 5; // Illustrative, not official.
                    return Ok("Pups are happier: +5 example morale.", reward);
                default:
                    return Fail("Unknown activity.");
            }
        }

        /// <summary>
        /// Call ONLY when the actual game's Wolf Mama consequence would end the run.
        /// Returning true suppresses that single fatal consequence. The host owns any strike reset.
        /// Does not protect against other deaths or revoke earlier resource costs.
        /// Persist the consumed state before returning control to gameplay.
        /// </summary>
        public bool TryForgiveFatalWolfMamaFailure()
        {
            if (!_state.ForgivenessAvailable || _state.ForgivenessSpent)
                return false;

            _state.ForgivenessAvailable = false;
            _state.ForgivenessSpent = true;
            return true;
        }

        /// <summary>Consume a queued hunting bonus after the host reaches the next day/hunt.</summary>
        public int ConsumeHuntingBonusPercent()
        {
            int percent = _state.PendingHuntingBonusPercent;
            _state.PendingHuntingBonusPercent = 0;
            return percent;
        }

        private static NightActivityResult Fail(string message)
        {
            return new NightActivityResult(false, message);
        }

        private static NightActivityResult Ok(string message, NightActivityReward reward)
        {
            return new NightActivityResult(true, message, reward);
        }
    }
}
