using MonoMod.Cil;

namespace WellRoundedBalance.Items.Whites
{
    public class BustlingFungus : ItemBase
    {
        public override string Name => ":: Items : Whites :: Bustling Fungus";
        public override string InternalPickupToken => "mushroom";

        public override string PickupText => "Heal all nearby allies after standing still for " + standingStillDuration + " second" +
                                             (standingStillDuration != 1 ? "s." : ".");

        public override string DescText => "After standing still for <style=cIsHealing>" + standingStillDuration + "</style> second" +
                                           (standingStillDuration != 1 ? "s," : ",") +
                                           " create a zone that <style=cIsHealing>heals</style> for <style=cIsHealing>" + d(baseHealing) + "</style> <style=cStack>(+" + d(healingPerStack) + " per stack)</style> of your <style=cIsHealing>health</style> every second to all allies within <style=cIsHealing>" + baseRadius + "m</style>" +
                                           (radiusPerStack > 0 ? " <style=cStack>(+" + radiusPerStack + "m per stack)</style>." : ".");

        [ConfigField("Base Radius", "", 13f)]
        public static float baseRadius;

        [ConfigField("Radius Per Stack", "", 0f)]
        public static float radiusPerStack;

        [ConfigField("Base Healing", "Decimal.", 0.05f)]
        public static float baseHealing;

        [ConfigField("Healing Per Stack", "Decimal.", 0f)]
        public static float healingPerStack;

        [ConfigField("Standing Still Duration", "", 0.5f)]
        public static float standingStillDuration;

        [ConfigField("Healing Interval", "", 0.25f)]
        public static float healingInterval;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.Items.MushroomBodyBehavior.FixedUpdate += Changes;
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchCallOrCallvirt<CharacterBody>("get_radius"),
                    x => x.MatchLdcR4(1.5f),
                    x => x.MatchAdd(),
                    x => x.MatchLdcR4(1.5f)))
            {
                c.Index += 1;
                c.Next.Operand = baseRadius;
                c.Index += 2;
                c.Next.Operand = radiusPerStack;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Bustling Fungus Radius hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.25f),
                    x => x.MatchStfld<HealingWard>("interval"),
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<RoR2.Items.MushroomBodyBehavior>("mushroomHealingWard"),
                    x => x.MatchLdcR4(0.045f),
                    x => x.MatchLdcR4(0.0225f)))
            {
                c.Next.Operand = healingInterval;
                c.Index += 4;
                c.Next.Operand = baseHealing;
                c.Index += 1;
                c.Next.Operand = healingPerStack;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Bustling Fungus Healing hook");
            }
        }
    }
}