using System;

namespace WellRoundedBalance.Mechanics.Director
{
    internal class SceneDirector : MechanicBase
    {
        public override string Name => ":: Mechanics :::: Scene Director";

        public override void Hooks()
        {
            RoR2.SceneDirector.onPrePopulateMonstersSceneServer += SceneDirector_onPrePopulateMonstersSceneServer;
        }

        private void SceneDirector_onPrePopulateMonstersSceneServer(RoR2.SceneDirector sd)
        {
            sd.expRewardCoefficient *= 3f; // holy fuck why is it so low (literally like 0.0667)
            sd.monsterCredit = Convert.ToInt32(sd.monsterCredit * 2.5f);
            sd.eliteBias = 0.8f; // down from 2
        }
    }
}