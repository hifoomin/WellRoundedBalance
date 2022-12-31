using MonoMod.Cil;
using UnityEngine;

namespace WellRoundedBalance.Items.Greens
{
    public class OldGuillotine : ItemBase
    {
        public override string Name => ":: Items :: Greens :: Old Guillotine";
        public override string InternalPickupToken => "executeLowHealthElite";

        public override string PickupText => "Instantly kill low health Elite monsters.";

        public override string DescText => "Instantly kill Elite monsters below <style=cIsHealth>26% <style=cStack>(+26% per stack)</style> health</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.OnInventoryChanged += ChangeThreshold;
        }

        public static void ChangeThreshold(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(13f)))
            {
                c.Next.Operand = 26f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Old Guillotine Threshold hook");
            }
        }
    }
}