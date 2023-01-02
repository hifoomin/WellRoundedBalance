using MonoMod.Cil;
using RoR2;

namespace WellRoundedBalance.Items.Whites
{
    public class BustlingFungus : ItemBase
    {
        public override string Name => ":: Items : Whites :: Bustling Fungus";
        public override string InternalPickupToken => "mushroom";

        public override string PickupText => "Heal all nearby allies after standing still for 0.5 seconds.";

        public override string DescText => "After standing still for <style=cIsHealing>0.5</style> seconds, create a zone that <style=cIsHealing>heals</style> for <style=cIsHealing>5%</style> <style=cStack>(+2.5% per stack)</style> of your <style=cIsHealing>health</style> every second to all allies within <style=cIsHealing>13m</style>.";

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
                c.Next.Operand = 13f;
                c.Index += 2;
                c.Next.Operand = 0f;
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
                c.Next.Operand = 0.25f;
                c.Index += 4;
                c.Next.Operand = 0.05f;
                c.Index += 1;
                c.Next.Operand = 0.025f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Bustling Fungus Healing hook");
            }
        }
    }
}