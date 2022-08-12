using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace UltimateCustomRun.Items.Greens
{
    public class KjarosBand : ItemBase
    {
        // ////////////
        //
        // Thanks to Borbo
        //
        // ///////////////

        public static float TotalDamage;
        public static float Threshold;
        public static float Cooldown;

        public override string Name => ":: Items :: Greens :: Kjaros Band";
        public override string InternalPickupToken => "firering";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Hits that deal <style=cIsDamage>more than " + d(Threshold) + " damage</style> also blasts enemies with a <style=cIsDamage>runic flame tornado</style>, dealing <style=cIsDamage>" + d(TotalDamage) + "</style> <style=cStack>(+" + d(TotalDamage) + " per stack)</style> TOTAL damage over time. Recharges every <style=cIsUtility>" + Cooldown + "</style> seconds.";

        public override void Init()
        {
            TotalDamage = ConfigOption(3f, "Total Damage", "Decimal. Per Stack. Vanilla is 3");
            Threshold = ConfigOption(4f, "Damage Threshold", "Decimal. Affects both Bands. Vanilla is 4");
            Cooldown = ConfigOption(10f, "Cooldown", "Affects both Bands. Vanilla is 10");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeDamage;
            IL.RoR2.GlobalEventManager.OnHitEnemy += BandsThreshold;
            IL.RoR2.GlobalEventManager.OnHitEnemy += BandsCooldown;
        }

        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld<RoR2.Projectile.ProjectileSimple>("lifetime"),
                x => x.MatchStloc(out _),
                x => x.MatchLdcR4(3f)))
            {
                c.Index += 2;
                c.Next.Operand = TotalDamage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Kjaro's Band Damage hook");
            }
        }

        public static void BandsThreshold(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<CharacterBody>("get_damage"),
                x => x.MatchDiv(),
                x => x.MatchLdcR4(4f)))
            {
                c.Index += 2;
                c.Next.Operand = Threshold;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Kjaro's Band Threshold hook");
            }
        }

        public static void BandsCooldown(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdloc(out _),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(10f),
                x => x.MatchBle(out _)))
            {
                c.Index += 2;
                c.Next.Operand = Cooldown;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Kjaro's Band Cooldown hook");
            }
        }
    }
}