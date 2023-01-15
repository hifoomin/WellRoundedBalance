using HG;
using MonoMod.Cil;

namespace WellRoundedBalance.Items.VoidGreens
{
    public class VoidsentFlame : ItemBase
    {
        public override string Name => ":: Items :::::: Voids :: Voidsent Flame";
        public override string InternalPickupToken => "explodeOnDeathVoid";

        public override string PickupText => "Full health enemies also detonate on hit. <style=cIsVoid>Corrupts all Will-o'-the-wisps</style>.";
        public override string DescText => "Upon hitting an enemy at <style=cIsDamage>100% health</style>, <style=cIsDamage>detonate</style> them in a <style=cIsDamage>12m</style> <style=cStack>(+2.4m per stack)</style> radius burst for <style=cIsDamage>150%</style> <style=cStack>(+75% per stack)</style> base damage. <style=cIsVoid>Corrupts all Will-o'-the-wisps</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += Changes;
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(1000f)))
            {
                c.Next.Operand = 0f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Voidsent Flame Knockback hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(12f),
                    x => x.MatchLdcR4(2.4f)))
            {
                c.Next.Operand = 12f;
                c.Index += 1;
                c.Next.Operand = 0f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Voidsent Flame Radius hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(2.6f),
                    x => x.MatchLdcR4(1f),
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdcI4(1),
                    x => x.MatchSub(),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.6f)))
            {
                c.Next.Operand = 1.5f;
                c.Index += 6;
                c.Next.Operand = 0.5f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Voidsent Flame Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcI4(2),
                x => x.MatchStfld("RoR2.DelayBlast", "falloffModel")))
            {
                c.Next.Operand = 0;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Voidsent Flame Falloff hook");
            }
        }
    }
}