using System;

namespace WellRoundedBalance.Items.Yellows
{
    public class ChargedPerforator : ItemBase<ChargedPerforator>
    {
        public override string Name => ":: Items :::: Yellows :: Charged Perforator";
        public override ItemDef InternalPickup => RoR2Content.Items.LightningStrikeOnHit;

        public override string PickupText => "Chance on hit to call down a lightning strike.";

        public override string DescText => "<style=cIsDamage>10%</style> chance on hit to call down a lightning strike, dealing <style=cIsDamage>" + d(baseTotalDamage) + "</style> <style=cStack>(+" + d(totalDamagePerStack) + " per stack)</style> TOTAL damage.";

        [ConfigField("Base TOTAL Damage", "Decimal. ", 3.5f)]
        public static float baseTotalDamage;

        [ConfigField("TOTAL Damage Per Stack", "Decimal. ", 3.5f)]
        public static float totalDamagePerStack;

        [ConfigField("Proc Coefficient", 1f)]
        public static float procCoefficient;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.ProcessHitEnemy += Changes;
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
                        orb.procCoefficient = procCoefficient * Items.Greens._ProcCoefficients.globalProc;
                        return orb;
                    });
                }
                error = false;
            }
            if (error)
            {
                Logger.LogError("Failed to apply Charged Perforator Damage and Proc Coefficient hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdloc(1),
                x => x.MatchCallOrCallvirt<CharacterBody>("get_crit"),
                x => x.MatchLdloc(out _),
                x => x.MatchCallOrCallvirt(typeof(Util).GetMethod("CheckRoll", new Type[] { typeof(float), typeof(CharacterMaster) })),
                x => x.MatchStfld<RoR2.Orbs.GenericDamageOrb>("isCrit")))
            {
                for (int i = 0; i < 4; i++)
                {
                    c.Remove();
                }
                c.Emit(OpCodes.Ldarg_1);
                c.Emit(OpCodes.Ldfld, typeof(DamageInfo).GetField("crit"));
            }
            else
            {
                Logger.LogError("Failed to apply Charged Perforator Crit hook");
            }
        }

        private void Changes()
        {
            LanguageAPI.Add("ITEM_lightningStrikeOnHit_NAME".ToUpper(), "Charged Peripherator");
        }
    }
}