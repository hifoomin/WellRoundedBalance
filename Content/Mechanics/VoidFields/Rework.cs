using UnityEngine.SceneManagement;

namespace WellRoundedBalance.Mechanics.VoidFields
{
    internal class VoidFields : MechanicBase<VoidFields>
    {
        public override string Name => ":: Mechanics :::::::::::::::: Void Fields Rework";

        [ConfigField("Wave Count", "", 5)]
        public static int waveCount;

        [ConfigField("Charge Multiplier", "Affected by Faster Holdout Zone mechanic.", 0.55f)]
        public static float chargeMultiplier;

        [ConfigField("Scene Director Credits", "", 120)]
        public static int sceneDirectorCredits;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.ArenaMissionController.Awake += ArenaMissionController_Awake;
            On.RoR2.ArenaMissionController.BeginRound += ArenaMissionController_BeginRound;
            On.RoR2.ClassicStageInfo.Awake += ClassicStageInfo_Awake;
        }

        private void ClassicStageInfo_Awake(On.RoR2.ClassicStageInfo.orig_Awake orig, ClassicStageInfo self)
        {
            orig(self);
            if (SceneManager.GetActiveScene().name == "arena")
            {
                self.sceneDirectorInteractibleCredits = sceneDirectorCredits;
            }
        }

        private void ArenaMissionController_BeginRound(On.RoR2.ArenaMissionController.orig_BeginRound orig, ArenaMissionController self)
        {
            if (NetworkServer.active)
            {
                switch (self.currentRound + 1)
                {
                    case 1:
                        self.AddItemStack();
                        break;

                    case 2:
                        self.AddMonsterType();
                        break;

                    case 3:
                        self.AddItemStack();
                        break;

                    case 4:
                        self.AddMonsterType();
                        break;

                    case 5:
                        self.AddItemStack();
                        break;

                    case 6:
                        self.AddMonsterType();
                        break;

                    case 7:
                        self.AddMonsterType();
                        break;

                    case 8:
                        self.AddMonsterType();
                        break;
                }
            }

            orig(self);
        }

        private void ArenaMissionController_Awake(On.RoR2.ArenaMissionController.orig_Awake orig, ArenaMissionController self)
        {
            if (self.currentRound == waveCount)
                return;
            orig(self);
        }
    }
}