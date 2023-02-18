namespace WellRoundedBalance.Mechanics.RemoveGesture
{
    public static class RemoveGesture
    {
        public static void Based()
        {
            var gesture = Utils.Paths.ItemDef.AutoCastEquipment.Load<ItemDef>();
            gesture.tier = ItemTier.NoTier;
            gesture.deprecatedTier = ItemTier.NoTier;
        }
    }
}