namespace WellRoundedBalance.Equipment
{
    public class Cooldowns : EquipmentBase
    {
        public override string Name => ":: Equipment : Cooldowns";
        public override EquipmentDef InternalPickup => null;

        public override string PickupText => "";

        public override string DescText => "";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        public static void Changes()
        {
            var Backup = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/DroneBackup/DroneBackup.asset").WaitForCompletion();
            Backup.cooldown = 75f;

            var Helfire = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/BurnNearby/BurnNearby.asset").WaitForCompletion();
            Helfire.cooldown = 60f;

            var Tonic = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/Tonic/Tonic.asset").WaitForCompletion();
            Tonic.cooldown = 75f;

            var Leech = Utils.Paths.EquipmentDef.LifestealOnHit.Load<EquipmentDef>();
            Leech.cooldown = 50f;
        }
    }
}