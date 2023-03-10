using MonoMod.Cil;

namespace WellRoundedBalance.Equipment.Orange
{
    public class EccentricVase : EquipmentBase
    {
        public override string Name => "::: Equipment :: Eccentric Vase";
        public override string InternalPickupToken => "gateway";

        public override string PickupText => "Create a quantum tunnel between two locations.";

        public override string DescText => "Create a <style=cIsUtility>quantum tunnel</style> of up to <style=cIsUtility>" + maxDistance + "m</style> in length. Lasts " + duration + " seconds.";

        [ConfigField("Cooldown", "", 20f)]
        public static float cooldown;

        [ConfigField("Max Distance", "", 1000f)]
        public static float maxDistance;

        [ConfigField("Duration", "", 15f)]
        public static float duration;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.EquipmentSlot.FireGateway += Changes;

            var Vase = Utils.Paths.EquipmentDef.Gateway.Load<EquipmentDef>();
            Vase.cooldown = cooldown;
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(1000f)))
            {
                c.Next.Operand = maxDistance;
            }
            else
            {
                Logger.LogError("Failed to apply Eccentric Vase Distance hook");
            }

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(30f)))
            {
                c.Next.Operand = duration;
            }
            else
            {
                Logger.LogError("Failed to apply Eccentric Vase Duration hook");
            }
        }
    }
}