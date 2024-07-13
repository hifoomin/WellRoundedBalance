using R2API.Utils;

namespace WellRoundedBalance.Gamemodes.Simulacrum
{
    internal class LessInteractables : GamemodeBase<LessInteractables>
    {
        public override string Name => ":: Gamemode :: Simulacrum Less Interactables";

        [ConfigField("Scene Director Credits", "", 350)]
        public static int sceneDirectorCredits;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.InfiniteTowerRun.OnPrePopulateSceneServer += InfiniteTowerRun_OnPrePopulateSceneServer;
        }

        private void InfiniteTowerRun_OnPrePopulateSceneServer(On.RoR2.InfiniteTowerRun.orig_OnPrePopulateSceneServer orig, InfiniteTowerRun self, SceneDirector sceneDirector)
        {
            self.interactableCredits = sceneDirectorCredits;
            orig(self, sceneDirector);
        }
    }
}