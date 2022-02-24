using RoR2;
using System;
using UnityEngine;

namespace UltimateCustomRun.Global
{
    public class TimeScaling : GlobalBase
    {
        public static float playerfactorbase;
        public static float playercountmultiplier;
        public static float playercountexponent;
        public static bool additivestagescaling;
        public static float additivestagescalingbase;
        public static bool exponentialstagescaling;
        public static float exponentialstagescalingbase;
        public static float exponentialstagescalingcount;
        public static float timefactormultiplier;
        public static int ambientlevelcap;
        public static bool tsguide;
        public override string Name => ": Global : Time Scaling";

        public override void Init()
        {
            playerfactorbase = ConfigOption(0.7f, "Player Factor Base", "Vanilla is 0.7");
            playercountmultiplier = ConfigOption(0.3f, "Player Count Multiplier", "Vanilla is 0.3");
            playercountexponent = ConfigOption(0.2f, "Player Count Exponent", "Vanilla is 0.2");
            additivestagescaling = ConfigOption(false, "Make Stage Scaling Additive?", "Vanilla is false");
            additivestagescalingbase = ConfigOption(0f, "How Much Time to add", "Only works with Additive Stage Scaling enabled. Vanilla is 0");
            exponentialstagescaling = ConfigOption(true, "Make Stage Scaling Exponential?", "Vanilla is true");
            exponentialstagescalingbase = ConfigOption(1.15f, "How Much Time to exponentially raise", "Only works with Additive Stage Scaling enabled. Vanilla is 1.15");
            exponentialstagescalingcount = ConfigOption(1f, "Every Nth Stage scales the Difficulty exponentially", "Only works with Additive Stage Scaling enabled. Vanilla is 1");
            timefactormultiplier = ConfigOption(0.0506f, "Time Factor Multiplier", "Vanilla is 0.0506");
            ambientlevelcap = ConfigOption(99, "Ambient Level Cap", "Vanilla is 99");
            tsguide = ConfigOption(true, "Time Scaling Guide", "Time Scaling Formula:\n(Player Factor Base + Player Count * Player Count Multiplier +\nTime Scaling Multiplier * Difficulty Def Scaling Value\n(1 for Drizzle, 2 for Rainstorm, 3 for Monsoon) *\nPlayer Count ^ Player Count Exponent *\nTime in Minutes) * Exponential Stage Scaling Base ^ Stages Cleared\nI highly recommend changing Gold Scaling while changing these as well");
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

                DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(self.selectedDifficulty);
                float playerFactor = playerfactorbase + playerCount * playercountmultiplier;
                float timeFactor = Time * timefactormultiplier * difficultyDef.scalingValue;
                float playerScalar = (float)Math.Pow(playerCount, playercountexponent);
                float stageFactor = 1f;

                if (additivestagescaling == false && exponentialstagescaling)
                {
                    stageFactor = Mathf.Pow(exponentialstagescalingbase, self.stageClearCount / exponentialstagescalingcount);
                }
                else if (additivestagescaling && exponentialstagescaling == false)
                {
                    stageFactor = 1f;
                    Time += additivestagescalingbase;
                }
                else if (exponentialstagescaling && additivestagescaling)
                {
                    stageFactor = Mathf.Pow(exponentialstagescalingbase, self.stageClearCount / exponentialstagescalingcount);
                    Time += additivestagescalingbase;
                }
                else
                {
                    stageFactor = 1f;
                }

                // im not changing this chain. Cope
                float finalDifficulty = (playerFactor + timeFactor * playerScalar) * stageFactor;
                self.compensatedDifficultyCoefficient = finalDifficulty;
                self.difficultyCoefficient = finalDifficulty;
                self.ambientLevel = Mathf.Min(3f * (finalDifficulty - playerFactor) + 1f, Run.ambientLevelCap);

                Run.ambientLevelCap = ambientlevelcap;
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