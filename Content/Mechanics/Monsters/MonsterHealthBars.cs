namespace WellRoundedBalance.Mechanics.Monsters
{
    internal class MonsterHealthBars : MechanicBase<MonsterHealthBars>
    {
        public override string Name => ":: Mechanics ::::::::: Monster Health Bars";

        [ConfigField("Health Bar Show Duration", "", float.MaxValue)]
        public static float showDuration;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.UI.CombatHealthBarViewer.Awake += CombatHealthBarViewer_Awake;
        }

        private void CombatHealthBarViewer_Awake(On.RoR2.UI.CombatHealthBarViewer.orig_Awake orig, RoR2.UI.CombatHealthBarViewer self)
        {
            orig(self);
            self.healthBarDuration = showDuration;
        }
    }
}