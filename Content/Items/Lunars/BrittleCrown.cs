using MonoMod.Cil;

namespace WellRoundedBalance.Items.Lunars
{
    public class BrittleCrown : ItemBase
    {
        public override string Name => ":: Items ::::: Lunars :: Brittle Crown";
        public override string InternalPickupToken => "goldOnHit";

        public override string PickupText => "Gain gold on hit... <color=#FF7F7F>BUT surrender gold on getting hit</color>.";
        public override string DescText => "<style=cIsUtility>30% chance on hit</style> to gain <style=cIsUtility>1.5<style=cStack> (+1.5 per stack)</style> gold</style>. <style=cIsUtility>Scales over time.</style>\n\nOn taking damage, <style=cIsHealth>lose gold</style> equal to <style=cIsHealth>100%<style=cStack> (+100% per stack)</style></style> of the <style=cIsHealth>maximum health percentage you lost</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += Changes;
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(2f),
                    x => x.MatchMul(),
                    x => x.MatchCallOrCallvirt<Run>("get_instance")))
            {
                c.Next.Operand = 1.5f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Brittle Crown Gold hook");
            }
        }
    }
}