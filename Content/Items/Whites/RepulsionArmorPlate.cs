using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Whites
{
    public class RepulsionArmorPlate : ItemBase
    {
        public override string Name => ":: Items : Whites :: Repulsion Armor Plate";
        public override string InternalPickupToken => "repulsionArmorPlate";

        public override string PickupText => "Receive flat damage reduction from all attacks.";

        public override string DescText => StackDesc(flatDamageReduction, flatDamageReductionStack,
            init => $"Reduce all <style=cIsDamage>incoming damage</style> by <style=cIsDamage>{init}</style>{{Stack}}. Cannot be reduced below <style=cIsDamage>{minimumDamage}</style>.",
            stack => stack.ToString());

        [ConfigField("Flat Damage Reduction", "", 5f)]
        public static float flatDamageReduction;

        [ConfigField("Flat Damage Reduction per Stack", "", 5f)]
        public static float flatDamageReductionStack;

        [ConfigField("Minimum Damage", "", 8f)]
        public static float minimumDamage;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += Changes;
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(x => x.MatchLdfld<HealthComponent.ItemCounts>(nameof(HealthComponent.ItemCounts.armorPlate))) && c.TryGotoNext(x => x.MatchStloc(6)))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldloc, 6);
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, HealthComponent, float>>((orig, self) => Mathf.Max(minimumDamage, StackAmount(flatDamageReduction, flatDamageReductionStack, self.itemCounts.armorPlate)));
            }
            else Main.WRBLogger.LogError("Failed to apply Repulsion Armor Plate hook");
        }
    }
}