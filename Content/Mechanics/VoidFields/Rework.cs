using UnityEngine.SceneManagement;

namespace WellRoundedBalance.Mechanics.VoidFields
{
    internal class VoidFields : MechanicBase<VoidFields>
    {
        public override string Name => ":: Mechanics :::::::::::::::: Void Fields Rework";

        [ConfigField("Charge Multiplier", "Affected by Faster Holdout Zone mechanic.", 0.55f)]
        public static float chargeMultiplier;

        [ConfigField("Scene Director Credits", "", 120)]
        public static int sceneDirectorCredits;

        public static PickupDropTable tier1;
        public static PickupDropTable tier2;
        public static PickupDropTable tier3;
        public static PickupDropTable tierVoid;

        public override void Init()
        {
            tier1 = Utils.Paths.BasicPickupDropTable.dtTier1Item.Load<BasicPickupDropTable>();
            tier2 = Utils.Paths.BasicPickupDropTable.dtTier2Item.Load<BasicPickupDropTable>();
            tier3 = Utils.Paths.BasicPickupDropTable.dtTier3Item.Load<BasicPickupDropTable>();
            tierVoid = Utils.Paths.BasicPickupDropTable.dtVoidChest.Load<BasicPickupDropTable>();
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.ArenaMissionController.AddMonsterType += ArenaMissionController_AddMonsterType;
            On.RoR2.ArenaMissionController.BeginRound += ArenaMissionController_BeginRound;
            On.RoR2.ClassicStageInfo.Awake += ClassicStageInfo_Awake;
            On.RoR2.ArenaMissionController.Awake += ArenaMissionController_Awake;
        }

        private void ArenaMissionController_Awake(On.RoR2.ArenaMissionController.orig_Awake orig, ArenaMissionController self)
        {
            orig(self);
            self.totalRoundsMax = 5;
            for (int i = 0; i < 5 && i < self.playerRewardOrder.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        self.playerRewardOrder[i] = tier1;
                        break;

                    case 1:
                        self.playerRewardOrder[i] = tier1;
                        break;

                    case 2:
                        self.playerRewardOrder[i] = tier2;
                        break;

                    case 3:
                        self.playerRewardOrder[i] = tierVoid;
                        break;

                    case 4:
                        self.playerRewardOrder[i] = tier3;
                        break;
                }
            }
        }

        private void ArenaMissionController_AddMonsterType(On.RoR2.ArenaMissionController.orig_AddMonsterType orig, ArenaMissionController self)
        {
            if (self.currentRound == 5)
                return;
            orig(self);
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
                }
            }

            orig(self);
        }
    }
}