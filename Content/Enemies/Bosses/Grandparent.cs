namespace WellRoundedBalance.Enemies.Bosses
{
    internal class Grandparent : EnemyBase<Grandparent>
    {
        public override string Name => "::: Bosses :: Grandparent";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.GrandParentSunController.Start += GrandParentSunController_Start;
            RoR2.CharacterMaster.onStartGlobal += CharacterMaster_onStartGlobal;
            Changes();
        }

        private void GrandParentSunController_Start(On.RoR2.GrandParentSunController.orig_Start orig, GrandParentSunController self)
        {
            self.bullseyeSearch.teamMaskFilter.RemoveTeam(self.teamFilter.teamIndex);
            orig(self);
        }

        private void CharacterMaster_onStartGlobal(CharacterMaster master)
        {
            if (Main.IsInfernoDef())
            {
                return;
            }
            switch (master.name)
            {
                case "GrandparentMaster(Clone)":
                    AISkillDriver Sun = (from x in master.GetComponents<AISkillDriver>()
                                         where x.customName == "ChannelSun"
                                         select x).First();
                    Sun.noRepeat = true;
                    break;
            }
        }

        private void Changes()
        {
            var sun = Utils.Paths.GameObject.GrandParentSun.Load<GameObject>();
            var sunController = sun.GetComponent<GrandParentSunController>();
            sunController.burnDuration = 0.4f;
        }
    }
}