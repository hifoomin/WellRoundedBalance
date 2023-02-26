using RoR2;
using UnityEngine.AddressableAssets;

namespace WellRoundedBalance.Equipment
{
    public class Cooldowns : EquipmentBase
    {
        public override string Name => ":: Equipment : Cooldowns";
        public override string InternalPickupToken => "guh";

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
            var BlastShower = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/Cleanse/Cleanse.asset").WaitForCompletion();
            BlastShower.cooldown = 15f;

            var DML = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/CommandMissile/CommandMissile.asset").WaitForCompletion();
            DML.cooldown = 30f;

            var Card = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/DLC1/MultiShopCard/MultiShopCard.asset").WaitForCompletion();
            Card.cooldown = 20f;

            var Fruit = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/Fruit/Fruit.asset").WaitForCompletion();
            Fruit.cooldown = 35f;

            var Goobo = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/DLC1/GummyClone/GummyClone.asset").WaitForCompletion();
            Goobo.cooldown = 60f;

            var Elephant = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/GainArmor/GainArmor.asset").WaitForCompletion();
            Elephant.cooldown = 30f;

            var Milky = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/Jetpack/Jetpack.asset").WaitForCompletion();
            Milky.cooldown = 45f;

            var Molotov = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/DLC1/Molotov/Molotov.asset").WaitForCompletion();
            Molotov.cooldown = 30f;

            var Hud = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/CritOnUse/CritOnUse.asset").WaitForCompletion();
            Hud.cooldown = 50f;

            var Preon = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/BFG/BFG.asset").WaitForCompletion();
            Preon.cooldown = 120f;

            var Radar = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/Scanner/Scanner.asset").WaitForCompletion();
            Radar.cooldown = 30f;

            var Backup = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/DroneBackup/DroneBackup.asset").WaitForCompletion();
            Backup.cooldown = 75f;

            var Helfire = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/BurnNearby/BurnNearby.asset").WaitForCompletion();
            Helfire.cooldown = 60f;

            var Tonic = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/Tonic/Tonic.asset").WaitForCompletion();
            Tonic.cooldown = 75f;

            var Leech = Utils.Paths.EquipmentDef.LifestealOnHit.Load<EquipmentDef>();
            Leech.cooldown = 50f;

            var Vase = Utils.Paths.EquipmentDef.Gateway.Load<EquipmentDef>();
            Vase.cooldown = 35f;
        }
    }
}