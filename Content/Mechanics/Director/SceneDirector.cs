using System;

namespace WellRoundedBalance.Mechanics.Director
{
    internal class SceneDirector : MechanicBase<SceneDirector>
    {
        public override string Name => ":: Mechanics :::: Scene Director";

        [ConfigField("Pre-Spawn Gold and Exp Reward Multiplier", "", 3f)]
        public static float prespawnGoldAndExpRewardMultiplier;

        [ConfigField("Pre-Spawn Elite Bias", "", 6f)]
        public static float prespawnEliteBias;

        [ConfigField("Pre-Spawn Credit Multiplier", "", 2.5f)]
        public static float prespawnCreditMultiplier;

        [ConfigField("Pre-Spawn Credit Subract Per Stage", "Reduces Pre-spawn Credit Multiplier by this amount every stage per loop (Stage 1 => Pre-Spawn Credit Multiplier, Stage 5 => Pre-Spawn Credit Multiplier - (Pre-Spawn Credit Subract Per Stage * 5), Stage 6 => Pre-Spawn Credit Multiplier", 0.15f)]
        public static float prespawnCreditSubtractPerStage;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            RoR2.SceneDirector.onPrePopulateMonstersSceneServer += SceneDirector_onPrePopulateMonstersSceneServer;
        }

        private void SceneDirector_onPrePopulateMonstersSceneServer(RoR2.SceneDirector sd)
        {
            var stageInLoop = Run.instance.stageClearCount % Run.stagesPerLoop;
            switch (stageInLoop)
            {
                case 0:
                    // Main.WRBLogger.LogError("monster credit pre is " + sd.monsterCredit);
                    sd.monsterCredit = Convert.ToInt32(sd.monsterCredit * prespawnCreditMultiplier);
                    // Main.WRBLogger.LogError("monster credit post is " + sd.monsterCredit);
                    break;

                case 1:
                    // Main.WRBLogger.LogError("monster credit pre is " + sd.monsterCredit);
                    sd.monsterCredit = Convert.ToInt32(sd.monsterCredit * (prespawnCreditMultiplier - prespawnCreditSubtractPerStage));
                    // Main.WRBLogger.LogError("monster credit post is " + sd.monsterCredit);
                    break;

                case 2:
                    // Main.WRBLogger.LogError("monster credit pre is " + sd.monsterCredit);
                    sd.monsterCredit = Convert.ToInt32(sd.monsterCredit * (prespawnCreditMultiplier - (prespawnCreditSubtractPerStage * 2)));
                    // Main.WRBLogger.LogError("monster credit post is " + sd.monsterCredit);
                    break;

                case 3:
                    // Main.WRBLogger.LogError("monster credit pre is " + sd.monsterCredit);
                    sd.monsterCredit = Convert.ToInt32(sd.monsterCredit * (prespawnCreditMultiplier - (prespawnCreditSubtractPerStage * 3)));
                    // Main.WRBLogger.LogError("monster credit post is " + sd.monsterCredit);
                    break;

                case 4:
                    // Main.WRBLogger.LogError("monster credit pre is " + sd.monsterCredit);
                    sd.monsterCredit = Convert.ToInt32(sd.monsterCredit * (prespawnCreditMultiplier - (prespawnCreditSubtractPerStage * 4)));
                    // Main.WRBLogger.LogError("monster credit post is " + sd.monsterCredit);
                    break;

                default:
                    sd.monsterCredit = Convert.ToInt32(sd.monsterCredit * prespawnCreditMultiplier);
                    Main.WRBLogger.LogError("SceneDirector Error. This message should not appear!");
                    break;
            }

            sd.expRewardCoefficient *= prespawnGoldAndExpRewardMultiplier; // holy fuck why is it so low (literally like 0.0667)

            sd.eliteBias = prespawnEliteBias; // up from 2
        }
    }
}