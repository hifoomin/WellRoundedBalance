using RoR2;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class BustlingFungus : Based
    {
        public static float radius;
        public static float radiustack;
        public static float interval;
        public static float healing;
        public static float healingstack;
        public override string Name => ":: Items : Whites :: Bustling Fungus";
        public override string InternalPickupToken => "mushroom";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public static float actualRadius = radius + radiustack;
        public override string DescText => "After standing still for <style=cIsHealing>1</style> second, create a zone that <style=cIsHealing>heals</style> for <style=cIsHealing>" + d(healing) + "</style> <style=cStack>(+" + d(healingstack) + " per stack)</style> of your <style=cIsHealing>health</style> every second to all allies within <style=cIsHealing>" + actualRadius + "m</style> <style=cStack>(+" + radiustack + "m per stack)</style>.";

        public override void Init()
        {
            healing = ConfigOption(0.045f, "Base Healing Percent", "Decimal. Vanilla is 0.045");
            healingstack = ConfigOption(0.0225f, "Stack Healing Percent", "Decimal. Per Stack. Vanilla is 0.0225");
            interval = ConfigOption(0.25f, "Interval", "Decimal. Vanilla is 0.25");
            radius = ConfigOption(3f, "Base Radius", "Vanilla is 3");
            radiustack = ConfigOption(1.5f, "Stack Radius", "Per Stack. Vanilla is 1.5");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.MushroomItemBehavior.FixedUpdate += ChangeRadius;
            IL.RoR2.CharacterBody.MushroomItemBehavior.FixedUpdate += ChangeHealing;
        }
        public static void ChangeRadius(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<CharacterBody>("get_radius"),
                x => x.MatchLdcR4(1.5f),
                x => x.MatchAdd(),
                x => x.MatchLdcR4(1.5f)
            );
            c.Index += 1;
            c.Next.Operand = radius - radiustack;
            c.Index += 2;
            c.Next.Operand = radiustack;
        }

        public static void ChangeHealing(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.25f),
                x => x.MatchStfld<HealingWard>("interval"),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(0.045f),
                x => x.MatchLdcR4(0.0225f)
            );
            c.Next.Operand = interval;
            c.Index += 3;
            c.Next.Operand = healing;
            c.Index += 1;
            c.Next.Operand = healingstack;
        }
        // TODO: Add Lingering
    }
}
