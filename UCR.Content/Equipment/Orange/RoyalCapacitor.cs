using MonoMod.Cil;

namespace UltimateCustomRun.Equipment
{
    public class RoyalCapacitor : EquipmentBase
    {
        public override string Name => "::: Equipment :: Royal Capacitor";
        public override string InternalPickupToken => "lightning";

        public override bool NewPickup => false;

        public override bool NewDesc => true;

        public override string PickupText => "";

        public override string DescText => "Call down a lightning strike on a targeted monster, dealing <style=cIsDamage>" + d(Damage) + " damage</style> and <style=cIsDamage>stunning</style> nearby monsters.";

        public static float Damage;
        public static float ProcCoefficient;

        public override void Init()
        {
            Damage = ConfigOption(30f, "Damage", "Decimal. Vanilla is 30");
            ProcCoefficient = ConfigOption(1f, "Proc Coefficient", "Vanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.EquipmentSlot.FireLightning += Changes;
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(30f)))
            {
                c.Next.Operand = Damage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Royal Capacitor Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(1f)))
            {
                c.Next.Operand = ProcCoefficient;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Royal Capacitor Proc Coefficient hook");
            }
        }
    }
}