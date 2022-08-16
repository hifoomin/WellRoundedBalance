using MonoMod.Cil;

namespace UltimateCustomRun.Equipment
{
    public class FuelArray : EquipmentBase
    {
        public override string Name => "::: Equipment :: Fuel Array";
        public override string InternalPickupToken => "questVolatileBattery";

        public override bool NewPickup => false;

        public override bool NewDesc => false;

        public override string PickupText => "";

        public override string DescText => "";

        public static float Damage;
        public static float ProcCoefficient;
        public static float Radius;
        public static float Threshold;
        public static float Duration;

        public override void Init()
        {
            Damage = ConfigOption(3f, "Percent Health Damage", "Decimal. Vanilla is 3");
            ProcCoefficient = ConfigOption(0f, "Proc Coefficient", "Vanilla is 0");
            Radius = ConfigOption(30f, "Explosion Radius", "Vanilla is 30");
            Threshold = ConfigOption(0.5f, "Health Threshold", "Decimal. Vanilla is 0.5");
            Duration = ConfigOption(3f, "Explosion Countdown", "Vanila is 3");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.QuestVolatileBattery.Monitor.FixedUpdate += ChangeThreshold;
            On.EntityStates.QuestVolatileBattery.CountDown.OnEnter += ChangeRadius;
            IL.EntityStates.QuestVolatileBattery.CountDown.Detonate += Changes;
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(3f)))
            {
                c.Next.Operand = Damage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Fuel Array Damage hook");
            }

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchDup(),
                x => x.MatchLdcR4(0f)))
            {
                c.Index += 1;
                c.Next.Operand = ProcCoefficient;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Fuel Array Proc Coefficient hook");
            }
        }

        private void ChangeRadius(On.EntityStates.QuestVolatileBattery.CountDown.orig_OnEnter orig, EntityStates.QuestVolatileBattery.CountDown self)
        {
            EntityStates.QuestVolatileBattery.CountDown.explosionRadius = Radius;
            EntityStates.QuestVolatileBattery.CountDown.duration = Duration;
            orig(self);
        }

        private void ChangeThreshold(On.EntityStates.QuestVolatileBattery.Monitor.orig_FixedUpdate orig, EntityStates.QuestVolatileBattery.Monitor self)
        {
            EntityStates.QuestVolatileBattery.Monitor.healthFractionDetonationThreshold = Threshold;
            orig(self);
        }
    }
}