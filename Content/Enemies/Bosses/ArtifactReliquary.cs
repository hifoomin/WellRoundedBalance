namespace WellRoundedBalance.Enemies.Bosses
{
    internal class ArtifactReliquary : EnemyBase<ArtifactReliquary>
    {
        public override string Name => "::: Bosses :: Artifact Reliquary";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.ArtifactShell.FireSolarFlares.OnEnter += FireSolarFlares_OnEnter;
            On.RoR2.ArtifactTrialMissionController.OnStartServer += ArtifactTrialMissionController_OnStartServer;
            Changes();
        }

        private void ArtifactTrialMissionController_OnStartServer(On.RoR2.ArtifactTrialMissionController.orig_OnStartServer orig, ArtifactTrialMissionController self)
        {
            orig(self);
            // vanilla is 0.03
            self.chanceForKeyDrop = 0.05f;
        }

        private void FireSolarFlares_OnEnter(On.EntityStates.ArtifactShell.FireSolarFlares.orig_OnEnter orig, EntityStates.ArtifactShell.FireSolarFlares self)
        {
            // EntityStates.ArtifactShell.FireSolarFlares.projectileSpeed = 25f;
            orig(self);
        }

        private void Changes()
        {
            var proj = Utils.Paths.GameObject.ArtifactShellSeekingSolarFlare.Load<GameObject>();
            proj.transform.localScale = new Vector3(2f, 2f, 2f);

            var ghost = Utils.Paths.GameObject.SolarFlareGhost.Load<GameObject>();
            ghost.transform.localScale = new Vector3(2f, 2f, 2f);
        }
    }
}