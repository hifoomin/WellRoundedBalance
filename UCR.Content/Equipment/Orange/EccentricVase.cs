using MonoMod.Cil;

namespace UltimateCustomRun.Equipment
{
    public class EccentricVase : EquipmentBase
    {
        public override string Name => "::: Equipment :: Eccentric Vase";
        public override string InternalPickupToken => "gateway";

        public override bool NewPickup => false;

        public override bool NewDesc => true;

        public override string PickupText => "";

        public override string DescText => "Create a <style=cIsUtility>quantum tunnel</style> of up to <style=cIsUtility>" + MaxDistance + "m</style> in length. Lasts " + Duration + " seconds.";

        public static float MaxDistance;
        public static float Duration;

        public override void Init()
        {
            MaxDistance = ConfigOption(1000f, "Max Distance", "Vanilla is 1000");
            Duration = ConfigOption(30f, "Duration", "Vanilla is 30");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.EquipmentSlot.FireGateway += Changes;
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(1000f)))
            {
                c.Next.Operand = MaxDistance;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Eccentric Vase Distance hook");
            }

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(30f)))
            {
                c.Next.Operand = Duration;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Eccentric Vase Duration hook");
            }
        }
    }
}