using MonoMod.Cil;

namespace UltimateCustomRun.Items.Greens
{
    public class WaxQuail : ItemBase
    {
        public static float Distance;

        public override string Name => ":: Items :: Greens :: Wax Quail";
        public override string InternalPickupToken => "jumpBoost";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "<style=cIsUtility>Jumping</style> while <style=cIsUtility>sprinting</style> boosts you forward by <style=cIsUtility>" + Distance + "m</style><style=cStack> (+" + Distance + "m per stack)</style>.";

        public override void Init()
        {
            Distance = ConfigOption(10f, "Distance", "Per Stack. Vanilla is 10");
            ROSOption("Greens", 0f, 50f, 1f, "2");
            base.Init();
        }

        public override void Hooks()
        {
            IL.EntityStates.GenericCharacterMain.ProcessJump += ChangeDistance;
        }

        public static void ChangeDistance(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(10f)
            );
            c.Next.Operand = Distance;
        }
    }
}