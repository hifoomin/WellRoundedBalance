using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace WellRoundedBalance.Items.Whites
{
    public class Medkit : ItemBase
    {
        public override string Name => ":: Items : Whites :: Medkit";
        public override string InternalPickupToken => "medkit";

        public override string PickupText => "Receive a delayed heal after taking damage.";

        public override string DescText => "2 seconds after getting hurt, <style=cIsHealing>heal</style> for <style=cIsHealing>20</style> plus an additional <style=cIsHealing>3.5%<style=cStack> (+3.5% per stack)</style></style> of <style=cIsHealing>maximum health</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RemoveBuff_BuffIndex += Changes;
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);
            /*
            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdsfld(typeof(RoR2Content.Items), "Medkit"),
                    x => x.MatchCallOrCallvirt<Inventory>("GetItemCount"),
                    x => x.MatchStloc(0),
                    x => x.MatchLdcR4(20f)))
            {
                c.Index += 3;
                c.Next.Operand = 20f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Medkit Flat Healing hook");
            }

            c.Index = 0;
            */

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdarg(0),
                    x => x.MatchCallOrCallvirt(typeof(CharacterBody).GetMethod("get_maxHealth")),
                    x => x.MatchLdcR4(0.05f)))
            {
                c.Index += 2;
                c.Next.Operand = 0.035f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Medkit Percent Healing hook");
            }
        }
    }
}