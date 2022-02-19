using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class WaxQuail : ItemBase
    {
        public static float distance;

        public override string Name => ":: Items :: Greens :: Wax Quail";
        public override string InternalPickupToken => "jumpBoost";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "<style=cIsUtility>Jumping</style> while <style=cIsUtility>sprinting</style> boosts you forward by <style=cIsUtility>" + distance + "m</style><style=cStack>(+" + distance + "m per stack)</style>.";


        public override void Init()
        {
            distance = ConfigOption(10f, "Distance", "Per Stack. Vanilla is 10");
            base.Init();
        }

        public override void Hooks()
        {
            IL.EntityStates.GenericCharacterMain.ProcessJump += ChangeDistance;
        }
        public static void ChangeDistance(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(10f)
            );
            c.Next.Operand = distance;
        }
    }
}
