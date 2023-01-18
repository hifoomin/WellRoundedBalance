using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Lunars
{
    public class LightFluxPauldron : ItemBase
    {
        public override string Name => ":: Items ::::: Lunars :: Light Flux Pauldron";
        public override string InternalPickupToken => "halfAttackSpeedHalfCooldowns";

        public override string PickupText => "Increase movement speed while airborne... <color=#FF7F7F>BUT getting hit forcibly pulls you to the ground.</color>\n";

        public override string DescText => "Increase <style=cIsUtility>movement speed</style> by <style=cIsUtility>30%</style> <style=cStack>(+30% per stack)</style> while airborne. Getting hit forcibly <style=cIsUtility>pulls</style> you to the ground. <style=cStack>(Pull strength increases per stack)</style>.";

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
        }

        private void CharacterMotor_OnLeaveStableGround(On.RoR2.CharacterMotor.orig_OnLeaveStableGround orig, CharacterMotor self)
        {
            self.body.statsDirty = true;
            orig(self);
        }

        private void CharacterMotor_OnLanded(On.RoR2.CharacterMotor.orig_OnLanded orig, CharacterMotor self)
        {
            self.body.statsDirty = true;
            orig(self);
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory && sender.characterMotor)
            {
                var stack = sender.inventory.GetItemCount(DLC1Content.Items.HalfAttackSpeedHalfCooldowns);
                if (!sender.characterMotor.isGrounded) args.moveSpeedMultAdd += 0.3f * stack;
            }
        }

        // enemies pulling player on hit
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
                            if (bodyMotor)
                            {
                                bodyMotor.velocity.y = Mathf.Min(bodyMotor.velocity.y, -40f - 30f * (stack - 1));
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
                Main.WRBLogger.LogError("Failed to apply Light Flux Pauldron Cooldown hook");
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
                Main.WRBLogger.LogError("Failed to apply Light Flux Pauldron Attack Speed hook");
            }
        }
    }
}