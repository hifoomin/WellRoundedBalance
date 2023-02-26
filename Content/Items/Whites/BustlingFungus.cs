using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2.Items;
using System;

namespace WellRoundedBalance.Items.Whites
{
    public class BustlingFungus : ItemBase
    {
        public override string Name => ":: Items : Whites :: Bustling Fungus";
        public override string InternalPickupToken => "mushroom";

        public override string PickupText => $"Heal all nearby allies after standing still for {s(standingStillDuration, "second")}.";

        public override string DescText => $"After standing still for <style=cIsHealing>{s(standingStillDuration, "</style> second")}, create a zone" + StackDesc(baseHealing, healingPerStack,
                init => $"that <style=cIsHealing>heals</style> for <style=cIsHealing>{d(init)}</style>{{Stack}} of your <style=cIsHealing>health</style> every second to all allies",
                stack => d(stack)) + StackDesc(baseRadius, radiusPerStack,
                    init => $"within <style=cIsHealing>{init}m</style>{{Stack}}.",
                    stack => stack + "m");

        [ConfigField("Base Radius", "", 13f)]
        public static float baseRadius;

        [ConfigField("Radius Per Stack", "", 0f)]
        public static float radiusPerStack;

        [ConfigField("Base Healing", "Decimal.", 0.05f)]
        public static float baseHealing;

        [ConfigField("Healing Per Stack", "Decimal.", 0.025f)]
        public static float healingPerStack;

        [ConfigField("Standing Still Duration", "", 0.5f)]
        public static float standingStillDuration;

        [ConfigField("Healing Interval", "", 0.25f)]
        public static float healingInterval;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.Items.MushroomBodyBehavior.FixedUpdate += Changes;
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(x => x.MatchStloc(2)))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldloc_0);
                c.EmitDelegate<Func<MushroomBodyBehavior, int, float>>((self, stack) => self.body.radius + StackAmount(baseRadius, radiusPerStack, stack));
            }
            else Main.WRBLogger.LogError("Failed to apply Bustling Fungus Radius hook");
            if (c.TryGotoNext(x => x.MatchStfld<HealingWard>(nameof(HealingWard.interval))))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldc_R4, healingInterval); // not operanding for compat
            }
            else Main.WRBLogger.LogError("Failed to apply Bustling Fungus Interval hook");
            if (c.TryGotoNext(x => x.MatchStfld<HealingWard>(nameof(HealingWard.healFraction))))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldloc_0);
                c.EmitDelegate<Func<MushroomBodyBehavior, int, float>>((self, stack) => StackAmount(baseHealing, self.mushroomHealingWard.interval * healingPerStack, stack));
            }
            else Main.WRBLogger.LogError("Failed to apply Bustling Fungus Healing hook");
        }
    }
}