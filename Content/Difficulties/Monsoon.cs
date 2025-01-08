using MonoMod.Cil;

namespace WellRoundedBalance.Difficulties
{
    internal class Monsoon : DifficultyBase<Monsoon>
    {
        public override string Name => ":: Difficulties ::: Monsoon";
        public override DifficultyIndex InternalDiff => DifficultyIndex.Hard;

        public override string DescText => "For hardcore players. Every bend introduces pain and horrors of the planet. You will die.<style=cStack>\n\n" +
                                           (percentRegenDecrease > 0 ? ">Player Health Regeneration: <style=cIsHealth>-" + d(percentRegenDecrease) + "</style> \n" : "") +
                                           ">Difficulty Scaling: <style=cIsHealth>+" + (totalDifficultyScaling - 100f) + "%</style></style>";

        [ConfigField("Percent Regen Decrease", "Decimal.", 0.25f)]
        public static float percentRegenDecrease;

        [ConfigField("Total Difficulty Scaling", "", 133f)]
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

            c.FindLocal(LocalType.ItemCount, "MonsoonPlayerHelper", out int dph);
            c.StepLocal(dph);
            
            if (c.TryGotoNext(MoveType.After,
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(0.4f)))
            {
                c.Prev.Operand = percentRegenDecrease;
            }
            else
            {
                Logger.LogError("Failed to apply Monsoon Regen hook");
            }
        }

        private void Changes()
        {
            DifficultyDef def = DifficultyCatalog.difficultyDefs.FirstOrDefault(x => DifficultyCatalog.GetDifficultyDef(InternalDiff) == x);
            if (def != null && def != default) def.scalingValue = totalDifficultyScaling / 50f;
        }
    }
}