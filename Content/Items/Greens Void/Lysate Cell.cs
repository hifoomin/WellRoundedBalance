﻿using MonoMod.Cil;

namespace WellRoundedBalance.Items.VoidGreens
{
    public class LysateCell : ItemBase
    {
        public override string Name => ":: Items :::::: Voids :: Lysate Cell";
        public override string InternalPickupToken => "equipmentMagazineVoid";

        public override string PickupText => "Add an extra charge of your Special skill. <style=cIsVoid>Corrupts all Fuel Cells</style>.";

        public override string DescText => "Add <style=cIsUtility>+1</style> <style=cStack>(+1 per stack)</style> charge of your <style=cIsUtility>Special skill</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeCDR;
        }

        private void ChangeCDR(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.67f)))
            {
                c.Next.Operand = 1f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Lysate Cell Cooldown Reduction hook");
            }
        }
    }
}