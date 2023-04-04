using MonoMod.Cil;

namespace WellRoundedBalance.Mechanics.Health
{
    public class OneShotProtection : MechanicBase<OneShotProtection>
    {
        public static BuffDef osp;
        public override string Name => ":: Mechanics :: One Shot Protection";

        [ConfigField("Percent Threshold", "Decimal.", 0.1f)]
        public static float percentThreshold;

        [ConfigField("Invincibility Duration", "", 0.5f)]
        public static float invincibilityDuration;

        public override void Init()
        {
            var shield = Utils.Paths.Texture2D.texBuffGenericShield.Load<Texture2D>();

            osp = ScriptableObject.CreateInstance<BuffDef>();
            osp.isHidden = false;
            osp.canStack = false;
            osp.isCooldown = true;
            osp.buffColor = new Color32(35, 167, 255, 255);
            osp.iconSprite = Sprite.Create(shield, new Rect(0f, 0f, (float)shield.width, (float)shield.height), new Vector2(0f, 0f));
            osp.name = "One Shot Protection Invinciblity Duration";

            ContentAddition.AddBuffDef(osp);
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TriggerOneShotProtection += ChangeTime;
            On.RoR2.HealthComponent.TriggerOneShotProtection += HealthComponent_TriggerOneShotProtection;
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
        }

        private void HealthComponent_TriggerOneShotProtection(On.RoR2.HealthComponent.orig_TriggerOneShotProtection orig, HealthComponent self)
        {
            orig(self);
            if (NetworkServer.active)
            {
                EffectManager.SpawnEffect(EffectCatalog.FindEffectIndexFromPrefab(Utils.Paths.GameObject.MuzzleflashMageLightningLargeWithTrail.Load<GameObject>()),
                         new EffectData { origin = self.body.corePosition, scale = self.body.radius, rotation = self.transform.rotation },
                         true);
                self.body.AddTimedBuff(osp, invincibilityDuration);
                Util.PlaySound("Play_item_proc_shocknearby_stop", self.gameObject);
            }
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody characterBody)
        {
            if (characterBody.isPlayerControlled)
            {
                characterBody.oneShotProtectionFraction = percentThreshold;
            }
        }

        public static void ChangeTime(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.1f)))
            {
                c.Next.Operand = invincibilityDuration;
            }
            else
            {
                Logger.LogError("Failed to apply One Shot Protection Time hook");
            }
        }
    }
}