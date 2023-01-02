using RoR2;
using UnityEngine.AddressableAssets;

namespace WellRoundedBalance.Interactable
{
    public class ScavengerBag : InteractableBase
    {
        public override string Name => "Interactables :::::::: Scavenger Bag";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.ScavBackpack.Opening.OnEnter += Opening_OnEnter;
        }

        private void Opening_OnEnter(On.EntityStates.ScavBackpack.Opening.orig_OnEnter orig, EntityStates.ScavBackpack.Opening self)
        {
            EntityStates.ScavBackpack.Opening.maxItemDropCount = 3;
            orig(self);
        }
    }
}