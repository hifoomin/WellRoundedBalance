using System;

namespace WellRoundedBalance.Mechanic.Director
{
    internal class SceneDirector : GlobalBase
    {
        public override string Name => ":: Mechanic :::::: Scene Director";

        public override void Hooks()
        {
            RoR2.SceneDirector.onPrePopulateMonstersSceneServer += SceneDirector_onPrePopulateMonstersSceneServer;
        }

        private void SceneDirector_onPrePopulateMonstersSceneServer(RoR2.SceneDirector sd)
        {
            sd.expRewardCoefficient *= 2.5f; // holy fuck why is it so low (literally like 0.0667)
            sd.monsterCredit = Convert.ToInt32(sd.monsterCredit * 2.5f);
            sd.eliteBias = 1f; // down from 2
        }
    }
}