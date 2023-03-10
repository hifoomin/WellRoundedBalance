using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Yellows
{
    public class ChargedPerforator : ItemBase
    {
        public override string Name => ":: Items :::: Yellows :: Charged Perforator";
        public override string InternalPickupToken => "lightningStrikeOnHit";

        public override string PickupText => "Chance on hit to call down a lightning strike.";

        public override string DescText => "<style=cIsDamage>10%</style> chance on hit to call down a lightning strike, dealing <style=cIsDamage>" + d(baseTotalDamage) + "</style> <style=cStack>(+" + d(totalDamagePerStack) + " per stack)</style> TOTAL damage.";

        [ConfigField("Base TOTAL Damage", "Decimal. ", 4f)]
        public static float baseTotalDamage;

        [ConfigField("TOTAL Damage Per Stack", "Decimal. ", 2f)]
        public static float totalDamagePerStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += Changes;
            Changes();
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            float initialDamage = baseTotalDamage - totalDamagePerStack;

            bool error = true;
            if (c.TryGotoNext(x => x.MatchLdsfld(typeof(RoR2Content.Items), "LightningStrikeOnHit")) &&
                c.TryGotoNext(x => x.MatchLdfld<DamageInfo>("damage")))
            {
                c.Index += 3;
                c.Next.Operand = totalDamagePerStack;
                c.Index += 4;
                c.EmitDelegate<Func<float, float>>((damageCoefficient) =>
                {
                    return damageCoefficient + initialDamage;
                });

                if (c.TryGotoNext(x => x.MatchCallvirt<RoR2.Orbs.OrbManager>("AddOrb")))
                {
                    c.EmitDelegate<Func<RoR2.Orbs.SimpleLightningStrikeOrb, RoR2.Orbs.SimpleLightningStrikeOrb>>((orb) =>
                    {
                        orb.procCoefficient = 0f;
                        return orb;
                    });
                }
                error = false;
            }
            if (error)
            {
                Main.WRBLogger.LogError("Failed to apply Charged Perforator hook");
            }
        }

        private void Changes()
        {
            LanguageAPI.Add("ITEM_lightningStrikeOnHit_NAME".ToUpper(), "Charged Peripherator");
        }
    }
}