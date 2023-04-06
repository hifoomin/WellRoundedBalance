using R2API.Utils;
using System;

namespace WellRoundedBalance.Gamemodes.Simulacrum
{
    internal class BiggerCrab : GamemodeBase<BiggerCrab>
    {
        public override string Name => ":: Gamemode :: Simulacrum Bigger Crab";

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
            var sizeMult = 2.5f;
            var thing1 = Utils.Paths.GameObject.InfiniteTowerSafeWardAwaitingInteraction.Load<GameObject>();
            var model1 = thing1.transform.GetChild(0);
            model1.transform.localScale = new Vector3(sizeMult, sizeMult, sizeMult);

            var positions1 = thing1.transform.GetChild(2);
            var reward1 = positions1.GetChild(0);
            reward1.position = new Vector3(0, 4.625f, 0); // 1.85 * 2.5
            var monster1 = positions1.GetChild(1);
            monster1.position = new Vector3(0, 4.625f, 0);

            var thing2 = Utils.Paths.GameObject.InfiniteTowerSafeWard.Load<GameObject>();
            var model2 = thing2.transform.GetChild(0);
            model2.transform.localScale = new Vector3(sizeMult, sizeMult, sizeMult);

            var positions2 = thing2.transform.GetChild(2);
            var reward2 = positions2.GetChild(0);
            reward2.position = new Vector3(0, 4.625f, 0); // 1.85 * 2.5
            var monster2 = positions2.GetChild(1);
            monster2.position = new Vector3(0, 4.625f, 0);
        }
    }
}