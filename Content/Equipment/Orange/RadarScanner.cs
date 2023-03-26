namespace WellRoundedBalance.Equipment.Orange
{
    public class RadarScanner : EquipmentBase
    {
        public override string Name => ":: Equipment :: Radar Scanner";
        public override EquipmentDef InternalPickup => RoR2Content.Equipment.Scanner;

        public override string PickupText => "Reveal all nearby interactables.";

        public override string DescText => "<style=cIsUtility>Reveal</style> all interactables within " + range + "m for <style=cIsUtility>" + duration + " seconds</style>.";

        [ConfigField("Cooldown", "", 25f)]
        public static float cooldown;

        [ConfigField("Duration", "", 10f)]
        public static float duration;

        [ConfigField("Range", "", 1000f)]
        public static float range;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();

            var Radar = Utils.Paths.EquipmentDef.Scanner.Load<EquipmentDef>();
            Radar.cooldown = cooldown;
        }

        private void Changes()
        {
            var scanner = Utils.Paths.GameObject.ChestScanner.Load<GameObject>().GetComponent<ChestRevealer>();
            scanner.radius = range;
            scanner.pulseTravelSpeed = range / 4f;
            scanner.revealDuration = duration;
            scanner.pulseInterval = duration;
        }
    }
}