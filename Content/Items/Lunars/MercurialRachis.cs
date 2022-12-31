/*
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

        public override string PickupText => "Randomly create a Ward of Power. ALL characters " +
                                             ((AttackSpeed > 0 || MoveSpeed > 0) && Damage > 0 ? "have bonus stats" : "deal bonus damage ") +
                                             "while in the Ward.";

        public override string DescText => "Creates a Ward of Power in a random location nearby that buffs both enemies and allies within <style=cIsUtility>" + Radius + "m</style> <style=cStack>(+" + d(StackRadius) + " per stack)</style>, causing them to deal <style=cIsDamage>+50%</style> damage" +
                                           (AttackSpeed > 0 ? " and gain <style=cIsDamage>" + AttackSpeed + "%</style> attack speed" : ".") +
                                           (MoveSpeed > 0 ? " and gain <style=cIsUtility>" + d(MoveSpeed) + "%</style> movement speed." : ".") +
                                           (EnemyMult != 1 ? "Enemies benefit from the buff <style=cIsHealth>" + d(EnemyMult) + " more</style>." : ".");

        public static BuffDef rachisBuff;
        public static float Damage;
        public static float Radius;
        public static float StackRadius;
        public static float Duration;
        public static int MaxWards;
        public static float AttackSpeed;
        public static float MoveSpeed;
        public static float EnemyMult;

        public override void Init()
        {
            Damage = ConfigOption(0.5f, "Damage", "Decimal. Vanilla is 0.5");
            Radius = ConfigOption(16, "Base Radius", "Vanilla is 16");
            StackRadius = ConfigOption(0.5f, "Stack Radius", "Decimal. Vanilla is 0.5");
            Duration = ConfigOption(25f, "Ward Duration", "Vanilla is 25");
            MaxWards = ConfigOption(1, "Maximum Wards", "Vanilla is 1");
            AttackSpeed = ConfigOption(0f, "Attack Speed", "Vanilla is 0");
            MoveSpeed = ConfigOption(0f, "Movement Speed", "Decimal. Vanilla is 0");
            EnemyMult = ConfigOption(1f, "Enemy Buff Stat Multiplier", "Decimal. Vanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
            AddBuff();
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
            On.RoR2.Items.RandomDamageZoneBodyBehavior.FixedUpdate += RandomDamageZoneBodyBehavior_FixedUpdate;
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += ChangeLimit;
            Changes();
        }

        private void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.teamComponent._teamIndex == TeamIndex.Player && sender.HasBuff(rachisBuff))
            {
                args.damageMultAdd += Damage;
                args.baseAttackSpeedAdd += AttackSpeed;
                args.moveSpeedMultAdd += MoveSpeed;
            }
            if (sender && sender.teamComponent._teamIndex != TeamIndex.Player && sender.HasBuff(rachisBuff))
            {
                args.damageMultAdd += Damage * EnemyMult;
                args.baseAttackSpeedAdd += AttackSpeed * EnemyMult;
                args.moveSpeedMultAdd += MoveSpeed * EnemyMult;
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
            R2API.ContentAddition.AddBuffDef(rachisBuff);
        }

        private void RandomDamageZoneBodyBehavior_FixedUpdate(On.RoR2.Items.RandomDamageZoneBodyBehavior.orig_FixedUpdate orig, RoR2.Items.RandomDamageZoneBodyBehavior self)
        {
            RoR2.Items.RandomDamageZoneBodyBehavior.baseWardRadius = Radius;
            RoR2.Items.RandomDamageZoneBodyBehavior.wardDuration = Duration;
            RoR2.Items.RandomDamageZoneBodyBehavior.wardRadiusMultiplierPerStack = 1 + StackRadius;
            orig(self);
        }

        public static int ChangeLimit(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
        {
            if (slot is DeployableSlot.PowerWard)
            {
                return MaxWards;
            }
            else
            {
                return orig(self, slot);
            }
        }

        private void Changes()
        {
            var ward = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/RandomDamageZone/DamageZoneWard.prefab").WaitForCompletion().GetComponent<BuffWard>();
            ward.buffDef = rachisBuff;
        }
    }
}
*/