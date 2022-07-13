using MonoMod.Cil;

namespace UltimateCustomRun.Items.Reds
{
    public class RejuvenationRack : ItemBase
    {
        public static float Healing;
        public override string Name => ":: Items ::: Reds :: Rejuvenation Rack";
        public override string InternalPickupToken => "increaseHealing";
        public override bool NewPickup => true;
        public override string PickupText => "Majorly increase the strength of healing.";

        public override string DescText => "<style=cIsHealing>Heal +" + d(Healing) + "</style> <style=cStack>(+" + d(Healing) + " per stack)</style> more.";

        public override void Init()
        {
            Healing = ConfigOption(1f, "Healing Increase", "Decimal. Per Stack. Vanilla is 1");
            ROSOption("Greens", 0f, 10f, 0.25f, "3");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.Heal += ChangeHealing;
        }

        public static void ChangeHealing(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1f)
            );
            c.Next.Operand = Healing;
        }
    }
}