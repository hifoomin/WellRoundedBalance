using MonoMod.Cil;

namespace WellRoundedBalance.Items.Whites
{
    internal class Gasoline : ItemBase
    {
        public override string Name => ":: Items : Whites :: Gasoline";

        public override string InternalPickupToken => "igniteOnKill";

        public override string PickupText => "Killing an enemy ignites other nearby enemies.";

        public override string DescText => "Killing an enemy <style=cIsDamage>ignites</style> all enemies within <style=cIsDamage>" + baseRange + "m</style>" +
                                           (rangePerStack > 0 ? " <style=cStack>(+" + rangePerStack + "m per stack)</style>" : "") +
                                           " for <style=cIsDamage>" + d(explosionDamage) + "</style> base damage." +
                                           " Additionally, enemies <style=cIsDamage>burn</style> for <style=cIsDamage>" + d(burnDamagePerStack * (baseBurnDamage + 1)) + "</style> <style=cStack>(+" + (baseBurnDamage != 1 ? d(burnDamagePerStack * (baseBurnDamage + 2)) : d(burnDamagePerStack * (baseBurnDamage + 1)) + " per stack)</style> base damage.");

        [ConfigField("Explosion Damage", "Decimal.", 1f)]
        public static float explosionDamage;

        [ConfigField("Base Burn Damage", "Decimal. Formula for burn damage: Burn Damage Per Stack * (Base Burn Damage + Gasoline)", 1)]
        public static int baseBurnDamage;

        [ConfigField("Burn Damage Per Stack", "Decimal. Formula for burn damage: Burn Damage Per Stack * (Base Burn Damage + Gasoline)", 1f)]
        public static float burnDamagePerStack;

        [ConfigField("Base Range", "", 12f)]
        public static float baseRange;

        [ConfigField("Range Per Stack", "", 0f)]
        public static float rangePerStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.ProcIgniteOnKill += GlobalEventManager_ProcIgniteOnKill;
        }

        private void GlobalEventManager_ProcIgniteOnKill(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcI4(1),
                    x => x.MatchLdarg(1),
                    x => x.MatchAdd(),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.75f)))
            {
                c.Next.Operand = baseBurnDamage;
                c.Index += 4;
                c.Next.Operand = burnDamagePerStack;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Gasoline Burn Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
               x => x.MatchAdd(),
               x => x.MatchStloc(1),
               x => x.MatchLdcR4(1.5f)))
            {
                c.Index += 2;
                c.Next.Operand = explosionDamage;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Gasoline Explosion Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(8f),
                x => x.MatchLdcR4(4f)))
            {
                c.Next.Operand = baseRange - rangePerStack;
                c.Index += 1;
                c.Next.Operand = rangePerStack;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Gasoline Radius hook");
            }
        }
    }
}