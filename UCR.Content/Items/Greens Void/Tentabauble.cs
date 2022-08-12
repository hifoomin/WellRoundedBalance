using MonoMod.Cil;

namespace UltimateCustomRun.Items.VoidGreens
{
    public class Tentabauble : ItemBase
    {
        public static float Chance;

        public override string Name => ":: Items ::::::: Void Greens :: Tentabauble";
        public override string InternalPickupToken => "slowOnHitVoid";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "<style=cIsUtility>5%</style> <style=cStack>(+5% per stack)</style> chance on hit to <style=cIsDamage>root</style> enemies for <style=cIsUtility>1s</style> <style=cStack>(+1s per stack)</style>. <style=cIsVoid>Corrupts all Chronobaubles</style>.";

        public override void Init()
        {
            Chance = ConfigOption(5f, "Chance", "Per Stack. Vanilla is 5");
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
        }
    }
}