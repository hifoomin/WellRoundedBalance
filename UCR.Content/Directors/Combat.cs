using RoR2;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System.Reflection;
using static RoR2.CombatDirector;
using System.Collections.Generic;
using System.Linq;
using System;
using R2API;
using static R2API.DirectorAPI;

namespace UltimateCustomRun.Elites
{
    public class Combat : DirectorBase
    {
        public static float Tier1Cost;
        public static float Tier1HonorCost;
        public static float Tier2Cost;
        public override string Name => "Directors : Combat";

        public override void Init()
        {
            Tier1Cost = ConfigOption(6f, "Tier 1 Elite Cost Multiplier", "Vanilla is 6");
            Tier1HonorCost = ConfigOption(3.5f, "Tier 1 Honor Elite Cost Multiplier", "Vanilla is 3.5");
            Tier2Cost = ConfigOption(36f, "Tier 2 Elite Cost Multiplier", "Vanilla is 36");
            base.Init();
        }

        public override void Hooks()
        {
            // Changes();
            StageSettingsActions += Changess;
        }

        private void Changess(StageSettings settings, StageInfo info)
        {
            for
            Main.UCRLogger.LogError("StageSettingsAction Event Ran");
        }
    }
}