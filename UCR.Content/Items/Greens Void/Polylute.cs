using MonoMod.Cil;
using RoR2;
using System;

namespace UltimateCustomRun.Items.VoidGreens
{
    public class Polylute : ItemBase
    {
        public static float Chance;
        public static float TotalDamage;
        public static int Strikes;
        public static float ProcCoefficient;

        public override string Name => ":: Items ::::::: Void Greens :: Polylute";
        public override string InternalPickupToken => "chainLightningVoid";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "<style=cIsDamage>" + d(Chance) + "</style> chance to fire <style=cIsDamage>lightning</style> for <style=cIsDamage>" + d(TotalDamage) + "</style> TOTAL damage up to <style=cIsDamage>" + Strikes + " <style=cStack>(+" + Strikes + " per stack)</style></style> times. <style=cIsVoid>Corrupts all Ukuleles</style>.";

        public override void Init()
        {
            Chance = ConfigOption(25f, "Chance", "Vanilla is 25");
            TotalDamage = ConfigOption(0.6f, "TOTAL Damage", "Decimal. Vanilla is 0.6");
            Strikes = ConfigOption(3, "Hit Count", "Per Stack. Vanilla is 3");
            ProcCoefficient = ConfigOption(0.2f, "Proc Coefficient", "Vanilla is 0.2");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeChance;
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeDamage;
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeStrikes;
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeProcCo;
        }

        private void ChangeProcCo(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.2f),
                x => x.MatchStfld<RoR2.Orbs.VoidLightningOrb>("procCoefficient")))
            {
                c.Next.Operand = ProcCoefficient;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Polylute Proc Coefficient hook");
            }
        }

        private void ChangeStrikes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchStfld<RoR2.Orbs.VoidLightningOrb>("isCrit"),
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdcI4(3)))
            {
                c.Index += 2;
                c.Next.Operand = Strikes;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Polylute Hits hook");
            }
        }

        private void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchCallOrCallvirt(typeof(Util).GetMethod("CheckRoll",
                        new Type[] { typeof(float), typeof(CharacterMaster) })),
                    x => x.MatchBrfalse(out _),
                    x => x.MatchLdcR4(0.6f)))
            {
                c.Index += 2;
                c.Next.Operand = TotalDamage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Polylute Damage hook");
            }
        }

        private void ChangeChance(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdsfld("RoR2.DLC1Content/Items", "ChainLightningVoid"),
                    x => x.MatchCallOrCallvirt<Inventory>("GetItemCount"),
                    x => x.MatchStloc(out _),
                    x => x.MatchLdcR4(25f)))
            {
                c.Index += 3;
                c.Next.Operand = Chance;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Polylute Chance hook");
            }
        }
    }
}