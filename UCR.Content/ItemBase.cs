using R2API;
using RoR2;

namespace UltimateCustomRun
{
    public abstract class ItemBase
    {
        public abstract string Name { get; }
        public abstract bool NewPickup { get; }
        public virtual string InternalPickupToken { get; }
        public abstract string PickupText { get; }
        public abstract string DescText { get; }
        public virtual bool isEnabled { get; } = true;

        public T ConfigOption<T>(T value, string name, string description)
        {
            return Main.UCRConfig.Bind<T>(Name, name, value, description).Value;
        }

        public abstract void Hooks();

        public string d(float f)
        {
            return (f * 100f).ToString() + "%";
        }

        public virtual void Init()
        {
            Hooks();
            string pickupToken = "ITEM_" + InternalPickupToken.ToUpper() + "_PICKUP";
            string descriptionToken = "ITEM_" + InternalPickupToken.ToUpper() + "_DESC";
            if (NewPickup)
            {
                LanguageAPI.Add(pickupToken, PickupText);
            }
            LanguageAPI.Add(descriptionToken, DescText);
            Main.UCRLogger.LogInfo("Added " + Name);
        }
    }
}
