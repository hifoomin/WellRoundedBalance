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
            Damage = ConfigOption(0.75f, "Damage Coefficient", "Decimal. Per Stack. Vanilla is 0.75");
            ROSOption("Whites", 0f, 1f, 0.05f, "1");
            Threshold = ConfigOption(0.9f, "Threshold", "Decimal. Vanilla is 0.9");
            ROSOption("Whites", 0f, 1f, 0.01f, "1");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += ChangeDamage;
            IL.RoR2.HealthComponent.TakeDamage += ChangeThreshold;
        }

        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1f),
                x => x.MatchLdcR4(0.75f)
            );
            c.Index += 1;
            c.Next.Operand = Damage;
        }

        public static void ChangeThreshold(ILContext il)
        {
            ILCursor c = new(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdarg(0),
                x => x.MatchCallOrCallvirt<HealthComponent>("get_fullCombinedHealth"),
                x => x.MatchLdcR4(0.9f)
            );
            c.Index += 2;
            c.Next.Operand = Threshold;
        }
    }
}