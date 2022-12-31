using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace WellRoundedBalance.Items.Greens
{
    public class KjarosBand : ItemBase
    {
        // ////////////
        //
        // Thanks to Borbo
        //
        // ///////////////

        public override string Name => ":: Items :: Greens :: Kjaros Band";
        public override string InternalPickupToken => "firering";

        public override string PickupText => "High damage hits also blasts enemies with a runic flame tornado. Recharges over time.";

        public override string DescText => "Hits that deal <style=cIsDamage>more than 400% damage</style> also blasts enemies with a <style=cIsDamage>runic flame tornado</style>, dealing <style=cIsDamage>150%</style> <style=cStack>(+150% per stack)</style> TOTAL damage over time. Recharges every <style=cIsUtility>10</style> seconds.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += Changes;
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld<RoR2.Projectile.ProjectileSimple>("lifetime"),
                x => x.MatchStloc(out _),
                x => x.MatchLdcR4(3f)))
            {
                c.Index += 2;
                c.Next.Operand = 1.5f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Kjaro's Band Damage hook");
            }
        }
    }
}