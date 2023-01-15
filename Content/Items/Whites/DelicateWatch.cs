using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace WellRoundedBalance.Items.Whites
{
    public class DelicateWatch : ItemBase
    {
        public static BuffDef watchDamage;
        public static BuffDef watchSpeed;

        public override string Name => ":: Items : Whites :: Delicate Watch";
        public override string InternalPickupToken => "fragileDamageBonus";

        public override string PickupText => "Every 12 seconds, switch between dealing bonus damage and moving faster. Breaks at low health.";
        public override string DescText => "Every <style=cIsUtility>12</style> seconds, switch between increasing damage by <style=cIsDamage>12%</style> <style=cStack>(+12% per stack)</style> and increasing movement speed by <style=cIsUtility>12%</style> <style=cStack>(+12% per stack)</style>. Taking damage to below <style=cIsHealth>25% health</style> <style=cIsUtility>breaks</style> this item.";

        public override void Init()
        {
            var damageIcon = Utils.Paths.Texture2D.texBuffFullCritIcon.Load<Texture2D>();
            var speedIcon = Utils.Paths.Texture2D.texBuffKillMoveSpeed.Load<Texture2D>();

            watchDamage = ScriptableObject.CreateInstance<BuffDef>();
            watchDamage.isHidden = false;
            watchDamage.canStack = false;
            watchDamage.isDebuff = false;
            watchDamage.buffColor = new Color32(208, 165, 136, 255);
            watchDamage.iconSprite = Sprite.Create(damageIcon, new Rect(0f, 0f, (float)damageIcon.width, (float)damageIcon.height), new Vector2(0f, 0f));

            watchSpeed = ScriptableObject.CreateInstance<BuffDef>();
            watchSpeed.isHidden = false;
            watchSpeed.canStack = false;
            watchSpeed.isDebuff = false;
            watchSpeed.buffColor = new Color32(208, 165, 136, 255);
            watchSpeed.iconSprite = Sprite.Create(speedIcon, new Rect(0f, 0f, (float)damageIcon.width, (float)damageIcon.height), new Vector2(0f, 0f));

            ContentAddition.AddBuffDef(watchDamage);
            ContentAddition.AddBuffDef(watchSpeed);

            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += ChangeDamage;
            IL.RoR2.HealthComponent.UpdateLastHitTime += ChangeThreshold;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(DLC1Content.Items.FragileDamageBonus);
                if (sender.HasBuff(watchSpeed))
                {
                    args.moveSpeedMultAdd += 0.12f * stack;
                }
                if (sender.HasBuff(watchDamage))
                {
                    args.damageMultAdd += 0.12f * stack;
                }
            }
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            if (NetworkServer.active)
            {
                var stack = body.inventory.GetItemCount(DLC1Content.Items.FragileDamageBonus);
                body.AddItemBehavior<DelicateWatchController>(stack);
            }
        }

        private void ChangeThreshold(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld<HealthComponent.ItemCounts>("fragileDamageBonus"),
                x => x.MatchLdcI4(0),
                x => x.MatchBle(out _),
                x => x.MatchLdarg(0),
                x => x.MatchCallOrCallvirt<HealthComponent>("get_isHealthLow")
            ))
            {
                c.Index += 5;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<bool, HealthComponent, bool>>((Check, self) =>
                {
                    if ((self.health + self.shield) / self.fullCombinedHealth < 0.25f)
                    {
                        Check = true;
                        return Check;
                    }
                    else
                    {
                        Check = false;
                        return Check;
                    }
                });
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Delicate Watch Threshold hook");
            }
        }

        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchBle(out _),
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdcR4(1),
                    x => x.MatchLdloc(out _),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.2f)))
            {
                c.Index += 5;
                c.Next.Operand = 0f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Delicate Watch Damage hook");
            }
        }
    }

    public class DelicateWatchController : CharacterBody.ItemBehavior
    {
        public float currentTime;
        public float interval = 12f;

        public void Start()
        {
            currentTime = 11f + 59 / 60f;
        }

        public void FixedUpdate()
        {
            currentTime += Time.fixedDeltaTime;
            if (currentTime >= interval)
            {
                if (body.HasBuff(DelicateWatch.watchDamage))
                {
                    body.RemoveBuff(DelicateWatch.watchDamage);
                }
                else
                {
                    body.AddBuff(DelicateWatch.watchDamage);
                }

                if (body.HasBuff(DelicateWatch.watchSpeed))
                {
                    body.RemoveBuff(DelicateWatch.watchSpeed);
                }
                else if (!body.HasBuff(DelicateWatch.watchDamage))
                {
                    body.AddBuff(DelicateWatch.watchSpeed);
                }
                currentTime = 0;
            }
        }
    }
}