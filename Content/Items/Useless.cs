namespace WellRoundedBalance.Items
{
    public class Useless
    {
        public static ItemDef uselessItem;

        public static void Create()
        {
            uselessItem = ScriptableObject.CreateInstance<ItemDef>();
            uselessItem.tier = ItemTier.NoTier;
            uselessItem.hidden = true;
            uselessItem.deprecatedTier = ItemTier.NoTier;
            uselessItem.name = "Useless Item";
            ContentAddition.AddItemDef(uselessItem);
        }
    }
}