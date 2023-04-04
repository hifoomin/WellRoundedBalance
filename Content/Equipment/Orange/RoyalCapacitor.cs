using MonoMod.Cil;
using WellRoundedBalance.Items;

namespace WellRoundedBalance.Equipment.Orange
{
    public class RoyalCapacitor : EquipmentBase<RoyalCapacitor>
    {
        public override string Name => ":: Equipment :: Royal Capacitor";
        public override EquipmentDef InternalPickup => RoR2Content.Equipment.Lightning;

        public override string PickupText => "Call down a lightning strike on a targeted monster.";

        public override string DescText => "Call down a lightning strike on a targeted monster, dealing <style=cIsDamage>" + d(damage) + " damage</style> and <style=cIsDamage>stunning</style> nearby monsters.";

        [ConfigField("Cooldown", "", 25f)]
        public static float cooldown;

        [ConfigField("Damage", "Decimal.", 20f)]
        public static float damage;

        [ConfigField("Proc Coefficient", "", 1f)]
        public static float procCoefficient;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.EquipmentSlot.FireLightning += Changes;

            var cap = Utils.Paths.EquipmentDef.Lightning.Load<EquipmentDef>();
            cap.cooldown = cooldown;
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(30f)))
            {
                c.Next.Operand = damage;
            }
            else
            {
                Logger.LogError("Failed to apply Royal Capacitor Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(1f)))
            {
                c.Next.Operand = procCoefficient;
            }
            else
            {
                Logger.LogError("Failed to apply Royal Capacitor Proc Coefficient hook");
            }
        }
    }
}