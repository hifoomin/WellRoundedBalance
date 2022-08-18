using MonoMod.Cil;

namespace UltimateCustomRun.Items.VoidGreens
{
    public class Tentabauble : ItemBase
    {
        public static float Chance;
        public static float DebuffLength;

        public override string Name => ":: Items :::::: Voids :: Tentabauble";
        public override string InternalPickupToken => "slowOnHitVoid";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "<style=cIsUtility>" + Chance + "%</style> <style=cStack>(+" + Chance + "% per stack)</style> chance on hit to <style=cIsDamage>root</style> enemies for <style=cIsUtility>" + DebuffLength + "s</style> <style=cStack>(+" + DebuffLength + "s per stack)</style>. <style=cIsVoid>Corrupts all Chronobaubles</style>.";

        public override void Init()
        {
            Chance = ConfigOption(5f, "Chance", "Per Stack. Vanilla is 5");
            DebuffLength = ConfigOption(1f, "Debuff Length", "Per Stack. Vanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeChance;
        }

        private void ChangeChance(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchBrfalse(out _),
                    x => x.MatchLdcR4(5f)))
            {
                c.Index += 1;
                c.Next.Operand = Chance;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Tentabauble Chance hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.RoR2Content/Buffs", "Nullified"),
                x => x.MatchLdcR4(1f)))
            {
                c.Index += 1;
                c.Next.Operand = Chance;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Tentabauble Debuff Length hook");
            }
        }
    }
}