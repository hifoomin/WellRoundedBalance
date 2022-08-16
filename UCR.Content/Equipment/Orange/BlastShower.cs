using MonoMod.Cil;

namespace UltimateCustomRun.Equipment
{
    public class BlastShower : EquipmentBase
    {
        public override string Name => "::: Equipment :: Blast Shower";
        public override string InternalPickupToken => "cleanse";

        public override bool NewPickup => false;

        public override bool NewDesc => false;

        public override string PickupText => "";

        public override string DescText => "";

        public static float RemovalRange;

        public override void Init()
        {
            RemovalRange = ConfigOption(6f, "Projectile Removal Radius", "Vanilla is 6");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.Util.CleanseBody += ChangeRemovalRange;
        }

        private void ChangeRemovalRange(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(6f)))
            {
                c.Next.Operand = RemovalRange;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Blast Shower Projectile Removal hook");
            }
        }
    }
}