using BepInEx.Configuration;
using System;

namespace WellRoundedBalance.Equipment
{
    public abstract class EquipmentBase : SharedBase
    {
        public virtual EquipmentDef InternalPickup { get; }
        public abstract string PickupText { get; }
        public abstract string DescText { get; }
        public override ConfigFile Config => Main.WRBEquipmentConfig;
        public static event Action onTokenRegister;

        public override void Init()
        {
            base.Init();
            onTokenRegister += SetToken;
        }

        [SystemInitializer(typeof(EquipmentCatalog))]
        public static void OnEquipmentInitialized() { if (onTokenRegister != null) onTokenRegister(); }

        public void SetToken()
        {
            if (InternalPickup != null)
            {
                LanguageAPI.Add(InternalPickup.pickupToken, PickupText);
                LanguageAPI.Add(InternalPickup.descriptionToken, DescText);
            };
        }
    }
}