using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun.Items.Yellows
{
    public class GenesisLoop : ItemBase
    {
        public static float Damage;
        public static float ProcCoefficient;
        public static float Range;
        public static float RechargeTime;

        public override string Name => ":: Items :::: Yellows :: Genesis Loop";
        public override string InternalPickupToken => "novaOnLowHealth";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Falling below <style=cIsHealth>25% health</style> causes you to explode, dealing <style=cIsDamage>6000% base damage</style>. Recharges every <style=cIsUtility>30 seconds</style> <style=cStack>(-50% per stack)</style>.";

        public override void Init()
        {
            Damage = ConfigOption(60f, "Damage", "Decimal. Vanilla is 60");
            ProcCoefficient = ConfigOption(1f, "Proc Coefficient", "Vanilla is 1");
            Range = ConfigOption(100, "Range", "Vanilla is 100");
            RechargeTime = ConfigOption(30f, "Recharge Time", "Vanilla is 30");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.VagrantNovaItem.DetonateState.OnEnter += Changes;
            On.EntityStates.VagrantNovaItem.RechargeState.FixedUpdate += ChangeRechargeTime;
        }

        private void ChangeRechargeTime(On.EntityStates.VagrantNovaItem.RechargeState.orig_FixedUpdate orig, EntityStates.VagrantNovaItem.RechargeState self)
        {
            EntityStates.VagrantNovaItem.RechargeState.baseDuration = RechargeTime;
            orig(self);
        }

        private void Changes(On.EntityStates.VagrantNovaItem.DetonateState.orig_OnEnter orig, EntityStates.VagrantNovaItem.DetonateState self)
        {
            EntityStates.VagrantNovaItem.DetonateState.blastDamageCoefficient = Damage;
            EntityStates.VagrantNovaItem.DetonateState.blastProcCoefficient = ProcCoefficient;
            EntityStates.VagrantNovaItem.DetonateState.blastRadius = Range;
            orig(self);
        }
    }
}