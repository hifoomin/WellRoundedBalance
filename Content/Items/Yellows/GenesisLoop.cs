using EntityStates.VagrantNovaItem;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Yellows
{
    public class GenesisLoop : ItemBase
    {
        public override string Name => ":: Items :::: Yellows :: Genesis Loop";
        public override ItemDef InternalPickup => RoR2Content.Items.NovaOnLowHealth;

        public override string PickupText => "Fire an electric nova at low health.";

        public override string DescText => "Falling below <style=cIsHealth>" + d(healthThreshold) + " health</style> causes you to explode, dealing <style=cIsDamage>" + d(baseDamage) + " base damage</style>. Recharges every <style=cIsUtility>" + cooldown + " seconds</style> <style=cStack>(-" + (cooldown / 2f) + "% per stack)</style>.";

        [ConfigField("Health Threshold", "Decimal.", 0.5f)]
        public static float healthThreshold;

        [ConfigField("Base Damage", "Decimal.", 40f)]
        public static float baseDamage;

        [ConfigField("Cooldown", "Formula for cooldown: Cooldown / (1 + Genesis Loop)", 60f)]
        public static float cooldown;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.VagrantNovaItem.DetonateState.OnEnter += Changes;
            IL.EntityStates.VagrantNovaItem.ReadyState.FixedUpdate += ChangeThreshold;
            On.EntityStates.VagrantNovaItem.RechargeState.FixedUpdate += RechargeState_FixedUpdate;
        }

        private void RechargeState_FixedUpdate(On.EntityStates.VagrantNovaItem.RechargeState.orig_FixedUpdate orig, RechargeState self)
        {
            RechargeState.baseDuration = cooldown; // make it actually 30s instead of 15s
            orig(self);
        }

        private void ChangeThreshold(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<HealthComponent>("get_isHealthLow")
            ))
            {
                c.Index += 1;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<bool, ReadyState, bool>>((Check, self) =>
                {
                    if ((self.attachedHealthComponent.health + self.attachedHealthComponent.shield) / self.attachedHealthComponent.fullCombinedHealth <= healthThreshold)
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
                Logger.LogError("Failed to apply Genesis Loop Threshold hook");
            }
        }

        public static void Changes(On.EntityStates.VagrantNovaItem.DetonateState.orig_OnEnter orig, DetonateState self)
        {
            DetonateState.blastDamageCoefficient = baseDamage;
            orig(self);
        }
    }
}