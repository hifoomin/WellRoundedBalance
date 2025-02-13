using System;
using MonoMod.Cil;

namespace WellRoundedBalance.Difficulties
{
    internal class Drizzle : DifficultyBase<Drizzle>
    {
        public override string Name => ":: Difficulties : Drizzle";
        public override DifficultyIndex InternalDiff => DifficultyIndex.Easy;

        public override string DescText => "For new players. Every move calls for less effort and attention.<style=cStack>\n\n" +
                                           (percentRegenIncrease > 0 ? ">Player Health Regeneration: <style=cIsHealing>+" + d(percentRegenIncrease) + "</style> \n" : "") +
                                           (armorGain > 0 ? ">Player Damage Reduction: <style=cIsHealing>+" + ((Math.Round(armorGain / (100 + armorGain), 2) * 100)) + "%</style> \n" : "") +
                                           ">Difficulty Scaling: <style=cIsHealing>" + (totalDifficultyScaling - 100f) + "%</style></style>";

        [ConfigField("Percent Regen Increase", "Decimal.", 0f)]
        public static float percentRegenIncrease;

        [ConfigField("Armor Gain", "", 0f)]
        public static float armorGain;

        [ConfigField("Total Difficulty Scaling", "", 75f)]
        public static float totalDifficultyScaling;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            Changes();
        }

        private void CharacterBody_RecalculateStats(ILContext il)
        {
            ILCursor c = new(il);

            c.FindLocal(LocalType.ItemCount, "DrizzlePlayerHelper", out int dph);
            c.StepLocal(dph);
            
            if (c.TryGotoNext(MoveType.After,
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(0.5f)))
            {
                c.Prev.Operand = percentRegenIncrease;
            }
            else
            {
                Logger.LogError("Failed to apply Drizzle Regen hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(70f)))
            {
                c.Next.Operand = armorGain;
            }
            else
            {
                Logger.LogError("Failed to apply Drizzle Armor hook");
            }
        }

        private void Changes()
        {
            DifficultyDef def = DifficultyCatalog.difficultyDefs.FirstOrDefault(x => DifficultyCatalog.GetDifficultyDef(InternalDiff) == x);
            if (def != null && def != default) def.scalingValue = totalDifficultyScaling / 50f;
        }
    }
}