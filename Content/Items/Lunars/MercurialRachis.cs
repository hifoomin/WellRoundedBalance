namespace WellRoundedBalance.Items.Lunars
{
    public class MercurialRachis : ItemBase<MercurialRachis>
    {
        public override string Name => ":: Items ::::: Lunars :: Mercurial Rachis";
        public override ItemDef InternalPickup => RoR2Content.Items.RandomDamageZone;

        public override string PickupText => "Randomly create a Ward of Power. ALL characters have bonus stats while in the Ward.";

        public override string DescText => "Creates a Ward of Power in a random location nearby that buffs both enemies and allies within <style=cIsUtility>" + baseRadius + "m</style> <style=cStack>(+" + d(radiusIncreasePerStack) + " per stack)</style>, causing them to gain <style=cIsDamage>" + d(attackSpeedAndMovementSpeedGain) + " attack speed</style> and <style=cIsUtility>movement speed</style>. Enemies benefit from the ward twice as much.";

        public static BuffDef rachisBuff;

        [ConfigField("Attack Speed and Movement Speed Gain", "", 0.35f)]
        public static float attackSpeedAndMovementSpeedGain;

        [ConfigField("Base Radius", "", 30f)]
        public static float baseRadius;

        [ConfigField("Radius Increase Per Stack", "Decimal.", 0.5f)]
        public static float radiusIncreasePerStack;

        [ConfigField("Ward Duration", "", 25f)]
        public static float wardDuration;

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
                args.baseAttackSpeedAdd += attackSpeedAndMovementSpeedGain;
                args.moveSpeedMultAdd += attackSpeedAndMovementSpeedGain;
            }
            if (sender && sender.teamComponent._teamIndex != TeamIndex.Player && sender.HasBuff(rachisBuff))
            {
                args.baseAttackSpeedAdd += attackSpeedAndMovementSpeedGain * 2f;
                args.moveSpeedMultAdd += attackSpeedAndMovementSpeedGain * 2f;
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
            RoR2.Items.RandomDamageZoneBodyBehavior.baseWardRadius = baseRadius;
            RoR2.Items.RandomDamageZoneBodyBehavior.wardDuration = wardDuration;
            RoR2.Items.RandomDamageZoneBodyBehavior.wardRadiusMultiplierPerStack = 1 + radiusIncreasePerStack;
            orig(self);
        }

        private void Changes()
        {
            var ward = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/RandomDamageZone/DamageZoneWard.prefab").WaitForCompletion().GetComponent<BuffWard>();
            ward.buffDef = rachisBuff;
        }
    }
}