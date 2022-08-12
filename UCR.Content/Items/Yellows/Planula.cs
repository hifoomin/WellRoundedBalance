using MonoMod.Cil;
using RoR2;

namespace UltimateCustomRun.Items.Yellows
{
    public class Planula : ItemBase
    {
        public static float FlatHealing;

        public override string Name => ":: Items :::: Yellows :: Planula";
        public override string InternalPickupToken => "parentEgg";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Heal from <style=cIsDamage>incoming damage</style> for <style=cIsHealing>" + FlatHealing + "<style=cStack> (+" + FlatHealing + " per stack)</style></style>.";

        public override void Init()
        {
            FlatHealing = ConfigOption(15, "Flat Healing", "Vanilla is 15");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += ChangeHealing;
        }

        public static void ChangeHealing(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(15f)))
            {
                c.Index += 1;
                c.Next.Operand = FlatHealing;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Planula Healing hook");
            }
        }
    }
}