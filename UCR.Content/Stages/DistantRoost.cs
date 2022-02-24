namespace UltimateCustomRun.Stages
{
    public static class DistantRoost
    {
        public static void AddCredits()
        {
            On.RoR2.SceneDirector.Start += (orig, self) =>
            {
                var stage = RoR2.SceneCatalog.GetSceneDefForCurrentScene();
                if (stage && stage.baseSceneName.Equals("blackbeach"))
                {
                    RoR2.ClassicStageInfo dir = RoR2.SceneInfo.instance.GetComponent<RoR2.ClassicStageInfo>();
                    if (dir)
                    {
                        dir.sceneDirectorInteractibleCredits = 220;
                    }
                }
                orig(self);
            };
        }
    }
}