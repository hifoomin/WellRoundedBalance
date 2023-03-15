using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2.Items;
using System;

namespace WellRoundedBalance.Items.Whites
{
    public class BustlingFungus : ItemBase
    {
        public override string Name => ":: Items : Whites :: Bustling Fungus";
        public override ItemDef InternalPickup => RoR2Content.Items.Mushroom;

        public override string PickupText => $"Heal all nearby allies after standing still for 0.5 second.";

        public override string DescText => $"After standing still for <style=cIsHealing>0.5</style> second, create a zone that <style=cIsHealing>heals</style> for " +
            StackDesc(flatHealing, flatHealingStack, init => $"<style=cIsHealing>{init / healingInterval}</style>{{Stack}} ", stack => GetPerSecondStack(flatHealing, stack, flatHealingIsHyperbolic).ToString()) +
            StackDesc(percentHealing, percentHealingStack, init => (flatHealing > 0 || flatHealingStack > 0 ? "plus an additional " : "") + $"<style=cIsHealing>{d(init / healingInterval)}</style>{{Stack}} of <style=cIsHealing>maximum health</style> ", stack => d(GetPerSecondStack(percentHealing, stack, percentHealingIsHyperbolic))) +
            "every second to all allies " + StackDesc(baseRadius, radiusStack, init => $"within <style=cIsHealing>{m(init)}</style>{{Stack}}", m) + ".";

        public static float GetPerSecondStack(float init, float stack, float hyper)
        {
            float a0 = init / healingInterval;
            float a1 = StackAmount(init, stack, 1, hyper) / StackAmount(healingInterval, healingIntervalStack, 1, healingIntervalIsHyperbolic);
            return a1 / a0 - 1f;
        }

        [ConfigField("Base Radius", 13f)]
        public static float baseRadius;

        [ConfigField("Radius Per Stack", 0f)]
        public static float radiusStack;

        [ConfigField("Radius is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float radiusIsHyperbolic;

        [ConfigField("Flat Healing", "Decimal.", 0f)]
        public static float flatHealing;

        [ConfigField("Flat Healing Per Stack", "Decimal.", 0f)]
        public static float flatHealingStack;

        [ConfigField("Flat Healing is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float flatHealingIsHyperbolic;

        [ConfigField("Percent Healing", "Decimal.", 0.0125f)]
        public static float percentHealing;

        [ConfigField("Percent Healing Per Stack", "Decimal.", 0.00625f)]
        public static float percentHealingStack;

        [ConfigField("Percent Healing is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float percentHealingIsHyperbolic;

        [ConfigField("Healing Interval", 0.25f)]
        public static float healingInterval;

        [ConfigField("Healing Interval per Stack", 0f)]
        public static float healingIntervalStack;

        [ConfigField("Healing Interval is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float healingIntervalIsHyperbolic;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.Items.MushroomBodyBehavior.FixedUpdate += MushroomBodyBehavior_FixedUpdate;
        }

        public static void MushroomBodyBehavior_FixedUpdate(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(x => x.MatchCallOrCallvirt<CharacterBody>("get_" + nameof(CharacterBody.radius))) && c.TryGotoNext(x => x.MatchStloc(out _)))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<MushroomBodyBehavior, float>>(self => self.body.radius + StackAmount(baseRadius, radiusStack, self.stack, radiusIsHyperbolic));
            }
            else Logger.LogError("Failed to apply Bustling Fungus Radius hook");
            if (c.TryGotoNext(x => x.MatchStfld<HealingWard>(nameof(HealingWard.interval))))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<MushroomBodyBehavior, float>>(self => StackAmount(healingInterval, healingIntervalStack, self.stack, healingIntervalIsHyperbolic));
            }
            else Logger.LogError("Failed to apply Bustling Fungus Interval hook");
            if (c.TryGotoNext(x => x.MatchStfld<HealingWard>(nameof(HealingWard.healFraction))))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<MushroomBodyBehavior, float>>(self => StackAmount(percentHealing, self.mushroomHealingWard.interval * percentHealingStack, self.stack, percentHealingIsHyperbolic));
            }
            else Logger.LogError("Failed to apply Bustling Fungus Percent Healing hook");
            if (c.TryGotoNext(x => x.MatchStfld<HealingWard>(nameof(HealingWard.healPoints))))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<MushroomBodyBehavior, float>>(self => StackAmount(flatHealing, self.mushroomHealingWard.interval * flatHealingStack, self.stack, flatHealingIsHyperbolic));
            }
            else Logger.LogError("Failed to apply Bustling Fungus Flat Healing hook");
        }
    }
}