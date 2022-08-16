using MonoMod.Cil;

namespace UltimateCustomRun.Items.Lunars
{
    public class Egocentrism : ItemBase
    {
        public static float Time;
        public static float Damage;

        public override string Name => ":: Items ::::: Lunars :: Egocentrism";
        public override string InternalPickupToken => "lunarSun";
        public override bool NewPickup => true;
        public override string PickupText => "Gain multiple orbiting bombs. <color=#FF7F7F>Every " + Time + " seconds, assimilate another item into Egocentrism.</color>";
        public override string DescText => "Every <style=cIsUtility>3</style><style=cStack>(-50% per stack)</style> seconds, gain an <style=cIsDamage>orbiting bomb</style> that detonates on impact for <style=cIsDamage>" + d(Damage) + "</style> damage, up to a maximum of <style=cIsUtility>3<style=cStack>(+1 per stack)</style> bombs</style>. Every <style=cIsUtility>" + Time + "</style> seconds, a random item is <style=cIsUtility>converted</style> into this item.";

        public override void Init()
        {
            Time = ConfigOption(60f, "Seconds to convert", "Vanilla is 60");
            Damage = ConfigOption(3.6f, "Damage", "Decimal. Vanilla is 3.6");
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
                    x => x.MatchLdcR4(60f)))
            {
                c.Next.Operand = Time;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Egocentrism Time hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(3.6f)))
            {
                c.Next.Operand = Damage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Egocentrism Damage hook");
            }
        }
    }
}