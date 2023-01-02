using MonoMod.Cil;

namespace WellRoundedBalance.Items.Lunars
{
    public class Egocentrism : ItemBase
    {
        public static float Damage;

        public override string Name => ":: Items ::::: Lunars :: Egocentrism";
        public override string InternalPickupToken => "lunarSun";

        public override string PickupText => "Gain multiple orbiting bombs. <color=#FF7F7F>Every 60 seconds, assimilate another item into Egocentrism.</color>";
        public override string DescText => "Every <style=cIsUtility>3</style><style=cStack>(-50% per stack)</style> seconds, gain an <style=cIsDamage>orbiting bomb</style> that detonates on impact for <style=cIsDamage>310%</style> damage, up to a maximum of <style=cIsUtility>3<style=cStack>(+1 per stack)</style> bombs</style>. Every <style=cIsUtility>60</style> seconds, a random item is <style=cIsUtility>converted</style> into this item.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.LunarSunBehavior.FixedUpdate += Changes;
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(3.6f)))
            {
                c.Next.Operand = 3.1f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Egocentrism Damage hook");
            }
        }
    }
}