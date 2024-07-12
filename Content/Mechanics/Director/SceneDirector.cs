using System;
using UnityEngine.SceneManagement;

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
            On.RoR2.ClassicStageInfo.Start += ClassicStageInfo_Start;
            RoR2.SceneDirector.onPrePopulateMonstersSceneServer += SceneDirector_onPrePopulateMonstersSceneServer;
            // On.RoR2.CombatDirector.SpendAllCreditsOnMapSpawns += CombatDirector_SpendAllCreditsOnMapSpawns;
        }

        private void ClassicStageInfo_Start(On.RoR2.ClassicStageInfo.orig_Start orig, ClassicStageInfo self)
        {
            var sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == "goldshores")
            {
                self.sceneDirectorMonsterCredits = 60;
            }
            orig(self);
        }

        /*
        private void CombatDirector_SpendAllCreditsOnMapSpawns(On.RoR2.CombatDirector.orig_SpendAllCreditsOnMapSpawns orig, RoR2.CombatDirector self, Transform mapSpawnTarget)
        {
            if (self.customName == "gex")
            {
                int num = 0;
                int num2 = 10;
                while (self.monsterCredit > 0f)
                {
                    self.PrepareNewMonsterWave(this.finalMonsterCardsSelection.Evaluate(self.rng.nextNormalizedFloat));
                    // ok so "just" make a new selection that excludes bosses ..
                    bool flag;
                    if (mapSpawnTarget)
                    {
                        flag = self.AttemptSpawnOnTarget(mapSpawnTarget, DirectorPlacementRule.PlacementMode.Approximate);
                    }
                    else
                    {
                        flag = self.AttemptSpawnOnTarget(null, SceneInfo.instance.approximateMapBoundMesh ? DirectorPlacementRule.PlacementMode.RandomNormalized : DirectorPlacementRule.PlacementMode.Random);
                    }
                    if (flag)
                    {
                        num = 0;
                    }
                    else
                    {
                        num++;
                        if (num >= num2)
                        {
                            break;
                        }
                    }
                }
            }
            else
            orig(self, mapSpawnTarget);
        }
        */

        private void SceneDirector_onPrePopulateMonstersSceneServer(RoR2.SceneDirector sd)
        {
            var stageInLoop = Run.instance.stageClearCount % Run.stagesPerLoop;
            var combatDirector = sd.GetComponent<RoR2.CombatDirector>();
            if (combatDirector)
            {
                combatDirector.customName = "gex";
            }
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