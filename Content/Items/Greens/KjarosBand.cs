﻿using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Greens
{
    public class KjarosBand : ItemBase<KjarosBand>
    {
        public override string Name => ":: Items :: Greens :: Kjaros Band";
        public override ItemDef InternalPickup => RoR2Content.Items.FireRing;

        public override string PickupText => "High damage hits also blast enemies with a runic flame tornado. Recharges over time.";

        public override string DescText => "Hits that deal <style=cIsDamage>more than 400% damage</style> also blast enemies for <style=cIsDamage>" + d(baseTotalDamage) + "</style> <style=cStack>(+" + d(totalDamagePerStack) + " per stack)</style> TOTAL damage over time. Recharges every <style=cIsUtility>10</style> seconds.";

        [ConfigField("Base TOTAL Damage", "Decimal.", 1.7f)]
        public static float baseTotalDamage;

        [ConfigField("TOTAL Damage Per Stack", "Decimal.", 1.7f)]
        public static float totalDamagePerStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.ProcessHitEnemy += Changes;
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchStloc(out _),
                x => x.MatchLdcR4(3f),
                x => x.MatchLdloc(out _),
                x => x.MatchConvR4(),
                x => x.MatchMul()))
            {
                c.Index += 1;
                c.Next.Operand = totalDamagePerStack;
                c.Index += 4;
                c.EmitDelegate<Func<float, float>>((self) =>
                {
                    return self + (baseTotalDamage - totalDamagePerStack);
                });
            }
            else
            {
                Logger.LogError("Failed to apply Kjaro's Band Damage hook");
            }
        }
    }
}