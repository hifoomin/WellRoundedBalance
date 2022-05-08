/*
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

        public static float BaseDamage;
        public static float TotalDamage;
        public static float Threshold;
        public static float Cooldown;

        public override string Name => ":: Items :: Greens :: Kjaros Band";
        public override string InternalPickupToken => "firering";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Hits that deal <style=cIsDamage>more than " + d(Threshold) + " Damage</style> also blasts enemies with a <style=cIsDamage>runic flame tornado</style>, dealing " +
                                            (TotalDamage != 0f ? "<style=cIsDamage>" + d(TotalDamage) + "</style> <style=cStack>(+" + d(TotalDamage) + " per stack)</style> TOTAL damage over time" : "") +
                                            (BaseDamage != 0f && TotalDamage != 0f ? " and " : "") +
                                            (BaseDamage != 0f ? "<style=cIsDamage>" + d(BaseDamage) + "</style> <style=cStack>(+" + d(BaseDamage) + " per stack)</style> base damage." : "") +
                                            " Recharges every <style=cIsUtility>" + Cooldown + "</style> seconds.";

        public override void Init()
        {
            BaseDamage = ConfigOption(0f, "Base Damage", "Decimal. Per Stack. Vanilla is 0");
            TotalDamage = ConfigOption(3f, "Total Damage", "Decimal. Per Stack. Vanilla is 3");
            Threshold = ConfigOption(4f, "Threshold", "Decimal. Affects both Bands. Vanilla is 4");
            Cooldown = ConfigOption(10f, "Cooldown", "Affects both Bands. Vanilla is 10");
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
            ILCursor c = new(il);

            int itemCountLocation = 81;
            int totalDamageMultiplierLocation = 91;

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
            c.Emit(OpCodes.Ldc_R4, TotalDamage);

            c.GotoNext(MoveType.After,
                x => x.MatchLdloc(totalDamageMultiplierLocation),
                x => x.MatchCallOrCallvirt(out _)
                );
            c.Emit(OpCodes.Ldloc_0);
            c.EmitDelegate<Func<float, CharacterBody, float>>((Damage, self) =>
            {
                float dam = self.baseDamage * BaseDamage;

                return Damage + dam;
            });
        }

        public static void BandsThreshold(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<CharacterBody>("get_damage"),
                x => x.MatchDiv(),
                x => x.MatchLdcR4(4f)
            );
            c.Index += 2;
            c.Next.Operand = Threshold;
        }

        public static void BandsCooldown(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdloc(out _),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(10f),
                x => x.MatchBle(out _)
            );
            c.Index += 2;
            c.Next.Operand = Cooldown;
        }
    }
}
*/