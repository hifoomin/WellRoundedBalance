﻿using MonoMod.Cil;
using RoR2.Orbs;

namespace WellRoundedBalance.Items.Reds
{
    public class UnstableTeslaCoil : ItemBase
    {
        public override string Name => ":: Items ::: Reds :: Unstable Tesla Coil";
        public override string InternalPickupToken => "shockNearby";

        public override string PickupText => "Shock all nearby enemies every 0.5 seconds.";

        public override string DescText => "Fire out <style=cIsDamage>lightning</style> that hits <style=cIsDamage>3</style> <style=cStack>(+2 per stack)</style> enemies for <style=cIsDamage>330%</style> damage per second. The Tesla Coil switches off every <style=cIsDamage>10 seconds</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.Items.ShockNearbyBodyBehavior.FixedUpdate += Changes;
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(2f)))
            {
                c.Next.Operand = 1.65f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Unstable Tesla Coil Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.3f)))
            {
                c.Next.Operand = 0f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Unstable Tesla Coil Proc Coefficient hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(35f)))
            {
                c.Next.Operand = 25f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Unstable Tesla Coil Range hook");
            }
        }
    }
}