namespace WellRoundedBalance.Equipment
{
    public abstract class EquipmentBase
    {
        public abstract string Name { get; }
        public virtual string InternalPickupToken { get; }
        public abstract string PickupText { get; }
        public abstract string DescText { get; }
        public virtual bool isEnabled { get; } = true;

        public string d(float f)
        {
            return (f * 100f).ToString() + "%";
        }

        public abstract void Hooks();

        public virtual void Init()
        {
            ConfigManager.HandleConfigAttributes(this.GetType(), Name, Main.WRBEquipmentConfig);
            Hooks();
            string pickupToken = "EQUIPMENT_" + InternalPickupToken.ToUpper() + "_PICKUP";
            string descriptionToken = "EQUIPMENT_" + InternalPickupToken.ToUpper() + "_DESC";
            LanguageAPI.Add(pickupToken, PickupText);
            LanguageAPI.Add(descriptionToken, DescText);
        }
    }
}