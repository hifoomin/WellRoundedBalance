using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace UltimateCustomRun.Items.Greens
{
    public class RunaldsBand : ItemBase
    {
        // ////////////
        //
        // Thanks to Borbo
        //
        // ///////////////
        public static float BaseDamage;

        public static float TotalDamage;

        public override string Name => ":: Items :: Greens :: Runalds Band";
        public override string InternalPickupToken => "icering";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Hits that deal <style=cIsDamage>more than " + d(KjarosBand.Threshold) + " damage</style> also blasts enemies with a <style=cIsDamage>runic ice blast</style>, <style=cIsUtility>slowing</style> them by <style=cIsUtility>80%</style> for <style=cIsUtility>3s</style> <style=cStack>(+3s per stack)</style> and dealing <style=cIsDamage>" + d(TotalDamage) + "</style> <style=cStack>(+" + d(TotalDamage) + " per stack)</style> TOTAL damage. Recharges every <style=cIsUtility>" + KjarosBand.Cooldown + "</style> seconds.";

        public override void Init()
        {
            TotalDamage = ConfigOption(2.5f, "Damage", "Decimal. Per Stack. Vanilla is 2.5");
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
                c.Next.Operand = TotalDamage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Runalds's Band Damage hook");
            }
        }
    }
}