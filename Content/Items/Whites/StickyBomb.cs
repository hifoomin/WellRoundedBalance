using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Whites
{
    public class StickyBomb : ItemBase
    {
        public override string Name => ":: Items : Whites :: Sticky Bomb";
        public override string InternalPickupToken => "stickyBomb";

        public override string PickupText => "Chance on hit to attach a bomb to enemies.";

        public override string DescText =>
            StackDesc(chance, chanceStack, init => $"<style=cIsDamage>{d(init)}</style>{{Stack}} chance on hit to attach a <style=cIsDamage>bomb</style> to an enemy", d) +
            StackDesc(damage, damageStack, init => $", detonating for <style=cIsDamage>{init}</style>{{Stack}} {(damageIsTotal ? "TOTAL" : "base")} damage.", d);

        // Better Configs Wave 2 Template -P

        [ConfigField("Chance", "Decimal.", 0.05f)]
        public static float chance;

        [ConfigField("Chance per Stack", "Decimal.", 0.05f)]
        public static float chanceStack;

        [ConfigField("Chance is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float chanceIsHyperbolic;

        [ConfigField("Damage Coefficient", "Decimal.", 1.8f)]
        public static float damage;

        [ConfigField("Damage Coefficient per Stack", "Decimal.", 0f)]
        public static float damageStack;

        [ConfigField("Damage is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float damageIsHyperbolic;

        [ConfigField("Damage is TOTAL", true)]
        public static bool damageIsTotal;

        [ConfigField("Lifetime", "Decimal.", 1f)]
        public static float lifetime;

        [ConfigField("Change Falloff", BlastAttack.FalloffModel.None)]
        public static BlastAttack.FalloffModel changeFalloff;

        [ConfigField("Proc Coefficient", "Decimal.", 0f)]
        public static float proc;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            Changes();
        }

        public static void GlobalEventManager_OnHitEnemy(ILContext il)
        {
            ILCursor c = new(il);
            int info = -1;
            int attacker = -1;
            c.TryGotoNext(x => x.MatchLdarg(out info), x => x.MatchLdfld<DamageInfo>(nameof(DamageInfo.attacker)));
            c.TryGotoNext(x => x.MatchStloc(attacker));
            if (info == -1 || attacker == -1) return;
            int stack = GetItemLoc(c, nameof(RoR2Content.Items.StickyBomb));
            if (stack != -1 && c.TryGotoNext(x => x.MatchCallOrCallvirt(typeof(Util), nameof(Util.CheckRoll))))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldloc, stack);
                c.Emit(OpCodes.Ldarg, info);
                c.EmitDelegate<Func<int, DamageInfo, float>>((stack, info) => info.procCoefficient * StackAmount(chance, chanceStack, stack, chanceIsHyperbolic));
            }
            else Main.WRBLogger.LogError("Failed to apply Sticky Bomb Chance hook");
            if (c.TryGotoNext(x => x.MatchStloc(79)))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldarg, info);
                c.Emit(OpCodes.Ldloc, attacker);
                c.Emit(OpCodes.Ldloc, stack);
                c.EmitDelegate<Func<DamageInfo, CharacterBody, int, float>>((info, body, stack) =>
                {
                    float ret = StackAmount(damage, damageStack, stack, damageIsHyperbolic);
                    if (damageIsTotal) ret = Util.OnHitProcDamage(info.damage, body.damage, ret);
                    return ret * 100;
                });
            }
            else Main.WRBLogger.LogError("Failed to apply Sticky Bomb Damage hook");
        }

        public static void Changes()
        {
            var StickyBombImpact = Utils.Paths.GameObject.StickyBomb1.Load<GameObject>().GetComponent<ProjectileImpactExplosion>();
            StickyBombImpact.lifetime = lifetime;
            StickyBombImpact.falloffModel = changeFalloff;
            StickyBombImpact.blastProcCoefficient = proc;
        }
    }
}