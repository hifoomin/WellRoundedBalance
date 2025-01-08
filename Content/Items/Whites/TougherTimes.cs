﻿using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Whites
{
    public class TougherTimes : ItemBase<TougherTimes>
    {
        public override string Name => ":: Items : Whites :: Tougher Times";
        public override ItemDef InternalPickup => RoR2Content.Items.Bear;

        public override string PickupText => "Chance to block incoming damage.";

        public override string DescText =>
            StackDesc(blockChance, blockChanceStack, init => $"<style=cIsHealing>{d(init)}</style>{{Stack}} chance to <style=cIsHealing>block</style> incoming damage. <style=cIsUtility>Unaffected by luck</style>.", d);

        [ConfigField("Block Chance", "Decimal.", 0.1f)]
        public static float blockChance;

        [ConfigField("Block Chance per Stack", "Decimal.", 0.1f)]
        public static float blockChanceStack;

        [ConfigField("Damage is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 1f)]
        public static float blockChanceIsHyperbolic;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamageProcess += HealthCompoment_TakeDamageProcess;
        }

        public static void HealthCompoment_TakeDamageProcess(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(x => x.MatchLdfld<HealthComponent.ItemCounts>(nameof(HealthComponent.ItemCounts.bear))) && c.TryGotoNext(x => x.MatchLdcR4(0), x => x.MatchLdnull(), x => x.MatchCallOrCallvirt(typeof(Util), nameof(Util.CheckRoll))))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<HealthComponent, float>>(self => StackAmount(blockChance, blockChanceStack, self.itemCounts.bear, blockChanceIsHyperbolic) * 100);
            }
            else Logger.LogError("Failed to apply Tougher Times Block hook");
        }
    }
}