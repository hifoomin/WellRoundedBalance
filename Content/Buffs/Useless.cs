namespace WellRoundedBalance.Buffs
{
    public class Useless
    {
        public static BuffDef uselessBuff;

        public static void Create()
        {
            uselessBuff = ScriptableObject.CreateInstance<BuffDef>();
            uselessBuff.isHidden = true;
            uselessBuff.isDebuff = false;
            uselessBuff.canStack = false;

            ContentAddition.AddBuffDef(uselessBuff);
        }
    }
}