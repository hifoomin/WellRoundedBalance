using R2API;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace UltimateCustomRun
{
    public class Infusion : Based
    {
        
        public static float basecap;
        public static float levelcoeff;
        public static float basehealth;
        public static float percenthealth;
        public static bool scaling;
        public static bool stackbase;
        public static bool stackpercent;

        public override string Name => ":: Items :: Greens :: Infusion";
        public override string InternalPickupToken => "infusion";
        public override bool NewPickup => true;

        public static bool iBaseH = basehealth != 0f;
        public static bool iStackBaseH = stackbase;
        public static bool iPercentH = percenthealth != 0f;
        public static bool iStackPercentH = stackpercent;
        public static bool iBothH = iBaseH && iPercentH;
        public static bool iScaling = scaling;
            

        public override string PickupText => "Gain" +
                                             (iPercentH? " " + d(percenthealth) : "") +
                                             (iBothH? " +" : "") +
                                             (iBaseH? " " + basehealth : "") +
                                             " max health. Killing an enemy permanently increases your maximum health, up to 100" +
                                             (iScaling ? ", scales with level" : "") +
                                             ".";

        public override string DescText => "Increases<style=cIsHealing> maximum health</style> by" +
                                            (iPercentH ? " <style=cIsHealing>" + d(percenthealth) + "</style> " : "") +
                                            (iStackPercentH ? "<style=cStack>(+" + d(percenthealth) + " per stack)</style>" : "") +
                                            (iBothH ? " +" : "") +
                                            (iBaseH ? " <style=cIsHealing>" + basehealth + "</style> " : "") +
                                            (iStackBaseH ? "<style=cStack>(+" + basehealth + " per stack)</style>" : "") +
                                            ". Killing an enemy increases your <style=cIsHealing>health permanently</style> by <style=cIsHealing>1</style> <style=cStack>(+1 per stack)</style>, up to a <style=cIsHealing>maximum</style> of <style=cIsHealing>100 <style=cStack>(+100 per stack)</style> health</style>." +
                                            (iScaling ? " Scales with level." : "");

        public override void Init()
        {
            basecap = ConfigOption(30f, "Base Cap", "Vanilla is 0");
            levelcoeff = ConfigOption(0.25f, "Level Scaling Coefficient", "Vanilla is 0");
            basehealth = ConfigOption(0f, "Flat Health", "Vanilla is 0");
            percenthealth = ConfigOption(0f, "Percent Health", "Decimal. Vanilla is 0");
            scaling = ConfigOption(false, "Use Level Scaling Cap?", "Formula:\nBase Cap * 1 + 0.3 * (Level - 1) * Count\nVanilla is false");
            stackbase = ConfigOption(false, "Should Flat Health Stack?", "Vanilla is false");
            stackpercent = ConfigOption(false, "Should Percent Health Stack?", "Vanilla is false");
            base.Init();
        }

        public override void Hooks()
        {
            if (scaling)
            {
                IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeBehavior;
            }
            RecalculateStatsAPI.GetStatCoefficients += BehaviorAddFlatHealth;
            RecalculateStatsAPI.GetStatCoefficients += BehaviorAddPercentHealth;
        }
        public static void ChangeBehavior(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            int bodyLoc = 17;
            int countLoc = 33;
            int capLoc = 47;

            c.GotoNext(MoveType.After,
                x => x.MatchLdsfld("RoR2.RoR2Content/Items", "Infusion"),
                x => x.MatchCallOrCallvirt<RoR2.Inventory>(nameof(RoR2.Inventory.GetItemCount)),
                x => x.MatchStloc(out countLoc)
                );
            c.GotoNext(MoveType.Before,
                x => x.MatchStloc(out capLoc)
                );
            c.Emit(OpCodes.Ldloc, countLoc);
            c.Emit(OpCodes.Ldloc, 13);
            c.EmitDelegate<Func<int, int, RoR2.CharacterBody, int>>((currentInfusionCap, infusionCount, body) =>
            {
                float newInfusionCap = 100 * infusionCount;

                if (body != null)
                {
                    float levelBonus = 1 + levelcoeff * (body.level - 1);

                    newInfusionCap = basecap * levelBonus * infusionCount;
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
                    args.baseHealthAdd += stackbase ? basehealth * stack : basehealth;
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
                    args.healthMultAdd += stackpercent ? percenthealth * stack : percenthealth;
                }
            }
        }
    }
}
