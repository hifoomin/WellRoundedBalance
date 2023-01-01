using RoR2;
using System;
using UnityEngine;

namespace WellRoundedBalance.Global
{
    public class TimeScaling : GlobalBase
    {
        public override string Name => ": Global : Time Scaling";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            ChangeBehavior();
        }

        public static void ChangeBehavior()
        {
            On.RoR2.Run.RecalculateDifficultyCoefficentInternal += (orig, self) =>
            {
                int playerCount = self.participatingPlayerCount;
                float Time = self.GetRunStopwatch() * 0.016666668f; // stupid vanilla workaround

                var playerfactorbase = 0.7f;
                var playercountmultiplier = 0.3f;
                var playercountexponent = 0.2f;
                var exponentialstagescalingbase = 1f;
                var timefactormultiplier = 0.0506f;

                DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(self.selectedDifficulty);
                float playerFactor = playerfactorbase + playerCount * playercountmultiplier;
                float timeFactor = Time * timefactormultiplier * difficultyDef.scalingValue;
                float playerScalar = (float)Math.Pow(playerCount, playercountexponent);

                float stageFactor = 1f + 0.65f * timeFactor * playerScalar;

                // im not changing this chain. Cope
                float finalDifficulty = (playerFactor + timeFactor * playerScalar) * stageFactor;
                self.compensatedDifficultyCoefficient = finalDifficulty;
                self.difficultyCoefficient = finalDifficulty;
                self.ambientLevel = Mathf.Min(3f * (finalDifficulty - playerFactor) + 1f, Run.ambientLevelCap);

                Run.ambientLevelCap = int.MaxValue;
                int ambientLevelFloor = self.ambientLevelFloor;
                self.ambientLevelFloor = Mathf.FloorToInt(self.ambientLevel);
                if (ambientLevelFloor != self.ambientLevelFloor && ambientLevelFloor != 0 && self.ambientLevelFloor > ambientLevelFloor)
                {
                    self.OnAmbientLevelUp();
                }
            };
        }
    }
}