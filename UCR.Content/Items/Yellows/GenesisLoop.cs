using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using System;

namespace UltimateCustomRun.Items.Yellows
{
    public class GenesisLoop : ItemBase
    {
        public static float Damage;
        public static float ProcCoefficient;
        public static float Range;
        public static float RechargeTime;
        public static float StackRechargeTime;
        public static float ChargeTime;
        public static float Threshold;

        public override string Name => ":: Items :::: Yellows :: Genesis Loop";
        public override string InternalPickupToken => "novaOnLowHealth";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Falling below <style=cIsHealth>"/* + d(Threshold)*/ + "25% health</style> causes you to explode, dealing <style=cIsDamage>" + d(Damage) + " base damage</style>. Recharges every <style=cIsUtility>" + RechargeTime + " seconds</style> <style=cStack>(-" + d(StackRechargeTime) + " per stack)</style>.";

        public override void Init()
        {
            Damage = ConfigOption(60f, "Damage", "Decimal. Vanilla is 60");
            ProcCoefficient = ConfigOption(1f, "Proc Coefficient", "Vanilla is 1");
            Range = ConfigOption(100f, "Range", "Vanilla is 100");
            RechargeTime = ConfigOption(30f, "Recharge Time", "Vanilla is 30");
            StackRechargeTime = ConfigOption(0.5f, "Stack Recharge Time Reduction", "Decimal. Vanilla is 0.5");
            ChargeTime = ConfigOption(3f, "Charge Time", "Vanilla is 3");
            // Threshold = ConfigOption(0.25f, "Health Threshold", "Decimal. Vanilla is 0.25");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.VagrantNovaItem.DetonateState.OnEnter += Changes;
            On.EntityStates.VagrantNovaItem.RechargeState.FixedUpdate += ChangeRechargeTime;
            On.EntityStates.VagrantNovaItem.ChargeState.OnEnter += ChangeChargeTime;
            // IL.EntityStates.VagrantNovaItem.ReadyState.FixedUpdate += ChangeThreshold;
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
                c.EmitDelegate<Func<bool, HealthComponent, bool>>((Check, self) =>
                {
                    if ((self.health + self.shield) / self.fullCombinedHealth < Threshold)
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
                Main.UCRLogger.LogError("Failed to apply Genesis Loop Threshold hook");
            }

            // This is very janky, seems to work randomly
        }

        private void ChangeChargeTime(On.EntityStates.VagrantNovaItem.ChargeState.orig_OnEnter orig, EntityStates.VagrantNovaItem.ChargeState self)
        {
            EntityStates.VagrantNovaItem.ChargeState.baseDuration = ChargeTime;
            orig(self);
        }

        public static void ChangeRechargeTime(On.EntityStates.VagrantNovaItem.RechargeState.orig_FixedUpdate orig, EntityStates.VagrantNovaItem.RechargeState self)
        {
            EntityStates.VagrantNovaItem.RechargeState.baseDuration = RechargeTime;
            orig(self);
        }

        public static void Changes(On.EntityStates.VagrantNovaItem.DetonateState.orig_OnEnter orig, EntityStates.VagrantNovaItem.DetonateState self)
        {
            EntityStates.VagrantNovaItem.DetonateState.blastDamageCoefficient = Damage;
            EntityStates.VagrantNovaItem.DetonateState.blastProcCoefficient = ProcCoefficient;
            EntityStates.VagrantNovaItem.DetonateState.blastRadius = Range;
            orig(self);
        }
    }
}