using MonoMod.Cil;

namespace UltimateCustomRun.Equipment
{
    public class GoragsOpus : EquipmentBase
    {
        public override string Name => "::: Equipment :: Gorags Opus";
        public override string InternalPickupToken => "teamWarCry";

        public override bool NewPickup => false;

        public override bool NewDesc => true;

        public override string PickupText => "";

        public override string DescText => "All allies enter a <style=cIsDamage>frenzy</style> for <style=cIsUtility>" + Duration + "</style> seconds. Increases <style=cIsUtility>movement speed</style> by <style=cIsUtility>50%</style> and <style=cIsDamage>attack speed</style> by <style=cIsDamage>100%</style>.";

        public static float Duration;

        public override void Init()
        {
            Duration = ConfigOption(7f, "Duration", "Vanilla is 7");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.EquipmentSlot.FireTeamWarCry += Changes;
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(7f)))
            {
                c.Next.Operand = Duration;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Gorag's Opus Duration hook");
            }
        }
    }
}