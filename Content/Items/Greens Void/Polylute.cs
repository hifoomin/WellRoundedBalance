using MonoMod.Cil;
using RoR2;
using System;

namespace WellRoundedBalance.Items.VoidGreens
{
    public class Polylute : ItemBase
    {
        public override string Name => ":: Items :::::: Voids :: Polylute";
        public override string InternalPickupToken => "chainLightningVoid";

        public override string PickupText => "Chance to repeatedly strike a single enemy with lightning. <style=cIsVoid>Corrupts all Ukuleles</style>.";
        public override string DescText => "<style=cIsDamage>25%</style> chance to fire <style=cIsDamage>lightning</style> for <style=cIsDamage>30%</style> TOTAL damage up to <style=cIsDamage>3<style=cStack> (+2 per stack)</style></style> times. <style=cIsVoid>Corrupts all Ukuleles</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += Changes;
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchCallOrCallvirt(typeof(Util).GetMethod("CheckRoll",
                        new Type[] { typeof(float), typeof(CharacterMaster) })),
                    x => x.MatchBrfalse(out _),
                    x => x.MatchLdcR4(0.6f)))
            {
                c.Index += 2;
                c.Next.Operand = 0.3f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Polylute Damage hook");
            }
        }
    }
}