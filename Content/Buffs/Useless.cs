namespace WellRoundedBalance.Buffs
{
    public class Useless
    {
        public static BuffDef uselessBuff;
        public static BuffDef voidtouchedSaferSpaces;
        public static BuffDef oddlyShapedOpalUseless;

        public static void Create()
        {
            uselessBuff = ScriptableObject.CreateInstance<BuffDef>();
            uselessBuff.isHidden = true;
            uselessBuff.isDebuff = false;
            uselessBuff.canStack = false;

            voidtouchedSaferSpaces = ScriptableObject.CreateInstance<BuffDef>();
            voidtouchedSaferSpaces.isHidden = true;
            voidtouchedSaferSpaces.isDebuff = false;
            voidtouchedSaferSpaces.canStack = false;

            oddlyShapedOpalUseless = ScriptableObject.CreateInstance<BuffDef>();
            oddlyShapedOpalUseless.isHidden = true;
            oddlyShapedOpalUseless.isDebuff = false;
            oddlyShapedOpalUseless.canStack = false;

            ContentAddition.AddBuffDef(uselessBuff);
            ContentAddition.AddBuffDef(voidtouchedSaferSpaces);
            ContentAddition.AddBuffDef(oddlyShapedOpalUseless);
        }
    }
}