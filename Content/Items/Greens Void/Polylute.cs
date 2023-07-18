using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2.Orbs;
using System;

namespace WellRoundedBalance.Items.VoidGreens
{
    public class Polylute : ItemBase<Polylute>
    {
        public override string Name => ":: Items :::::: Voids :: Polylute";
        public override ItemDef InternalPickup => DLC1Content.Items.ChainLightningVoid;

        public override string PickupText => "Chance to repeatedly strike a single enemy with lightning. <style=cIsVoid>Corrupts all Ukuleles</style>.";

        public override string DescText =>
            StackDesc(chance, chanceStack, init => $"<style=cIsDamage>{d(init)}</style>{{Stack}} chance to fire <style=cIsDamage>lightning</style>", d) +
            StackDesc(damage, damageStack, init => $" for <style=cIsDamage>{d(init)}</style>{{Stack}} {(damageIsTotal ? "TOTAL" : "base")} damage", d) +
            StackDesc(strikeCount, strikeCountPerStack, init => $" up to <style=cIsDamage>{init}</style>{{Stack}} times") + ". <style=cIsVoid>Corrupts all Ukuleles</style>.";

        [ConfigField("Damage Coefficient", "Decimal.", 0.4f)]
        public static float damage;

        [ConfigField("Damage Coefficient per Stack", "Decimal.", 0f)]
        public static float damageStack;

        [ConfigField("Damage is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float damageIsHyperbolic;

        [ConfigField("Damage is TOTAL", true)]
        public static bool damageIsTotal;

        [ConfigField("Chance", "Decimal.", 0.25f)]
        public static float chance;

        [ConfigField("Chance per Stack", "Decimal.", 0f)]
        public static float chanceStack;

        [ConfigField("Chance is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float chanceIsHyperbolic;

        [ConfigField("Base Strike Count", "", 3)]
        public static int strikeCount;

        [ConfigField("Strike Count Per Stack", "", 2)]
        public static int strikeCountPerStack;

        [ConfigField("Strike Interval", "Decimal.", 0.1f)]
        public static float interval;

        [ConfigField("Strike Interval per Stack", "Decimal.", 0f)]
        public static float intervalStack;

        [ConfigField("Strike Interval is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float intervalIsHyperbolic;

        [ConfigField("Proc Coefficient", 0.1f)]
        public static float procCoefficient;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        }

        private void GlobalEventManager_OnHitEnemy(ILContext il)
        {
            ILCursor c = new(il);

            int stack = GetItemLoc(c, nameof(DLC1Content.Items.ChainLightningVoid));
            int idx = c.Index;
            int dmg = -1;
            if (c.TryGotoNext(x => x.MatchLdloc(out dmg), x => x.MatchCallOrCallvirt(typeof(Util), nameof(Util.OnHitProcDamage))) && c.TryGotoPrev(x => x.MatchStloc(dmg)))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldloc, stack);
                c.EmitDelegate<Func<int, float>>(stack => StackAmount(damage, damageStack, stack, damageIsHyperbolic));
                c.GotoNext(MoveType.After, x => x.MatchCallOrCallvirt(typeof(Util), nameof(Util.OnHitProcDamage)));
                c.Emit(OpCodes.Ldloc, dmg);
                c.EmitDelegate<Func<float, float, float>>((t, f) => damageIsTotal ? t : f);
            }
            else Logger.LogError("Failed to apply Polylute Damage hook");
            c.Index = idx;
            if (c.TryGotoNext(x => x.MatchStfld<VoidLightningOrb>(nameof(VoidLightningOrb.totalStrikes))))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldloc, stack);
                c.EmitDelegate<Func<int, int>>(stack => (int)StackAmount(strikeCount, strikeCountPerStack, stack));
            }
            else Logger.LogError("Failed to apply Polylute Hit Count hook");
            c.Index = idx;
            if (c.TryGotoNext(x => x.MatchStfld<VoidLightningOrb>(nameof(VoidLightningOrb.secondsPerStrike))))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldloc, stack);
                c.EmitDelegate<Func<int, float>>(stack => StackAmount(interval, intervalStack, stack, intervalIsHyperbolic));
            }
            else Logger.LogError("Failed to apply Polylute Hit Interval hook");
            c.Index = idx;
            if (c.TryGotoNext(x => x.MatchStfld<VoidLightningOrb>(nameof(VoidLightningOrb.procCoefficient))))
            {
                c.Emit(OpCodes.Pop);
                c.EmitDelegate(() => procCoefficient * globalProc);
            }
            else Logger.LogError("Failed to apply Polylute Proc Coefficient hook");
            c.Index = idx;
            int ch = -1;
            if (c.TryGotoNext(x => x.MatchCallOrCallvirt(typeof(Util), nameof(Util.CheckRoll))) && c.TryGotoPrev(x => x.MatchMul()) && c.TryGotoPrev(x => x.MatchLdloc(out ch)) && c.TryGotoPrev(x => x.MatchStloc(ch)))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldloc, stack);
                c.EmitDelegate<Func<int, float>>(stack => StackAmount(chance, chanceStack, stack, chanceIsHyperbolic) * 100);
            }
            else Logger.LogError("Failed to apply Polylute Chance hook");
        }
    }
}