using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace UltimateCustomRun
{
    public class KjarosBand : ItemBase
    {
        public static float basedmg;
        public static float totaldmg;
        public static float threshold;
        public static float cooldown;

        public override string Name => ":: Items :: Greens :: Kjaros Band";
        public override string InternalPickupToken => "firering";
        public override bool NewPickup => false;

        bool kBaseDamage = basedmg != 0f;
        bool kTotalDamage = totaldmg != 0f;
        bool kBoth = basedmg != 0f && totaldmg != 0f;

        public override string PickupText => "";

        public override string DescText => "Hits that deal <style=cIsDamage>more than " + d(threshold) + " damage</style> also blasts enemies with a <style=cIsDamage>runic flame tornado</style>, dealing " +
                                            (kTotalDamage ? "<style=cIsDamage>" + d(totaldmg) + "</style> <style=cStack>(+" + d(totaldmg) + " per stack)</style> TOTAL damage over time" : "") +
                                            (kBoth ? " and " : "") +
                                            (kBaseDamage ? "<style=cIsDamage>" + d(basedmg) + "</style> <style=cStack>(+" + d(basedmg) + " per stack)</style> base damage." : "") +
                                            " Recharges every <style=cIsUtility>" + cooldown + "</style> seconds.";

        public override void Init()
        {
            basedmg = ConfigOption(0f, "Base Damage", "Decimal. Per Stack. Vanilla is 0");
            totaldmg = ConfigOption(3f, "Total Damage", "Decimal. Per Stack. Vanilla is 3");
            threshold = ConfigOption(4f, "Threshold", "Decimal. Affects both Bands. Vanilla is 4");
            cooldown = ConfigOption(10f, "Cooldown", "Affects both Bands. Vanilla is 10");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += KjaroChange;
            IL.RoR2.GlobalEventManager.OnHitEnemy += BandsThreshold;
            IL.RoR2.GlobalEventManager.OnHitEnemy += BandsCooldown;
        }
        public static void KjaroChange(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            int itemCountLocation = 51;
            int totalDamageMultiplierLocation = 56;

            c.GotoNext(MoveType.After,
                x => x.MatchLdsfld("RoR2.RoR2Content/Items", "FireRing"),
                x => x.MatchCallOrCallvirt<Inventory>(nameof(Inventory.GetItemCount)),
                x => x.MatchStloc(out itemCountLocation)
                );

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(out _),
                x => x.MatchLdloc(itemCountLocation),
                x => x.MatchConvR4(),
                x => x.MatchMul(),
                x => x.MatchStloc(out totalDamageMultiplierLocation)
                );
            c.Remove();
            c.Emit(OpCodes.Ldc_R4, totaldmg);

            c.GotoNext(MoveType.After,
                x => x.MatchLdloc(totalDamageMultiplierLocation),
                x => x.MatchCallOrCallvirt(out _)
                );
            c.Emit(OpCodes.Ldloc_0);
            c.EmitDelegate<Func<float, CharacterBody, float>>((damage, self) =>
            {
                float dam = self.baseDamage * basedmg;

                return damage + dam;
            });
        }
        public static void BandsThreshold(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("get_damage"),
                x => x.MatchDiv(),
                x => x.MatchLdcR4(4f)
            );
            c.Index += 2;
            c.Next.Operand = threshold;
        }
        public static void BandsCooldown(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdloc(out _),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(10f),
                x => x.MatchBle(out _)
            );
            c.Index += 2;
            c.Next.Operand = cooldown;
        }
    }
}
