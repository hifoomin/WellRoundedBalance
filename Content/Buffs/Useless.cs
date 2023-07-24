namespace WellRoundedBalance.Buffs
{
    public class Useless
    {
        public static BuffDef uselessBuff;
        public static BuffDef voidtouchedSaferSpaces;
        public static BuffDef oddlyShapedOpalUseless;
        public static BuffDef chronobaubleUseless;

        public static void Init()
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

            chronobaubleUseless = ScriptableObject.CreateInstance<BuffDef>();
            chronobaubleUseless.isHidden = true;
            chronobaubleUseless.isDebuff = false;
            chronobaubleUseless.canStack = false;

            ContentAddition.AddBuffDef(uselessBuff);
            ContentAddition.AddBuffDef(voidtouchedSaferSpaces);
            ContentAddition.AddBuffDef(oddlyShapedOpalUseless);
            ContentAddition.AddBuffDef(chronobaubleUseless);
        }
    }
}