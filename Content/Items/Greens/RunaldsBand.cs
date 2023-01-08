using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace WellRoundedBalance.Items.Greens
{
    public class RunaldsBand : ItemBase
    {
        public override string Name => ":: Items :: Greens :: Runalds Band";
        public override string InternalPickupToken => "icering";

        public override string PickupText => "High damage hits also blast enemies with runic ice. Recharges over time.";

        public override string DescText => "Hits that deal <style=cIsDamage>more than 400% damage</style> also blast enemies with a <style=cIsDamage>runic ice blast</style>, <style=cIsUtility>slowing</style> them by <style=cIsUtility>80%</style> for <style=cIsUtility>3s</style> <style=cStack>(+3s per stack)</style> and dealing <style=cIsDamage>125%</style> <style=cStack>(+125% per stack)</style> TOTAL damage. Recharges every <style=cIsUtility>10</style> seconds.";

        // slows arent accurate in ror2
        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeDamage;
        }

        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchBle(out _),
                x => x.MatchLdcR4(2.5f)))
            {
                c.Index += 1;
                c.Next.Operand = 1.25f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Runalds's Band Damage hook");
            }
        }
    }
}