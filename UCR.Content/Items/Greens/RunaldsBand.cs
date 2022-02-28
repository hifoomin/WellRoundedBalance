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

        public override string DescText => "Hits that deal <style=cIsDamage>more than " + d(KjarosBand.Threshold) + " Damage</style> also blasts enemies with a <style=cIsDamage>runic ice blast</style>, <style=cIsUtility>slowing</style> them by <style=cIsUtility>80%</style> for <style=cIsUtility>3s</style> <style=cStack>(+3s per stack)</style>" +
                                            (TotalDamage != 0f ? " and <style=cIsDamage>" + d(TotalDamage) + "</style> <style=cStack>(+" + d(TotalDamage) + " per stack)</style> TOTAL damage" : "" +
                                            (BaseDamage != 0f && TotalDamage != 0f ? " and" : "") +
                                            (BaseDamage != 0f ? " <style=cIsDamage>" + d(BaseDamage) + "</style> <style=cStack>(+" + d(BaseDamage) + " per stack)</style> base damage." : "") +
                                            " Recharges every <style=cIsUtility>" + KjarosBand.Cooldown + "</style> seconds.");

        public override void Init()
        {
            BaseDamage = ConfigOption(0f, "Base Damage", "Decimal. Per Stack. Vanilla is 0");
            TotalDamage = ConfigOption(2.5f, "Total Damage", "Decimal. Per Stack. Vanilla is 2.5");
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

        // PLEASE HELP TO FIX
    }
}