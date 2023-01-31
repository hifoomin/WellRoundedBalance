namespace WellRoundedBalance.Buffs
{
    public class Useless
    {
        public static BuffDef uselessBuff;
        public static BuffDef voidtouchedSaferSpaces;

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

            ContentAddition.AddBuffDef(uselessBuff);
            ContentAddition.AddBuffDef(voidtouchedSaferSpaces);
        }
    }
}