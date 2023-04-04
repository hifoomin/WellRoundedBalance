using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Yellows
{
    public class MoltenPerforator : ItemBase<MoltenPerforator>
    {
        public override string Name => ":: Items :::: Yellows :: Molten Perforator";
        public override ItemDef InternalPickup => RoR2Content.Items.FireballsOnHit;

        public override string PickupText => "Chance on hit to fire magma balls.";

        public override string DescText => "<style=cIsDamage>10%</style> chance on hit to call forth <style=cIsDamage>3 magma balls</style> from an enemy, dealing <style=cIsDamage>" + d(baseTotalDamage) + "</style> <style=cStack>(+" + d(totalDamagePerStack) + " per stack)</style> TOTAL damage.";

        [ConfigField("Base TOTAL Damage", "Decimal.", 2.2f)]
        public static float baseTotalDamage;

        [ConfigField("TOTAL Damage Per Stack", "Decimal.", 1.1f)]
        public static float totalDamagePerStack;

        [ConfigField("Proc Chance", 0f)]
        public static float procChance;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += Changes;
            ChangeProcCoefficient();
            Changes();
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            float initialDamage = baseTotalDamage - totalDamagePerStack;

            bool error = true;

            if (c.TryGotoNext(x => x.MatchLdsfld(typeof(RoR2Content.Items), "FireballsOnHit")) &&
            c.TryGotoNext(x => x.MatchLdfld<DamageInfo>("damage")))
            {
                c.Index -= 6;
                c.Next.Operand = totalDamagePerStack;
                c.Index += 4;
                c.EmitDelegate<Func<float, float>>((damageCoefficient) =>
                {
                    return damageCoefficient + initialDamage;
                });
                error = false;
            }
            if (error)
            {
                Logger.LogError("Failed to apply Molten Perforator hook");
            }
        }

        private void ChangeProcCoefficient()
        {
            var m = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/firemeatball").GetComponent<ProjectileController>();
            m.procCoefficient = procChance * globalProc;
        }

        private void Changes()
        {
            LanguageAPI.Add("ITEM_fireballsOnHit_NAME".ToUpper(), "Molten Peripherator");
        }
    }
}