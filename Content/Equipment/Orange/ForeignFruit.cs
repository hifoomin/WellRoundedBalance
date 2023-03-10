using MonoMod.Cil;

namespace WellRoundedBalance.Equipment
{
    public class ForeignFruit : EquipmentBase
    {
        public override string Name => ":: Equipment :: Foreign Fruit";

        public override EquipmentDef InternalPickup => RoR2Content.Equipment.Fruit;

        public override string PickupText => "Heal on use.";

        public override string DescText => "Instantly heal for <style=cIsHealing>" + d(healPercent) + " of your maximum health</style>.";

        [ConfigField("Cooldown", "", 30f)]
        public static float cooldown;

        [ConfigField("Heal Percent", "Decimal.", 0.5f)]
        public static float healPercent;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var Fruit = Utils.Paths.EquipmentDef.Fruit.Load<EquipmentDef>();
            Fruit.cooldown = cooldown;

            IL.RoR2.EquipmentSlot.FireFruit += EquipmentSlot_FireFruit;
        }

        private void EquipmentSlot_FireFruit(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.5f)))
            {
                c.Next.Operand = healPercent;
            }
            else
            {
                Logger.LogError("Failed to apply Foreign Fruit Healing hook");
            }
        }
    }
}