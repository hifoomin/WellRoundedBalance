using BepInEx.Configuration;

namespace WellRoundedBalance.Equipment
{
    public abstract class EquipmentBase : SharedBase
    {
        public virtual string InternalPickupToken { get; }
        public abstract string PickupText { get; }
        public abstract string DescText { get; }
        public override ConfigFile Config => Main.WRBEquipmentConfig;
    }
}