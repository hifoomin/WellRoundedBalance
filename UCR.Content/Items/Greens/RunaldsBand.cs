using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace UltimateCustomRun
{
    public class RunaldsBand : Based
    {
        public static float basedmg;
        public static float totaldmg;

        public override string Name => ":: Items :: Greens :: Runalds Band";
        public override string InternalPickupToken => "icering";
        public override bool NewPickup => false;

        public static bool rBaseDamage = basedmg != 0f;
        public static bool rTotalDamage = totaldmg != 0f;
        public static bool rBoth = rBaseDamage && rTotalDamage;

        public override string PickupText => "";

        public override string DescText => "Hits that deal <style=cIsDamage>more than " + d(KjarosBand.threshold) + " damage</style> also blasts enemies with a <style=cIsDamage>runic ice blast</style>, <style=cIsUtility>slowing</style> them by <style=cIsUtility>80%</style> for <style=cIsUtility>3s</style> <style=cStack>(+3s per stack)</style>" +
                                            (rTotalDamage ? " and <style=cIsDamage>" + d(totaldmg) + "</style> <style=cStack>(+" + d(totaldmg) + " per stack)</style> TOTAL damage" : "" +
                                            (rBoth ? " and " : "") +
                                            (rBaseDamage ? "<style=cIsDamage>" + d(basedmg) + "</style> <style=cStack>(+" + d(basedmg) + " per stack)</style> base damage." : "") +
                                            " Recharges every <style=cIsUtility>" + KjarosBand.cooldown + "</style> seconds.");

        public override void Init()
        {
            basedmg = ConfigOption(0f, "Base Damage", "Decimal. Per Stack. Vanilla is 0");
            totaldmg = ConfigOption(2.5f, "Total Damage", "Decimal. Per Stack. Vanilla is 2.5");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += RunaldChange;
        }
        public static void RunaldChange(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            int itemCountLocation = 51;
            int totalDamageMultiplierLocation = 56;

            c.GotoNext(MoveType.After,
                x => x.MatchLdsfld("RoR2.RoR2Content/Items", "IceRing"),
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
        // PLEASE HELP TO FIX
    }
}
