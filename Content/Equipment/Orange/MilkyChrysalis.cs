namespace WellRoundedBalance.Equipment.Orange
{
    public class MilkyChrysalis : EquipmentBase<MilkyChrysalis>
    {
        public override string Name => ":: Equipment :: Milky Chrysalis";
        public override EquipmentDef InternalPickup => RoR2Content.Equipment.Jetpack;

        public override string PickupText => "Gain temporary flight.";

        public override string DescText => "Sprout wings and <style=cIsUtility>fly for 15 seconds</style>. Gain <style=cIsUtility>+20% movement speed</style> for the duration.";

        [ConfigField("Cooldown", "", 45f)]
        public static float cooldown;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var Milky = Utils.Paths.EquipmentDef.Jetpack.Load<EquipmentDef>();
            Milky.cooldown = cooldown;
        }
    }
}