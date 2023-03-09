namespace WellRoundedBalance.Difficulties
{
    internal class Rainstorm : DifficultyBase
    {
        public override string Name => ":: Difficulties :: Rainstorm";
        public override string InternalDiffToken => "difficulty_normal_description";

        public override string DescText => "The way the game is meant to be played. Test your abilities and skills against formidable foes." +
                                           (totalDifficultyScaling != 100f ? "<style=cStack>\n\n>Difficulty Scaling: <style=cIsHealth>+" + (totalDifficultyScaling - 100f) + "%</style></style>" : "");

        [ConfigField("Total Difficulty Scaling", "", 100f)]
        public static float totalDifficultyScaling;

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
            for (int i = 0; i < DifficultyCatalog.difficultyDefs.Length; i++)
            {
                var difficulty = DifficultyCatalog.difficultyDefs[i];
                if (difficulty.nameToken == "DIFFICULTY_NORMAL_NAME")
                {
                    difficulty.scalingValue = totalDifficultyScaling / 50f;
                }
            }
        }
    }
}