using HG;
using MonoMod.Cil;

namespace WellRoundedBalance.Items.Reds
{
    internal class InterstellarDeskPlant : ItemBase
    {
        public override string Name => ":: Items ::: Reds :: Interstellar Desk Plant";

        public override string InternalPickupToken => "interstellarDeskPlant";

        public override string PickupText => "Plant a healing fruit on kill.";

        public override string DescText => "On kill, plant a <style=cIsHealing>healing</style> fruit seed that grows into a plant after <style=cIsUtility>5</style> seconds. \n\nThe plant <style=cIsHealing>heals</style> for <style=cIsHealing>7%</style> of <style=cIsHealing>maximum health</style> every second to all allies within <style=cIsHealing>12m</style> <style=cStack>(+3m per stack)</style>. Lasts <style=cIsUtility>10</style> seconds.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.DeskPlantController.Awake += DeskPlantController_Awake;
            IL.RoR2.DeskPlantController.MainState.OnEnter += MainState_OnEnter;
        }

        private void MainState_OnEnter(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.05f)))
            {
                c.Next.Operand = 0.035f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Interstellar Desk Plant Healing hook");
            }
        }

        private void DeskPlantController_Awake(On.RoR2.DeskPlantController.orig_Awake orig, DeskPlantController self)
        {
            self.healingRadius = 12f;
            self.radiusIncreasePerStack = 3f;
            orig(self);
        }
    }
}