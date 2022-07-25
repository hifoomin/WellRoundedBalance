using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UltimateCustomRun.Items.Equipment
{
    public class All : GlobalBase
    {
        public override string Name => "::: Equipment :: All";
        public static float BlastShowerCd;
        public static float DisposableCd;
        public static float VaseCd;
        public static float CardCd;
        public static float FruitCd;
        public static float FmpCd;
        public static float WoodspriteCd;
        public static float GooboCd;
        public static float GoragCd;
        public static float ElephantCd;
        public static float MilkyCd;
        public static float MolotovCd;
        public static float HudCd;
        public static float PreonCd;
        public static float CubeCd;
        public static float RadarCd;
        public static float RecyclerCd;
        public static float CaffeinatorCd;
        public static float CapacitorCd;
        public static float SawmerangCd;
        public static float LeechCd;
        public static float BackupCd;
        public static float EggCd;
        public static float EffigyCd;
        public static float MeteoriteCd;
        public static float HelfireCd;
        public static float TonicCd;

        public override void Init()
        {
            BlastShowerCd = ConfigOption(20f, "Blast Shower Cooldown", "Vanilla is 20");
            DisposableCd = ConfigOption(45f, "Disposable Missile Launcher Cooldown", "Vanilla is 45");
            VaseCd = ConfigOption(45f, "Eccentric Vase Cooldown", "Vanilla is 45");
            CardCd = ConfigOption(0.1f, "Executive Card Cooldown", "Vanilla is 0.1");
            FruitCd = ConfigOption(45f, "Foreign Fruit Cooldown", "Vanilla is 45");
            FmpCd = ConfigOption(45f, "Forgive Me Please Cooldown", "Vanilla is 45");
            WoodspriteCd = ConfigOption(15f, "Gnarled Woodsprite Cooldown", "Vanilla is 15");
            GooboCd = ConfigOption(100f, "Goobo Jr Cooldown", "Vanilla is 100");
            GoragCd = ConfigOption(45f, "Gorags Opus Cooldown", "Vanilla is 45");
            ElephantCd = ConfigOption(45f, "Jade Elephant Cooldown", "Vanilla is 45");
            MilkyCd = ConfigOption(60f, "Milky Chrysalis Cooldown", "Vanilla is 60");
            MolotovCd = ConfigOption(45f, "Molotov 6 Pack Cooldown", "Vanilla is 45");
            HudCd = ConfigOption(60f, "Ocular HUD Cooldown", "Vanilla is 60");
            PreonCd = ConfigOption(140f, "Preon Accumulator Cooldown", "Vanilla is 140");
            CubeCd = ConfigOption(60f, "Primordial Cube Cooldown", "Vanilla is 60");
            RadarCd = ConfigOption(45f, "Radar Scanner Cooldown", "Vanilla is 45");
            RecyclerCd = ConfigOption(45f, "Recycler Cooldown", "Vanilla is 45");
            CaffeinatorCd = ConfigOption(60f, "Remote Caffeinator Cooldown", "Vanilla is 60");
            CapacitorCd = ConfigOption(20f, "Royal Capacitor Cooldown", "Vanilla is 20");
            SawmerangCd = ConfigOption(45f, "Sawmerang Cooldown", "Vanilla is 45");
            LeechCd = ConfigOption(60f, "Super Massive Leech Cooldown", "Vanilla is 60");
            BackupCd = ConfigOption(100f, "The Backup Cooldown", "Vanilla is 100");
            EggCd = ConfigOption(30f, "Volcanic Egg Cooldown", "Vanilla is 30");
            EffigyCd = ConfigOption(15f, "Effigy of Grief Cooldown", "Vanilla is 15");
            MeteoriteCd = ConfigOption(140f, "Meteorite Cooldown", "Vanilla is 140");
            HelfireCd = ConfigOption(45f, "Helfire Tincture Cooldown", "Vanilla is 45");
            TonicCd = ConfigOption(60f, "Spinel Tonic Cooldown", "Vanilla is 60");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        public static void Changes()
        {
            var BlastShower = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/Cleanse/Cleanse.asset").WaitForCompletion();
            BlastShower.cooldown = BlastShowerCd;

            var DML = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/CommandMissile/CommandMissile.asset").WaitForCompletion();
            DML.cooldown = DisposableCd;

            var Vase = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/DLC1/MultiShopCard/MultiShopCard.asset").WaitForCompletion();
            Vase.cooldown = VaseCd;

            var Card = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/Gateway/Gateway.asset").WaitForCompletion();
            Card.cooldown = CardCd;

            var Fruit = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/Fruit/Fruit.asset").WaitForCompletion();
            Fruit.cooldown = FruitCd;

            var Fmp = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/DeathProjectile/DeathProjectile.asset").WaitForCompletion();
            Fmp.cooldown = FmpCd;

            var Woodsprite = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/PassiveHealing/PassiveHealing.asset").WaitForCompletion();
            Woodsprite.cooldown = WoodspriteCd;

            var Goobo = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/DLC1/GummyClone/GummyClone.asset").WaitForCompletion();
            Goobo.cooldown = GooboCd;

            var Gorag = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/TeamWarCry/TeamWarCry.asset").WaitForCompletion();
            Gorag.cooldown = GoragCd;

            var Elephant = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/GainArmor/GainArmor.asset").WaitForCompletion();
            Elephant.cooldown = ElephantCd;

            var Milky = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/Jetpack/Jetpack.asset").WaitForCompletion();
            Milky.cooldown = MilkyCd;

            var Molotov = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/DLC1/Molotov/Molotov.asset").WaitForCompletion();
            Molotov.cooldown = MolotovCd;

            var Hud = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/CritOnUse/CritOnUse.asset").WaitForCompletion();
            Hud.cooldown = HudCd;

            var Preon = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/BFG/BFG.asset").WaitForCompletion();
            Preon.cooldown = PreonCd;

            var Cube = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/Blackhole/Blackhole.asset").WaitForCompletion();
            Cube.cooldown = CubeCd;

            var Radar = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/Scanner/Scanner.asset").WaitForCompletion();
            Radar.cooldown = RadarCd;

            var Recycler = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/Recycle/Recycle.asset").WaitForCompletion();
            Recycler.cooldown = RecyclerCd;

            var Caffeinator = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/DLC1/VendingMachine/VendingMachine.asset").WaitForCompletion();
            Caffeinator.cooldown = CaffeinatorCd;

            var Capacitor = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/Lightning/Lightning.asset").WaitForCompletion();
            Capacitor.cooldown = CapacitorCd;

            var Sawmerang = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/Saw/Saw.asset").WaitForCompletion();
            Sawmerang.cooldown = SawmerangCd;

            var Leech = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/LifestealOnHit/LifestealOnHit.asset").WaitForCompletion();
            Leech.cooldown = LeechCd;

            var Backup = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/DroneBackup/DroneBackup.asset").WaitForCompletion();
            Backup.cooldown = BackupCd;

            var Egg = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/FireBallDash/FireBallDash.asset").WaitForCompletion();
            Egg.cooldown = EggCd;

            var Effigy = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/CrippleWard/CrippleWard.asset").WaitForCompletion();
            Effigy.cooldown = EffigyCd;

            var Meteorite = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/Meteor/Meteor.asset").WaitForCompletion();
            Meteorite.cooldown = MeteoriteCd;

            var Helfire = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/BurnNearby/BurnNearby.asset").WaitForCompletion();
            Helfire.cooldown = HelfireCd;

            var Tonic = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/Tonic/Tonic.asset").WaitForCompletion();
            Tonic.cooldown = TonicCd;
        }
    }
}