using MonoMod.Cil;

namespace UltimateCustomRun.Equipment
{
    public class ForeignFruit : EquipmentBase
    {
        public override string Name => "::: Equipment :: Foreign Fruit";
        public override string InternalPickupToken => "fruit";

        public override bool NewPickup => false;

        public override bool NewDesc => true;

        public override string PickupText => "";

        public override string DescText => "Instantly heal for <style=cIsHealing>" + d(Healing) + " of your maximum health</style>.";

        public static float Healing;

        public override void Init()
        {
            Healing = ConfigOption(0.5f, "Percent Healing", "Decimal. Vanilla is 0.5");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.EquipmentSlot.FireFruit += ChangeHealing;
        }

        private void ChangeHealing(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.5f)))
            {
                c.Next.Operand = Healing;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Foreign Fruit Healing hook");
            }
        }
    }
}