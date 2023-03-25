namespace WellRoundedBalance.Enemies.Bosses
{
    internal class Grandparent : EnemyBase
    {
        public override string Name => ":: Enemies ::::::::: Grandparent";

        public override void Hooks()
        {
            On.RoR2.GrandParentSunController.Start += GrandParentSunController_Start;
        }

        private void GrandParentSunController_Start(On.RoR2.GrandParentSunController.orig_Start orig, GrandParentSunController self)
        {
            self.bullseyeSearch.teamMaskFilter.RemoveTeam(self.teamFilter.teamIndex);
            orig(self);
        }
    }
}