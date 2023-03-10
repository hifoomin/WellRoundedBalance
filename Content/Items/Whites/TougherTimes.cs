using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using System;

namespace WellRoundedBalance.Items.Whites
{
    public class TougherTimes : ItemBase
    {
        public override string Name => ":: Items : Whites :: Tougher Times";
        public override string InternalPickupToken => "bear";

        public override string PickupText => "Chance to block incoming damage.";

        public override string DescText =>
            StackDesc(blockChance, blockChanceStack, init => $"<style=cIsHealing>{d(init)}</style>{{Stack}} chance to <style=cIsHealing>block</style> incoming damage. <style=cIsUtility>Unaffected by luck</style>.", d);

        [ConfigField("Block Chance", "Decimal.", 0.09f)]
        public static float blockChance;

        [ConfigField("Block Chance per Stack", "Decimal.", 0.09f)]
        public static float blockChanceStack;

        [ConfigField("Damage is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 1f)]
        public static float blockChanceIsHyperbolic;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += HealthCompoment_TakeDamage;
        }

        public static void HealthCompoment_TakeDamage(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(x => x.MatchLdfld<HealthComponent.ItemCounts>(nameof(HealthComponent.ItemCounts.bear))) && c.TryGotoNext(x => x.MatchLdcR4(0), x => x.MatchLdnull(), x => x.MatchCallOrCallvirt(typeof(Util), nameof(Util.CheckRoll))))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<HealthComponent, float>>(self => StackAmount(blockChance, blockChanceStack, self.itemCounts.bear, blockChanceIsHyperbolic));
            }
            else Main.WRBLogger.LogError("Failed to apply Tougher Times Block hook");
        }
    }
}