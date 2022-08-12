using MonoMod.Cil;

namespace UltimateCustomRun.Items.Lunars
{
    public class BrittleCrown : ItemBase
    {
        public static float Chance;
        public static float Gold;

        public override string Name => ":: Items ::::: Lunars :: Brittle Crown";
        public override string InternalPickupToken => "goldOnHit";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "<style=cIsUtility>30% chance on hit</style> to gain <style=cIsUtility>2<style=cStack> (+2 per stack)</style> gold</style>. <style=cIsUtility>Scales over time.</style>\n\nOn taking damage, <style=cIsHealth>lose gold</style> equal to <style=cIsHealth>100%<style=cStack> (+100% per stack)</style></style> of the <style=cIsHealth>maximum health percentage you lost</style>.";

        public override void Init()
        {
            Chance = ConfigOption(30f, "Chance", "Vanilla is 30");
            Gold = ConfigOption(2f, "Gold", "Per Stack. Vanilla is 2.");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeChance;
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeGold;
        }

        private void ChangeGold(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(2f),
                    x => x.MatchMul(),
                    x => x.MatchCallOrCallvirt<RoR2.Run>("get_instance")))
            {
                c.Next.Operand = Gold;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Brittle Crown Gold hook");
            }
        }

        private void ChangeChance(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchBle(out _),
                    x => x.MatchLdcR4(30f)))
            {
                c.Index += 1;
                c.Next.Operand = Chance;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Brittle Crown Chance hook");
            }
        }
    }
}