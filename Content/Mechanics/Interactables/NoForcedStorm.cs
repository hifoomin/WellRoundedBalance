using System;

namespace WellRoundedBalance.Mechanics.Interactables
{
    internal class NoForcedStorm : MechanicBase<NoForcedStorm>
    {
        public override string Name => ":: Mechanics :::::: No Forced SOTS Drop Pools";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.PickupPickerController.GenerateOptionsFromDropTablePlusForcedStorm += DisableForcedStorm;
        }

        private RoR2.PickupPickerController.Option[] DisableForcedStorm(On.RoR2.PickupPickerController.orig_GenerateOptionsFromDropTablePlusForcedStorm orig, int numOptions, RoR2.PickupDropTable dropTable, RoR2.PickupDropTable stormDropTable, Xoroshiro128Plus rng)
        {
            List<UniquePickup> list = new List<UniquePickup>();
            List<PickupPickerController.Option> options = new();
            dropTable.GenerateDistinctPickups(list, numOptions, rng);

            foreach (UniquePickup pickup in list) {
                options.Add(new() {
                    available = true,
                    pickup = pickup
                });
            }

            PickupPickerController.GrabNumbers = numOptions - 2;

            return options.ToArray();
        }
    }
}