using RoR2;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class Crowbar : ItemBase
    {
        public static float damage;
        public static float threshold;

        public override string Name => ":: Items : Whites :: Crowbar";
        public override string InternalPickupToken => "crowbar";
        public override bool NewPickup => true;

        public override string PickupText => "Deal bonus damage to enemies above " + d(threshold) + " health.";
        public override string DescText => "Deal <style=cIsDamage>+" + d(damage) + "</style> <style=cStack>(+" + d(damage) + " per stack)</style> damage to enemies above <style=cIsDamage>" + d(threshold) + " health</style>.";
        public override void Init()
        {
            damage = ConfigOption(0.75f, "Damage Coefficient", "Decimal. Per Stack. Vanilla is 0.75");
            threshold = ConfigOption(0.9f, "Threshold", "Decimal. Vanilla is 0.9");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += ChangeDamage;
            IL.RoR2.HealthComponent.TakeDamage += ChangeThreshold;
        }
        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1f),
                x => x.MatchLdcR4(0.75f)
            );
            c.Index += 1;
            c.Next.Operand = damage;
        }
        public static void ChangeThreshold(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdarg(0),
                x => x.MatchCallOrCallvirt<HealthComponent>("get_fullCombinedHealth"),
                x => x.MatchLdcR4(0.9f)
            );
            c.Index += 2;
            c.Next.Operand = threshold;
        }
    }
}
