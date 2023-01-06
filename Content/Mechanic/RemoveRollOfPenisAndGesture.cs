namespace WellRoundedBalance.Global
{
    public static class RemoveRollOfPenisAndGesture
    {
        public static void Based()
        {
            var penis = Utils.Paths.ItemDef.GoldOnHurt.Load<ItemDef>();
            penis.tier = ItemTier.NoTier;
            penis.deprecatedTier = ItemTier.NoTier;

            var gesture = Utils.Paths.ItemDef.AutoCastEquipment.Load<ItemDef>();
            gesture.tier = ItemTier.NoTier;
            gesture.deprecatedTier = ItemTier.NoTier;
        }
    }
}