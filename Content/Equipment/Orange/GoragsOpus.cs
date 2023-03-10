using MonoMod.Cil;

namespace WellRoundedBalance.Equipment.Orange
{
    public class GoragsOpus : EquipmentBase
    {
        public override string Name => "::: Equipment :: Gorags Opus";
        public override string InternalPickupToken => "teamWarCry";

        public override string PickupText => "You and all your allies enter a frenzy.";

        public override string DescText => "All allies enter a <style=cIsDamage>frenzy</style> for <style=cIsUtility>" + buffDuration + "</style> seconds. Increases <style=cIsUtility>movement speed</style> by <style=cIsUtility>50%</style> and <style=cIsDamage>attack speed</style> by <style=cIsDamage>100%</style>.";

        [ConfigField("Buff Duration", "", 7f)]
        public static float buffDuration;

        [ConfigField("Cooldown", "", 45f)]
        public static float cooldown;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.EquipmentSlot.FireTeamWarCry += Changes;

            var Gorags = Utils.Paths.EquipmentDef.TeamWarCry.Load<EquipmentDef>();
            Gorags.cooldown = cooldown;
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(7f)))
            {
                c.Next.Operand = buffDuration;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Gorag's Opus Duration hook");
            }
        }
    }
}