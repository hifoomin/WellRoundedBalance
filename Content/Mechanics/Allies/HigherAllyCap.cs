namespace WellRoundedBalance.Mechanics.Allies
{
    internal class HigherAllyCap : MechanicBase<HigherAllyCap>
    {
        public override string Name => ":: Mechanics ::::::::::::::: Higher Ally Cap";

        [ConfigField("Cap", "", 100)]
        public static int cap;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        private void Changes()
        {
            TeamDef def = TeamCatalog.teamDefs.FirstOrDefault(x => TeamCatalog.GetTeamDef(TeamIndex.Player) == x);
            def.softCharacterLimit = cap;
        }
    }
}