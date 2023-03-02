using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Whites
{
    internal class PowerElixir : ItemBase<PowerElixir>
    {
        public static BuffDef regen;
        public override string Name => ":: Items : Whites :: Power Elixir";

        public override string InternalPickupToken => "healingPotion";

        public override string PickupText => "Quickly regenerate upon taking heavy damage. Recharges each stage.";

        public override string DescText => $"Taking damage to below <style=cIsHealth>{d(healthThreshold)} health</style> <style=cIsUtility>consumes</style> this item, <style=cIsHealing>healing</style> you for <style=cIsHealing>{d(percentHealing)}</style> of your <style=cIsHealing>maximum health</style> over <style=cIsUtility>{healingTime}s</style>." +
            (refillEveryStage ? " <style=cIsUtility>Refills every stage</style>." : "");

        [ConfigField("Health Threshold", "Decimal.", 0.5f)]
        public static float healthThreshold;

        [ConfigField("Percent Healing", "Decimal.", 0.25f)]
        public static float percentHealing;

        [ConfigField("Healing Time", 4f)]
        public static float healingTime;

        [ConfigField("Refill Every Stage?", true)]
        public static bool refillEveryStage;

        public override void Init()
        {
            var medkitIcon = Utils.Paths.Texture2D.texBuffMedkitHealIcon.Load<Texture2D>();

            regen = ScriptableObject.CreateInstance<BuffDef>();
            regen.isHidden = false;
            regen.isDebuff = false;
            regen.canStack = false;
            regen.buffColor = new Color32(255, 132, 115, 255);
            regen.iconSprite = Sprite.Create(medkitIcon, new Rect(0f, 0f, medkitIcon.width, medkitIcon.height), new Vector2(0f, 0f));
            regen.name = "Power Elixir Regen";

            ContentAddition.AddBuffDef(regen);

            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.UpdateLastHitTime += HealthComponent_UpdateLastHitTime;
            IL.RoR2.HealthComponent.ServerFixedUpdate += HealthComponent_ServerFixedUpdate;
            On.RoR2.Stage.Start += Stage_Start;
            On.RoR2.HealthComponent.UpdateLastHitTime += HealthComponent_UpdateLastHitTime1;
        }

        private void HealthComponent_UpdateLastHitTime1(On.RoR2.HealthComponent.orig_UpdateLastHitTime orig, HealthComponent self, float damageValue, Vector3 damagePosition, bool damageIsSilent, GameObject attacker)
        {
            if (self.itemCounts.healingPotion > 0 && !self.body.HasBuff(regen))
            {
                float healthFraction = self.health / self.fullHealth;
                if (healthFraction < healthThreshold)
                {
                    self.body.inventory.RemoveItem(DLC1Content.Items.HealingPotion, 1);
                    self.body.inventory.GiveItem(DLC1Content.Items.HealingPotionConsumed, 1);
                    CharacterMasterNotificationQueue.SendTransformNotification(self.body.master, DLC1Content.Items.HealingPotion.itemIndex, DLC1Content.Items.HealingPotionConsumed.itemIndex, CharacterMasterNotificationQueue.TransformationType.Default);

                    self.body.AddTimedBuff(regen, healingTime);

                    EffectData effectData = new()
                    {
                        origin = self.transform.position
                    };
                    effectData.SetNetworkedObjectReference(self.gameObject);
                    EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/HealingPotionEffect"), effectData, true);
                }
            }
            orig(self, damageValue, damagePosition, damageIsSilent, attacker);
        }

        private void Stage_Start(On.RoR2.Stage.orig_Start orig, Stage self)
        {
            orig(self);
            if (CharacterMaster.instancesList != null && refillEveryStage)
            {
                foreach (CharacterMaster cm in CharacterMaster.instancesList)
                {
                    if (cm.inventory)
                    {
                        int brokenElixirCount = cm.inventory.GetItemCount(DLC1Content.Items.HealingPotionConsumed);
                        if (brokenElixirCount > 0)
                        {
                            cm.inventory.RemoveItem(DLC1Content.Items.HealingPotionConsumed, brokenElixirCount);
                            cm.inventory.GiveItem(DLC1Content.Items.HealingPotion, brokenElixirCount);
                            CharacterMasterNotificationQueue.SendTransformNotification(cm, DLC1Content.Items.HealingPotionConsumed.itemIndex, DLC1Content.Items.HealingPotion.itemIndex, CharacterMasterNotificationQueue.TransformationType.Default);
                        }
                    }
                }
            }
        }

        private void HealthComponent_ServerFixedUpdate(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(x => x.MatchStfld(typeof(HealthComponent), nameof(HealthComponent.regenAccumulator))))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, HealthComponent, float>>((regenAccumulator, self) =>
                {
                    if (self.body.HasBuff(regen)) regenAccumulator += Time.fixedDeltaTime * (percentHealing / healingTime) * self.fullHealth;
                    return regenAccumulator;
                });
            }
            else Main.WRBLogger.LogError("Failed to apply Power Elixir Regen hook");
        }

        private void HealthComponent_UpdateLastHitTime(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.After, x => x.MatchLdfld(typeof(HealthComponent.ItemCounts), "healingPotion")))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldc_I4_0);
            }
            else Main.WRBLogger.LogError("Failed to apply Power Elixir Count hook");
        }
    }
}