using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Lunars
{
    public class LightFluxPauldron : ItemBase
    {
        public override string Name => ":: Items ::::: Lunars :: Light Flux Pauldron";
        public override ItemDef InternalPickup => DLC1Content.Items.HalfAttackSpeedHalfCooldowns;

        public override string PickupText => "Increase movement speed while airborne... <color=#FF7F7F>BUT getting hit forcibly pulls you to the ground.</color>\n";

        public override string DescText => "Increase <style=cIsUtility>movement speed</style> by <style=cIsUtility>30%</style> <style=cStack>(+30% per stack)</style> while airborne. Getting hit forcibly <style=cIsUtility>pulls</style> you to the ground. <style=cStack>(Pull strength increases per stack)</style>.";

        [ConfigField("Base Movement Speed Gain", "Decimal.", 0.3f)]
        public static float baseMovementSpeedGain;

        [ConfigField("Movement Speed Gain Per Stack", "Decimal.", 0.3f)]
        public static float movementSpeedGainPerStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += Changes;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.CharacterMotor.OnLanded += CharacterMotor_OnLanded;
            On.RoR2.CharacterMotor.OnLeaveStableGround += CharacterMotor_OnLeaveStableGround;
            On.RoR2.Inventory.GiveItem_ItemIndex_int += Inventory_GiveItem_ItemIndex_int;
        }

        private void Inventory_GiveItem_ItemIndex_int(On.RoR2.Inventory.orig_GiveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)
        {
            orig(self, itemIndex, count);
            if (NetworkServer.active && itemIndex == DLC1Content.Items.HalfAttackSpeedHalfCooldowns.itemIndex)
            {
                var master = self.gameObject.GetComponent<CharacterMaster>();
                if (master)
                {
                    var body = master.GetBodyObject();

                    if (body)
                    {
                        var lfpc = body.GetComponent<LightFluxPauldronComponent>();
                        if (lfpc == null)
                        {
                            body.AddComponent<LightFluxPauldronComponent>();
                        }
                        else
                        {
                            lfpc.Refresh();
                        }
                    }
                }
            }
        }

        private void CharacterMotor_OnLeaveStableGround(On.RoR2.CharacterMotor.orig_OnLeaveStableGround orig, CharacterMotor self)
        {
            var body = self.body;
            if (body)
            {
                var inventory = body.inventory;
                if (inventory && inventory.GetItemCount(DLC1Content.Items.HalfAttackSpeedHalfCooldowns) > 0)
                {
                    self.body.statsDirty = true;
                }
            }
            orig(self);
        }

        private void CharacterMotor_OnLanded(On.RoR2.CharacterMotor.orig_OnLanded orig, CharacterMotor self)
        {
            var body = self.body;
            if (body)
            {
                var inventory = body.inventory;
                if (inventory && inventory.GetItemCount(DLC1Content.Items.HalfAttackSpeedHalfCooldowns) > 0)
                {
                    self.body.statsDirty = true;
                }
            }

            orig(self);
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory && sender.characterMotor)
            {
                var stack = sender.inventory.GetItemCount(DLC1Content.Items.HalfAttackSpeedHalfCooldowns);
                if (!sender.characterMotor.isGrounded) args.moveSpeedMultAdd += baseMovementSpeedGain + movementSpeedGainPerStack * (stack - 1);
            }
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var attacker = damageInfo.attacker;
            if (attacker)
            {
                var body = self.body;
                if (body)
                {
                    var attackerBody = attacker.GetComponent<CharacterBody>();
                    var inventory = body.inventory;
                    if (attackerBody)
                    {
                        if (inventory)
                        {
                            // enemies pulling player on hit
                            var stack = inventory.GetItemCount(DLC1Content.Items.HalfAttackSpeedHalfCooldowns);
                            var bodyMotor = body.characterMotor;
                            if (bodyMotor && stack > 0)
                            {
                                bodyMotor.velocity.y = Mathf.Min((Mathf.Abs(bodyMotor.velocity.y) - body.jumpPower - 35f) * damageInfo.procCoefficient * globalProc * stack, 0f);
                                Main.WRBLogger.LogFatal("body y velocity is " + bodyMotor.velocity.y);
                            }
                        }
                    }
                }
            }
            orig(self, damageInfo);
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchStloc(89),
                    x => x.MatchBr(out _),
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdcR4(0.5f)))
            {
                c.Index += 3;
                c.Next.Operand = 1f;
            }
            else
            {
                Logger.LogError("Failed to apply Light Flux Pauldron Cooldown hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
               x => x.MatchLdloc(83),
               x => x.MatchLdloc(43)))
            {
                c.Index += 2;
                c.EmitDelegate<Func<int, int>>((useless) =>
                {
                    return 0;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Light Flux Pauldron Attack Speed hook");
            }
        }
    }

    public class LightFluxPauldronComponent : MonoBehaviour
    {
        public CharacterBody body;
        public Inventory inventory;
        public CharacterMotor motor;
        public int stack;
        public float currentAirControl;
        public float interval = 0.2f;
        public float timer;

        private void Start()
        {
            body = GetComponent<CharacterBody>();
            motor = GetComponent<CharacterMotor>();
            if (body)
            {
                inventory = body.inventory;
            }
        }

        public void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer >= interval)
            {
                timer = 0f;
                if (inventory)
                {
                    stack = inventory.GetItemCount(DLC1Content.Items.HalfAttackSpeedHalfCooldowns);
                    Refresh();
                }
            }
        }

        public void Refresh()
        {
            if (motor && inventory)
            {
                if (stack > 0)
                {
                    motor.airControl = 1f;
                }
                else
                {
                    motor.airControl = 0.25f;
                }

                currentAirControl = motor.airControl;
            }
        }
    }
}