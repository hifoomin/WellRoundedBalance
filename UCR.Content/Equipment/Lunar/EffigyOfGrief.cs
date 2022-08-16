using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using R2API;
using MonoMod.Cil;

namespace UltimateCustomRun.Equipment
{
    public class EffigyOfGrief : EquipmentBase
    {
        public override string Name => "::: Equipment ::: Effigy Of Grief";
        public override string InternalPickupToken => "crippleWard";

        public override bool NewPickup => true;

        public override bool NewDesc => true;

        public override string PickupText => "Drop a permanent effigy that cripples ALL characters inside. Can place up to " + MaxEffigies + ".";

        public override string DescText => "ALL characters within are <style=cIsUtility>slowed by " + d(SpeedDebuff) + "</style> and have their <style=cIsDamage>armor reduced by " + ArmorDebuff + "</style>. Can place up to <style=cIsUtility>" + MaxEffigies + "</style>.";

        public static int MaxEffigies;
        public static float Radius;
        public static float SpeedDebuff;
        public static float ArmorDebuff;

        public static BuffDef CrippleEffigy;

        public override void Init()
        {
            MaxEffigies = ConfigOption(5, "Maximum Effigies", "Vanilla is 5");
            Radius = ConfigOption(16f, "Radius", "Vanilla is 16");
            SpeedDebuff = ConfigOption(0.5f, "Movement Speed Debuff", "Decimal. Vanilla is 0.5");
            ArmorDebuff = ConfigOption(20f, "Armor Debuff", "Vanilla is 20");
            base.Init();
        }

        public override void Hooks()
        {
            MakeBuff();
            Changes();
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += ChangeLimit;
        }

        private void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.HasBuff(CrippleEffigy))
            {
                args.moveSpeedReductionMultAdd += SpeedDebuff * 2f;
                args.armorAdd += -ArmorDebuff;
            }
        }

        private int ChangeLimit(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
        {
            if (slot is DeployableSlot.CrippleWard)
            {
                return MaxEffigies;
            }
            else
            {
                return orig(self, slot);
            }
        }

        private void MakeBuff()
        {
            CrippleEffigy = ScriptableObject.CreateInstance<BuffDef>();

            var crippleIcon = Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/texBuffCrippleIcon.tif").WaitForCompletion();

            CrippleEffigy.isHidden = false;
            CrippleEffigy.buffColor = new Color32(24, 177, 226, 225);
            CrippleEffigy.canStack = false;
            CrippleEffigy.isDebuff = true;
            CrippleEffigy.name = "Effigy of Grief Cripple";
            CrippleEffigy.iconSprite = Sprite.Create(crippleIcon, new Rect(0f, 0f, (float)crippleIcon.width, (float)crippleIcon.height), new Vector2(0f, 0f));
            R2API.ContentAddition.AddBuffDef(CrippleEffigy);
        }

        private void Changes()
        {
            var eff = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/CrippleWard/CrippleWard.prefab").WaitForCompletion().GetComponent<BuffWard>();
            eff.radius = Radius;
            eff.buffDef = CrippleEffigy;
        }
    }
}