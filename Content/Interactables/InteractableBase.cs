using BepInEx.Configuration;

namespace WellRoundedBalance.Interactables
{
    public abstract class InteractableBase : SharedBase
    {
        public override ConfigFile Config => Main.WRBInteractableConfig;
        public static List<string> interactableList = new();

        public override void Init()
        {
            base.Init();
            interactableList.Add(Name);
        }
    }
}