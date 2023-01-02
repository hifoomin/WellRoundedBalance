using System;

namespace WellRoundedBalance.Utils {
    public static class Extensions {
        public static PickupIndex GetPickupIndex(this ItemDef def) {
            return PickupCatalog.FindPickupIndex(def.itemIndex);
        }
    }
}