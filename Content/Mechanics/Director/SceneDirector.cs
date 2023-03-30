using System;

namespace WellRoundedBalance.Mechanics.Director
{
    internal class SceneDirector : MechanicBase
    {
        public override string Name => ":: Mechanics :::: Scene Director";

        [ConfigField("Pre-Spawn Gold and Exp Reward Multiplier", "", 3f)]
        public static float prespawnGoldAndExpRewardMultiplier;

        [ConfigField("Pre-Spawn Elite Biass", "", 6f)]
        public static float prespawnEliteBias;

        [ConfigField("Pre-Spawn Credit Multiplier", "", 2.5f)]
        public static float prespawnCreditMultiplier;

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
            sd.expRewardCoefficient *= prespawnGoldAndExpRewardMultiplier; // holy fuck why is it so low (literally like 0.0667)
            sd.monsterCredit = Convert.ToInt32(sd.monsterCredit * prespawnCreditMultiplier);
            sd.eliteBias = prespawnEliteBias; // up from 2
        }
    }
}