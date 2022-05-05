using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace UltimateCustomRun.Items.Greens
{
    public class IgnitionTank : ItemBase
    {
        public static int Damage;

        public override string Name => ":: Items :: Greens :: Ignition Tank";
        public override string InternalPickupToken => "strengthenBurn";
        public override bool NewPickup => true;
        public override string PickupText => "Your ignite effects deal " + Damage + "x damage.";
        public override string DescText => "Ignite effects deal <style=cIsDamage>+" + d(Damage) + "</style> <style=cStack>(+" + d(Damage) + " per stack)</style> more damage over time.";

        public override void Init()
        {
            Damage = ConfigOption(3, "Burn Damage", "Decimal. Per Stack. Vanilla is 3");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.StrengthenBurnUtils.CheckDotForUpgrade += ChangeDamage;
        }

        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcI4(3)
            );
            c.Index += 1;
            c.Next.Operand = Damage;
        }
    }
}