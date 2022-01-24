using RoR2;
using UnityEngine;
using System;

namespace UltimateCustomRun
{
    public static class Scaling
    {
        public static void ChangeBehavior()
        {
            On.RoR2.Run.RecalculateDifficultyCoefficentInternal += (orig, self) =>
            {
                int playerCount = self.participatingPlayerCount;
                float time = self.GetRunStopwatch() * 0.016666668f; // stupid vanilla workaround

                DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(self.selectedDifficulty);
                float playerFactor = Main.PlayerFactorBase.Value + playerCount * Main.PlayerCountMultiplier.Value;
                float timeFactor = time * Main.TimeFactorMultiplier.Value * difficultyDef.scalingValue;
                float playerScalar = (float)Math.Pow(playerCount, Main.PlayerCountExponent.Value);
                float stageFactor = 1f;
                if (Main.AdditiveStageScaling.Value == false && Main.ExponentialStageScaling.Value)
                {
                    stageFactor = Mathf.Pow(Main.ExponentialStageScalingBase.Value, self.stageClearCount / Main.ExponentialStageScalingCount.Value);
                }
                else if (Main.AdditiveStageScaling.Value && Main.ExponentialStageScaling.Value == false)
                {
                    stageFactor = 1f;
                    time += Main.AdditiveStageScalingBase.Value;
                }
                else if (Main.ExponentialStageScaling.Value && Main.AdditiveStageScaling.Value)
                {
                    stageFactor = Mathf.Pow(Main.ExponentialStageScalingBase.Value, self.stageClearCount / Main.ExponentialStageScalingCount.Value);
                    time += Main.AdditiveStageScalingBase.Value;
                }
                else
                {
                    stageFactor = 1f;
                }
                float finalDifficulty = (playerFactor + timeFactor * playerScalar) * stageFactor;
                self.compensatedDifficultyCoefficient = finalDifficulty;
                self.difficultyCoefficient = finalDifficulty;
                self.ambientLevel = Mathf.Min(3f * (finalDifficulty - playerFactor) + 1f, Run.ambientLevelCap);

                Run.ambientLevelCap = Main.AmbientLevelCap.Value;
                int ambientLevelFloor = self.ambientLevelFloor;
                self.ambientLevelFloor = Mathf.FloorToInt(self.ambientLevel);
                if (ambientLevelFloor != self.ambientLevelFloor && ambientLevelFloor != 0 && self.ambientLevelFloor > ambientLevelFloor)
                {
                    self.OnAmbientLevelUp();
                }

            };
            On.RoR2.DeathRewards.OnKilledServer += (orig, self, damageReport) =>
            {
                self.goldReward = (uint)Mathf.CeilToInt(Mathf.Pow(self.goldReward, Main.GoldRewardExponent.Value) / Mathf.Pow(Main.GoldRewardDivisorBase.Value, Run.instance.stageClearCount / Main.GoldRewardStageClearCountDivisor.Value));
                orig(self, damageReport);
            };
            // still unfinished
            // why does adding time not work here
        }
    }
}
