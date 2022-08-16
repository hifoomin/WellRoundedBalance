using MonoMod.Cil;
using RoR2;

namespace UltimateCustomRun.Items.Whites
{
    public class Crowbar : ItemBase
    {
        public static float Damage;
        public static float Threshold;

        public override string Name => ":: Items : Whites :: Crowbar";
        public override string InternalPickupToken => "crowbar";
        public override bool NewPickup => true;

        public override string PickupText => "Deal bonus damage to enemies above " + d(Threshold) + " health.";
        public override string DescText => "Deal <style=cIsDamage>+" + d(Damage) + "</style> <style=cStack>(+" + d(Damage) + " per stack)</style> damage to enemies above <style=cIsDamage>" + d(Threshold) + " health</style>.";

        public override void Init()
        {
            Damage = ConfigOption(0.75f, "Damage", "Decimal. Per Stack. Vanilla is 0.75");
            Threshold = ConfigOption(0.9f, "Health Threshold", "Decimal. Vanilla is 0.9");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += Changes;
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(1f),
                x => x.MatchLdcR4(0.75f)))
            {
                c.Index += 1;
                c.Next.Operand = Damage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Crowbar Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
               x => x.MatchLdarg(0),
               x => x.MatchCallOrCallvirt<HealthComponent>("get_fullCombinedHealth"),
               x => x.MatchLdcR4(0.9f)))
            {
                c.Index += 2;
                c.Next.Operand = Threshold;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Crowbar Threshold hook");
            }
        }
    }
}