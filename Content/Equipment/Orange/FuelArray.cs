using MonoMod.Cil;

namespace WellRoundedBalance.Equipment.Orange
{
    public class FuelArray : EquipmentBase<FuelArray>
    {
        public override string Name => ":: Equipment :: Fuel Array";

        public override EquipmentDef InternalPickup => RoR2Content.Equipment.QuestVolatileBattery;

        public override string PickupText => "Looks like it could power something. <color=#FF7F7F>EXTREMELY unstable...</color>";

        public override string DescText => "Looks like it could power something. <color=#FF7F7F>EXTREMELY unstable...</color>";

        [ConfigField("Health Threshold", "Decimal.", 0.35f)]
        public static float threshold;

        [ConfigField("Percent Health Damage", "Decimal.", 3f)]
        public static float damage;

        [ConfigField("Proc Coefficient", "", 0f)]
        public static float procCoefficient;

        [ConfigField("Explosion Countdown Timer", "", 3f)]
        public static float countdown;

        [ConfigField("Explosion Radius", "", 30f)]
        public static float radius;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.QuestVolatileBattery.Monitor.FixedUpdateServer += Monitor_FixedUpdateServer;
            On.EntityStates.QuestVolatileBattery.CountDown.OnEnter += CountDown_OnEnter;
            IL.EntityStates.QuestVolatileBattery.CountDown.Detonate += CountDown_Detonate;
        }

        private void CountDown_Detonate(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                 x => x.MatchLdcR4(3f)))
            {
                c.Next.Operand = damage;
            }
            else
            {
                Logger.LogError("Failed to apply Fuel Array Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                 x => x.MatchLdcR4(0f),
                 x => x.MatchStfld<BlastAttack>("procCoefficient")))
            {
                c.Next.Operand = procCoefficient;
            }
            else
            {
                Logger.LogError("Failed to apply Fuel Array Proc Coefficient hook");
            }
        }

        private void CountDown_OnEnter(On.EntityStates.QuestVolatileBattery.CountDown.orig_OnEnter orig, EntityStates.QuestVolatileBattery.CountDown self)
        {
            EntityStates.QuestVolatileBattery.CountDown.duration = countdown;
            EntityStates.QuestVolatileBattery.CountDown.explosionRadius = radius;
            orig(self);
        }

        private void Monitor_FixedUpdateServer(On.EntityStates.QuestVolatileBattery.Monitor.orig_FixedUpdateServer orig, EntityStates.QuestVolatileBattery.Monitor self)
        {
            EntityStates.QuestVolatileBattery.Monitor.healthFractionDetonationThreshold = threshold;
            orig(self);
        }
    }
}