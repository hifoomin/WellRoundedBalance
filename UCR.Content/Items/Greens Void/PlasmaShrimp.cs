using MonoMod.Cil;
using UnityEngine;

namespace UltimateCustomRun.Items.VoidGreens
{
    public class PlasmaShrimp : ItemBase
    {
        public static float Shield;
        public static float TotalDamage;
        public static float ProcCoefficient;

        public override string Name => ":: Items ::::::: Void Greens :: Plasma Shrimp";
        public override string InternalPickupToken => "missileVoid";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "Gain a <style=cIsHealing>shield</style> equal to <style=cIsHealing>10%</style> of your maximum health. While you have a <style=cIsHealing>shield</style>, hitting an enemy fires a missile that deals <style=cIsDamage>40%</style> <style=cStack>(+50% per stack)</style> TOTAL damage. <style=cIsVoid>Corrupts all AtG Missile Mk. 1s</style>.";

        public override void Init()
        {
            Shield = ConfigOption(0.1f, "Shield", "Decimal. Vanilla is 0.1");
            TotalDamage = ConfigOption(0.4f, "Base TOTAL Damage", "Decimal. Vanilla is 0.4");
            ProcCoefficient = ConfigOption(0.2f, "Proc Coefficient", "Vanilla is 0.2");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeShield;
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeDamage;
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeProcCo;
        }

        private void ChangeProcCo(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.2f),
                    x => x.MatchStfld<RoR2.Orbs.GenericDamageOrb>("procCoefficient")))
            {
                c.Next.Operand = TotalDamage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Plasma Shrimp Proc Coefficient hook");
            }
        }

        private void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.4f)))
            {
                c.Next.Operand = TotalDamage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Plasma Shrimp Damage hook");
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
                c.Next.Operand = Shield;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Plasma Shrimp Shield hook");
            }
        }
    }
}