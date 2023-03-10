using BepInEx.Configuration;

namespace WellRoundedBalance.Interactables
{
    public abstract class InteractableBase : SharedBase
    {
        public override ConfigFile Config => Main.WRBInteractableConfig;
    }
}