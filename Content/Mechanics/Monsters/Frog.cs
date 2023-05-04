namespace WellRoundedBalance.Mechanics.Monsters
{
    internal class Frog : MechanicBase<Frog>
    {
        public override string Name => ":: Mechanics ::::::::: Frog";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.FrogController.Pet += FrogController_Pet;
        }

        private void FrogController_Pet(On.RoR2.FrogController.orig_Pet orig, FrogController self, Interactor interactor)
        {
            self.maxPets = 1;
            orig(self, interactor);
        }
    }
}