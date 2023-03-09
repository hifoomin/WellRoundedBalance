using MonoMod.Cil;

namespace WellRoundedBalance.Items.VoidGreens
{
    public class VoidsentFlame : ItemBase
    {
        public override string Name => ":: Items :::::: Voids :: Voidsent Flame";
        public override string InternalPickupToken => "explodeOnDeathVoid";

        public override string PickupText => "Full health enemies also detonate on hit. <style=cIsVoid>Corrupts all Will-o'-the-wisps</style>.";

        public override string DescText => "Upon hitting an enemy at <style=cIsDamage>100% health</style>, <style=cIsDamage>detonate</style> them in a <style=cIsDamage>" + baseRange + "m</style>" +
                                           (rangePerStack > 0 ? " <style=cStack>(+" + rangePerStack + "m per stack)</style>" : "") +
                                           " radius burst for <style=cIsDamage>" + d(baseDamage) + "</style> <style=cStack>(+" + d(damagePerStack) + " per stack)</style> base damage. <style=cIsVoid>Corrupts all Will-o'-the-wisps</style>.";

        [ConfigField("Base Damage", "Decimal.", 1f)]
        public static float baseDamage;

        [ConfigField("Damage Per Stack", "Decimal.", 0.5f)]
        public static float damagePerStack;

        [ConfigField("Base Range", 12f)]
        public static float baseRange;

        [ConfigField("Range Per Stack", 0f)]
        public static float rangePerStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            Changes();
        }

        private void HealthComponent_TakeDamage(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(12f),
                    x => x.MatchLdcR4(2.4f)))
            {
                c.Next.Operand = baseRange;
                c.Index += 1;
                c.Next.Operand = rangePerStack;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Voidsent Flame Radius hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(2.6f),
                    x => x.MatchLdcR4(1f),
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdcI4(1),
                    x => x.MatchSub(),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.6f)))
            {
                c.Next.Operand = baseDamage;
                c.Index += 6;
                c.Next.Operand = damagePerStack / baseDamage;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Voidsent Flame Damage hook");
            }
        }

        private void Changes()
        {
            var hopooGames = Utils.Paths.GameObject.ExplodeOnDeathVoidExplosion.Load<GameObject>();
            var delayBlast = hopooGames.GetComponent<DelayBlast>();
            delayBlast.procCoefficient = 0f;
        }
    }
}