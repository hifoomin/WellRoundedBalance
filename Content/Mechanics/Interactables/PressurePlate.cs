namespace WellRoundedBalance.Mechanics.Interactables
{
    internal class PressurePlate : MechanicBase<PressurePlate>
    {
        public override string Name => ":: Mechanics :::::: Abandoned Aqueduct Pressure Plate";

        [ConfigField("Attack Speed Gain per Mountain Shrine", "", 0.1f)]
        public static float attackSpeedGainPerMountainShrine;

        [ConfigField("Movement Speed Gain per Mountain Shrine", "", 0.1f)]
        public static float movementSpeedGainPerMountainShrine;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.PressurePlateController.SetSwitch += PressurePlateController_SetSwitch;
        }

        private void PressurePlateController_SetSwitch(On.RoR2.PressurePlateController.orig_SetSwitch orig, PressurePlateController self, bool switchIsDown)
        {
            if (switchIsDown == true)
                orig(self, switchIsDown);
        }
    }
}