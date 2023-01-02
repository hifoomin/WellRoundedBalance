using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using R2API;

namespace WellRoundedBalance.Items.Lunars
{
    public class MercurialRachis : ItemBase
    {
        public override string Name => ":: Items ::::: Lunars :: Mercurial Rachis";
        public override string InternalPickupToken => "randomDamageZone";

        public override string PickupText => "Randomly create a Ward of Power. ALL characters have bonus stats while in the Ward.";

        public override string DescText => "Creates a Ward of Power in a random location nearby that buffs both enemies and allies within <style=cIsUtility>24m</style> <style=cStack>(+50% per stack)</style>, causing them to gain <style=cIsDamage>25%</style> attack speed and <style=cIsUtility>movement speed</style>. Enemies benefit from the ward twice as much.";

        public static BuffDef rachisBuff;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            AddBuff();
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
            On.RoR2.Items.RandomDamageZoneBodyBehavior.FixedUpdate += RandomDamageZoneBodyBehavior_FixedUpdate;
            Changes();
        }

        private void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.teamComponent._teamIndex == TeamIndex.Player && sender.HasBuff(rachisBuff))
            {
                args.baseAttackSpeedAdd += 0.25f;
                args.moveSpeedMultAdd += 0.25f;
            }
            if (sender && sender.teamComponent._teamIndex != TeamIndex.Player && sender.HasBuff(rachisBuff))
            {
                args.baseAttackSpeedAdd += 0.5f;
                args.moveSpeedMultAdd += 0.5f;
            }
        }

        private void AddBuff()
        {
            rachisBuff = ScriptableObject.CreateInstance<BuffDef>();

            var rachisSprite = Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/RandomDamageZone/texBuffPowerIcon.tif").WaitForCompletion();
            rachisBuff.canStack = false;
            rachisBuff.isDebuff = false;
            rachisBuff.name = "Mercurial Rachis Buff";
            rachisBuff.iconSprite = Sprite.Create(rachisSprite, new Rect(0f, 0f, (float)rachisSprite.width, (float)rachisSprite.height), new Vector2(0f, 0f));
            rachisBuff.buffColor = new Color32(96, 123, 239, 225);
            ContentAddition.AddBuffDef(rachisBuff);
        }

        private void RandomDamageZoneBodyBehavior_FixedUpdate(On.RoR2.Items.RandomDamageZoneBodyBehavior.orig_FixedUpdate orig, RoR2.Items.RandomDamageZoneBodyBehavior self)
        {
            RoR2.Items.RandomDamageZoneBodyBehavior.baseWardRadius = 24f;
            RoR2.Items.RandomDamageZoneBodyBehavior.wardDuration = 25f;
            RoR2.Items.RandomDamageZoneBodyBehavior.wardRadiusMultiplierPerStack = 1 + 0.5f;
            orig(self);
        }

        private void Changes()
        {
            var ward = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/RandomDamageZone/DamageZoneWard.prefab").WaitForCompletion().GetComponent<BuffWard>();
            ward.buffDef = rachisBuff;
        }
    }
}