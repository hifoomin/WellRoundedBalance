using R2API.Utils;
using RoR2;
using System;
using UnityEngine;

namespace WellRoundedBalance.Global
{
    public class TimeScaling : GlobalBase
    {
        public static float timer;
        public static float interval = 120f;
        public static float vanillaStandardScaling;
        public static float vanillaLinearScaling;

        public override string Name => ": Global : Time Scaling";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            ChangeBehavior();
            RoR2Application.onFixedUpdate += RoR2Application_onFixedUpdate;
        }

        private void RoR2Application_onFixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer >= interval && Run.instance)
            {
                ChatMessage.Send("Current difficulty coefficient is " + Run.instance.compensatedDifficultyCoefficient);
                ChatMessage.Send("Vanilla standard difficulty coefficient would be " + vanillaStandardScaling);
                ChatMessage.Send("Vanilla linear difficulty coefficient would be " + vanillaLinearScaling);
                timer = 0f;
            }
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

                float customTimeFactor = Mathf.Sqrt(Time) * 0.25f * difficultyDef.scalingValue;

                float customFactor = 1f + 0.35f * customTimeFactor * playerScalar;
                // previously * timeFactor instead of customTimeFactor

                // im not changing this chain. Cope
                float finalDifficulty = (playerFactor + timeFactor * playerScalar) * customFactor;
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

                float stageFactor = Mathf.Pow(1.15f, self.stageClearCount);

                vanillaStandardScaling = (playerFactor + timeFactor * playerScalar) * stageFactor;

                vanillaLinearScaling = playerFactor + timeFactor * playerScalar;
            };
        }
    }
}