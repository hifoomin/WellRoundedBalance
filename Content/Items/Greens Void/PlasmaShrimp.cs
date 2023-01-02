using MonoMod.Cil;
using UnityEngine;

namespace WellRoundedBalance.Items.VoidGreens
{
    public class PlasmaShrimp : ItemBase
    {
        public override string Name => ":: Items :::::: Voids :: Plasma Shrimp";
        public override string InternalPickupToken => "missileVoid";

        public override string PickupText => "While you have shield, fire missiles on every hit. <style=cIsVoid>Corrupts all AtG Missile Mk. 1s</style>.";
        public override string DescText => "Gain a <style=cIsHealing>shield</style> equal to <style=cIsHealing>20%</style> of your maximum health. While you have a <style=cIsHealing>shield</style>, hitting an enemy fires a missile that deals <style=cIsDamage>12%</style> <style=cStack>(+12% per stack)</style> TOTAL damage. <style=cIsVoid>Corrupts all AtG Missile Mk. 1s</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeShield;
            IL.RoR2.GlobalEventManager.OnHitEnemy += Changes;
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.4f)))
            {
                c.Next.Operand = 0.12f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Plasma Shrimp Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.2f),
                    x => x.MatchStfld<RoR2.Orbs.GenericDamageOrb>("procCoefficient")))
            {
                c.Next.Operand = 0f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Plasma Shrimp Proc Coefficient hook");
            }
        }

        private void ChangeShield(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("get_maxHealth"),
                    x => x.MatchLdcR4(0.1f)))
            {
                c.Index += 1;
                c.Next.Operand = 0.2f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Plasma Shrimp Shield hook");
            }
        }
    }
}