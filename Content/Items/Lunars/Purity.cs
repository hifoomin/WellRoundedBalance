using MonoMod.Cil;

namespace WellRoundedBalance.Items.Lunars
{
    public class Purity : ItemBase
    {
        public override string Name => ":: Items ::::: Lunars :: Purity";
        public override string InternalPickupToken => "lunarBadLuck";

        public override string PickupText => "Reduce your skill cooldowns by 3 seconds. <color=#FF7F7F>You are unlucky.</color>";
        public override string DescText => "All skill cooldowns are reduced by <style=cIsUtility>3</style> <style=cStack>(+1 per stack)</style> seconds. All random effects are rolled <style=cIsUtility>+1</style> <style=cStack>(+1 per stack)</style> times for an <style=cIsHealth>unfavorable outcome</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeCdr;
        }

        private void ChangeCdr(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdcR4(2f),
                    x => x.MatchLdcR4(1f)))
            {
                c.Index += 1;
                c.Next.Operand = 3f;
                c.Index += 1;
                c.Next.Operand = 1f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Purity Cooldown Reduction hook");
            }
        }
    }
}