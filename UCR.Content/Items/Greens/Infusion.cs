using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using System;

namespace UltimateCustomRun.Items.Greens
{
    public class Infusion : ItemBase
    {
        // ////////////
        //
        // Thanks to Borbo
        //
        // ///////////////

        public static float BaseCap;
        public static float LevelCoefficient;
        public static float BaseHealth;
        public static float PercentHealth;
        public static bool Scaling;
        public static bool StackBase;
        public static bool StackPercent;

        public override string Name => ":: Items :: Greens :: Infusion";
        public override string InternalPickupToken => "infusion";
        public override bool NewPickup => true;

        public override string PickupText => "Gain" +
                                             (PercentHealth != 0f ? " " + d(PercentHealth) : "") +
                                             (BaseHealth != 0f && PercentHealth != 0f ? " +" : "") +
                                             (BaseHealth != 0f ? " " + BaseHealth : "") +
                                             " max health. Killing an enemy permanently increases your maximum health, up to 100" +
                                             (Scaling ? ", scales with level" : "") +
                                             ".";

        public override string DescText => "Increases<style=cIsHealing> maximum health</style> by" +
                                            (PercentHealth != 0f ? " <style=cIsHealing>" + d(PercentHealth) + "</style> " : "") +
                                            (StackPercent ? "<style=cStack>(+" + d(PercentHealth) + " per stack)</style>" : "") +
                                            (BaseHealth != 0f && PercentHealth != 0f ? " +" : "") +
                                            (BaseHealth != 0f ? " <style=cIsHealing>" + BaseHealth + "</style> " : "") +
                                            (StackBase ? "<style=cStack>(+" + BaseHealth + " per stack)</style>" : "") +
                                            ". Killing an enemy increases your <style=cIsHealing>health permanently</style> by <style=cIsHealing>1</style> <style=cStack>(+1 per stack)</style>, up to a <style=cIsHealing>maximum</style> of <style=cIsHealing>100 <style=cStack>(+100 per stack)</style> health</style>." +
                                            (Scaling ? " Scales with level." : "");

        public override void Init()
        {
            BaseCap = ConfigOption(30f, "Base Cap", "Vanilla is 0");
            ROSOption("Greens", 0f, 200f, 5f, "2");
            LevelCoefficient = ConfigOption(0.25f, "Level Scaling Coefficient", "Vanilla is 0");
            ROSOption("Greens", 0f, 1f, 0.01f, "2");
            BaseHealth = ConfigOption(0f, "Flat Health", "Vanilla is 0");
            ROSOption("Greens", 0f, 200f, 5f, "2");
            PercentHealth = ConfigOption(0f, "Percent Health", "Decimal. Vanilla is 0");
            ROSOption("Greens", 0f, 1f, 0.02f, "2");
            Scaling = ConfigOption(false, "Use Level Scaling Cap?", "Formula:\nBase Cap * 1 + 0.3 * (Level - 1) * Count\nVanilla is false");
            ROSOption("Greens", 0f, 10f, 1f, "2");
            StackBase = ConfigOption(false, "Should Flat Health Stack?", "Vanilla is false");
            ROSOption("Greens", 0f, 10f, 1f, "2");
            StackPercent = ConfigOption(false, "Should Percent Health Stack?", "Vanilla is false");
            ROSOption("Greens", 0f, 10f, 1f, "2");
            base.Init();
        }

        public override void Hooks()
        {
            if (Scaling)
            {
                IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeBehavior;
            }
            RecalculateStatsAPI.GetStatCoefficients += BehaviorAddFlatHealth;
            RecalculateStatsAPI.GetStatCoefficients += BehaviorAddPercentHealth;
        }

        public static void ChangeBehavior(ILContext il)
        {
            ILCursor c = new(il);

            //int bodyLoc = 17;
            int countLoc = 43;
            int capLoc = 63;

            c.GotoNext(MoveType.After,
                x => x.MatchLdsfld("RoR2.RoR2Content/Items", "Infusion"),
                x => x.MatchCallOrCallvirt<Inventory>(nameof(Inventory.GetItemCount)),
                x => x.MatchStloc(out countLoc)
                );
            c.GotoNext(MoveType.Before,
                x => x.MatchStloc(out capLoc)
                );
            c.Emit(OpCodes.Ldloc, countLoc);
            c.Emit(OpCodes.Ldloc, 15);
            // Ldloc here is infusionOrb.target = Util.FindBodyMainHurtBox(attackerBody);
            c.EmitDelegate<Func<int, int, CharacterBody, int>>((currentInfusionCap, infusionCount, body) =>
            {
                float newInfusionCap = 100 * infusionCount;

                if (body != null)
                {
                    float levelBonus = 1 + LevelCoefficient * (body.level - 1);

                    newInfusionCap = BaseCap * levelBonus * infusionCount;
                }

                return (int)newInfusionCap;
            });
        }

        public static void BehaviorAddFlatHealth(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.Infusion);
                if (stack > 0)
                {
                    args.baseHealthAdd += StackBase ? BaseHealth * stack : BaseHealth;
                }
            }
        }

        public static void BehaviorAddPercentHealth(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.Infusion);
                if (stack > 0)
                {
                    args.healthMultAdd += StackPercent ? PercentHealth * stack : PercentHealth;
                }
            }
        }
    }
}