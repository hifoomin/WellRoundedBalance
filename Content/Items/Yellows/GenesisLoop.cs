using EntityStates.VagrantNovaItem;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Yellows
{
    public class GenesisLoop : ItemBase
    {
        public override string Name => ":: Items :::: Yellows :: Genesis Loop";
        public override string InternalPickupToken => "novaOnLowHealth";

        public override string PickupText => "Fire an electric nova at low health.";

        public override string DescText => "Falling below <style=cIsHealth>50% health</style> causes you to explode, dealing <style=cIsDamage>5000% base damage</style>. Recharges every <style=cIsUtility>30 seconds</style> <style=cStack>(-33% per stack)</style>.";

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
            RechargeState.baseDuration = 60f; // make it actually 30s instead of 15s
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
                    if ((self.attachedHealthComponent.health + self.attachedHealthComponent.shield) / self.attachedHealthComponent.fullCombinedHealth <= 0.5f)
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
                Main.WRBLogger.LogError("Failed to apply Genesis Loop Threshold hook");
            }

            // This is very janky, seems to not work?
        }

        public static void Changes(On.EntityStates.VagrantNovaItem.DetonateState.orig_OnEnter orig, DetonateState self)
        {
            DetonateState.blastDamageCoefficient = 50f;
            orig(self);
        }
    }
}