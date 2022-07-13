using MonoMod.Cil;
using RoR2;

namespace UltimateCustomRun.Items.Whites
{
    public class DelicateWatch : ItemBase
    {
        public static float Damage;

        public override string Name => ":: Items : Whites :: Delicate Watch";
        public override string InternalPickupToken => "fragileDamageBonus";
        public override bool NewPickup => false;

        public override string PickupText => "";
        public override string DescText => "Increase damage by <style=cIsDamage>" + d(Damage) + "</style> <style=cStack>(+" + d(Damage) + " per stack)</style>. Taking damage to below <style=cIsHealth>25% health</style> <style=cIsUtility>breaks</style> this item.";

        public override void Init()
        {
            Damage = ConfigOption(0.2f, "Damage Coefficient", "Decimal. Per Stack. Vanilla is 0.2");
            ROSOption("Whites", 0f, 1f, 0.01f, "1");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += ChangeDamage;
        }

        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchBle(out _),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(1),
                x => x.MatchLdloc(out _),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.2f)
            );
            c.Index += 5;
            c.Next.Operand = Damage;
        }
    }
}