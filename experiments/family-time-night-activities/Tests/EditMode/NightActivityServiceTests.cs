using NUnit.Framework;
using UnityEngine;
using Buni.FanPrototype.FamilyTimeNightActivities;

namespace Buni.FanPrototype.FamilyTimeNightActivities.Tests
{
    public class NightActivityServiceTests
    {
        [Test]
        public void RomanceGrantsExactlyOneForgivenessPerRun()
        {
            var service = new NightActivityService();
            var first = service.TryComplete(NightActivityType.Romance, true, 1, true);
            Assert.IsTrue(first.Success);
            Assert.IsTrue(first.Reward.ExtraLifeCharge);
            Assert.IsFalse(service.TryComplete(NightActivityType.Romance, true, 2, true).Success);
            Assert.IsTrue(service.TryForgiveFatalWolfMamaFailure());
            Assert.IsFalse(service.TryForgiveFatalWolfMamaFailure());
            Assert.IsFalse(service.TryComplete(NightActivityType.Romance, true, 3, true).Success);
        }

        [Test]
        public void FailureOrDaytimeDoesNotGiveRewardOrLockOutRomance()
        {
            var service = new NightActivityService();
            Assert.IsFalse(service.TryComplete(NightActivityType.Romance, false, 1, true).Success);
            Assert.IsFalse(service.TryComplete(NightActivityType.Romance, true, 1, false).Success);
            Assert.IsFalse(service.TryForgiveFatalWolfMamaFailure());
            Assert.IsTrue(service.TryComplete(NightActivityType.Romance, true, 1, true).Success);
        }

        [Test]
        public void SaveRoundTripPreservesChargeAndUsedState()
        {
            var original = new NightActivityService();
            original.TryComplete(NightActivityType.Romance, true, 1, true);
            string json = JsonUtility.ToJson(original.Snapshot());
            var loaded = new NightActivityService(JsonUtility.FromJson<NightActivitySaveData>(json));
            Assert.IsTrue(loaded.TryForgiveFatalWolfMamaFailure());
            string usedJson = JsonUtility.ToJson(loaded.Snapshot());
            var afterReload = new NightActivityService(JsonUtility.FromJson<NightActivitySaveData>(usedJson));
            Assert.IsFalse(afterReload.TryForgiveFatalWolfMamaFailure());
            Assert.IsFalse(afterReload.TryComplete(NightActivityType.Romance, true, 5, true).Success);
        }

        [Test]
        public void ExtraLifeDoesNotMagicallyExistOnNewRun()
        {
            var original = new NightActivityService();
            original.TryComplete(NightActivityType.Romance, true, 1, true);
            var newRun = new NightActivityService();
            Assert.AreNotEqual(original.Snapshot().RunId, newRun.Snapshot().RunId);
            Assert.IsFalse(newRun.TryForgiveFatalWolfMamaFailure());
        }

        [Test]
        public void RepeatingCookingWithinSameNightDoesNotFarmFood()
        {
            var service = new NightActivityService();
            Assert.AreEqual(2, service.TryComplete(NightActivityType.Cooking, true, 1, true).Reward.FoodPortions);
            Assert.IsFalse(service.TryComplete(NightActivityType.Cooking, true, 1, true).Success);
            Assert.IsTrue(service.TryComplete(NightActivityType.Cooking, true, 2, true).Success);
        }

        [Test]
        public void HuntingBonusMustBeConsumedBeforeAnotherIsGranted()
        {
            var service = new NightActivityService();
            Assert.IsTrue(service.TryComplete(NightActivityType.HuntingPractice, true, 1, true).Success);
            Assert.IsFalse(service.TryComplete(NightActivityType.HuntingPractice, true, 2, true).Success);
            Assert.AreEqual(10, service.ConsumeHuntingBonusPercent());
            Assert.AreEqual(0, service.ConsumeHuntingBonusPercent());
            Assert.IsTrue(service.TryComplete(NightActivityType.HuntingPractice, true, 2, true).Success);
        }

        [Test]
        public void InconsistentSaveCannotRestoreSpentCharge()
        {
            var service = new NightActivityService(new NightActivitySaveData {
                RomanceRewardEarned = true, ForgivenessAvailable = true, ForgivenessSpent = true
            });
            Assert.IsFalse(service.TryForgiveFatalWolfMamaFailure());
        }
    }
}
