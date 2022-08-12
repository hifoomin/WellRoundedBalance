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
            base.Init();
        }

        public override void Hooks()
        {
            IL.EntityStates.GenericCharacterMain.ProcessJump += ChangeDistance;
        }

        public static void ChangeDistance(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(10f)))
            {
                c.Next.Operand = Distance;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Wax Quail Distance hook");
            }
        }
    }
}