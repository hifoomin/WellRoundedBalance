using MonoMod.Cil;
using RoR2;

namespace UltimateCustomRun.Items.Whites
{
    public class BustlingFungus : ItemBase
    {
        public static float Radius;
        public static float StackRadius;
        public static float Interval;
        public static float Healing;
        public static float StackHealing;
        public override string Name => ":: Items : Whites :: Bustling Fungus";
        public override string InternalPickupToken => "mushroom";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "After standing still for <style=cIsHealing>" +
                                            (Global.GetNotMoving.NotMovingTimer != 1f ? "" + Global.GetNotMoving.NotMovingTimer + "</style> seconds" : "1</style> second") +
                                            ", create a zone that <style=cIsHealing>heals</style> for <style=cIsHealing>" + d(Healing) + "</style> <style=cStack>(+" + d(StackHealing) + " per stack)</style> of your <style=cIsHealing>health</style> every second to all allies within <style=cIsHealing>" + Radius + "m</style> <style=cStack>(+" + StackRadius + "m per stack)</style>.";

        public override void Init()
        {
            Healing = ConfigOption(0.045f, "Base Healing Percent", "Decimal. Vanilla is 0.045");
            StackHealing = ConfigOption(0.0225f, "Stack Healing Percent", "Decimal. Per Stack. Vanilla is 0.0225");
            Interval = ConfigOption(0.25f, "Interval", "Decimal. Vanilla is 0.25");
            Radius = ConfigOption(3f, "Base Radius", "Vanilla is 3");
            StackRadius = ConfigOption(1.5f, "Stack Radius", "Per Stack. Vanilla is 1.5");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.Items.MushroomBodyBehavior.FixedUpdate += ChangeRadius;
            IL.RoR2.Items.MushroomBodyBehavior.FixedUpdate += ChangeHealing;
        }

        public static void ChangeRadius(ILContext il)
        {
            ILCursor c = new(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<CharacterBody>("get_radius"),
                x => x.MatchLdcR4(1.5f),
                x => x.MatchAdd(),
                x => x.MatchLdcR4(1.5f)
            );
            c.Index += 1;
            c.Next.Operand = Radius - StackRadius;
            c.Index += 2;
            c.Next.Operand = StackRadius;
        }

        public static void ChangeHealing(ILContext il)
        {
            ILCursor c = new(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.25f),
                x => x.MatchStfld<HealingWard>("interval"),
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<RoR2.Items.MushroomBodyBehavior>("mushroomHealingWard"),
                x => x.MatchLdcR4(0.045f),
                x => x.MatchLdcR4(0.0225f)
            );
            c.Next.Operand = Interval;
            c.Index += 4;
            c.Next.Operand = Healing;
            c.Index += 1;
            c.Next.Operand = StackHealing;
        }
    }
}