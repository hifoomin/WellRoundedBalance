using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2.Orbs;
using System;

namespace WellRoundedBalance.Items.VoidGreens
{
    public class Polylute : ItemBase
    {
        public override string Name => ":: Items :::::: Voids :: Polylute";
        public override string InternalPickupToken => "chainLightningVoid";

        public override string PickupText => "Chance to repeatedly strike a single enemy with lightning. <style=cIsVoid>Corrupts all Ukuleles</style>.";
        public override string DescText => "<style=cIsDamage>25%</style> chance to fire <style=cIsDamage>lightning</style> for <style=cIsDamage>" + d(totalDamage) + "</style> TOTAL damage up to <style=cIsDamage>4<style=cStack> (+1 per stack)</style></style> times. <style=cIsVoid>Corrupts all Ukuleles</style>.";

        [ConfigField("TOTAL Damage", "Decimal.", 0.25f)]
        public static float totalDamage;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        }

        private void GlobalEventManager_OnHitEnemy(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchCallOrCallvirt(typeof(Util).GetMethod("CheckRoll",
                        new Type[] { typeof(float), typeof(CharacterMaster) })),
                    x => x.MatchBrfalse(out _),
                    x => x.MatchLdcR4(0.6f)))
            {
                c.Index += 2;
                c.Next.Operand = totalDamage;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Polylute Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchStfld(typeof(VoidLightningOrb), "isCrit"),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcI4(3),
                x => x.MatchLdloc(out _),
                x => x.MatchMul()))
            {
                c.Index += 4;
                c.Emit(OpCodes.Add);
                c.Remove();
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Polylute Count hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.2f),
                x => x.MatchStfld<VoidLightningOrb>("procCoefficient")))
            {
                c.Next.Operand = 0f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Polylute Proc Coefficient hook");
            }
        }
    }
}