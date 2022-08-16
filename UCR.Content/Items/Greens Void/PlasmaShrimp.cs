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
        public override string DescText => "Gain a <style=cIsHealing>shield</style> equal to <style=cIsHealing>" + d(Shield) + "</style> of your maximum health. While you have a <style=cIsHealing>shield</style>, hitting an enemy fires a missile that deals <style=cIsDamage>" + d(TotalDamage) + "</style> <style=cStack>(+ " + d(TotalDamage) + " per stack)</style> TOTAL damage. <style=cIsVoid>Corrupts all AtG Missile Mk. 1s</style>.";

        public override void Init()
        {
            Shield = ConfigOption(0.1f, "Shield", "Decimal. Vanilla is 0.1");
            TotalDamage = ConfigOption(0.4f, "Damage", "Decimal. Vanilla is 0.4");
            ProcCoefficient = ConfigOption(0.2f, "Proc Coefficient", "Vanilla is 0.2");
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
                c.Next.Operand = TotalDamage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Plasma Shrimp Damage hook");
            }

            c.Index = 0;

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