using MonoMod.Cil;
using R2API;
using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Items.Whites
{
    public class BisonSteak : ItemBase
    {
        public static float BaseHealth;
        public static bool HealthFromLevel;
        public static float Regen;
        public static bool StackRegen;
        public override string Name => ":: Items : Whites :: Bison Steak";
        public override string InternalPickupToken => "flatHealth";
        public override bool NewPickup => true;

        public override string PickupText => "Gain" +
                                             (HealthFromLevel ? " 30% base health" : "") +
                                             (BaseHealth != 0f && HealthFromLevel ? " +" : "") +
                                             (BaseHealth != 0f ? " " + BaseHealth + " max health" : "") +
                                             ".";

        public override string DescText => "Increases <style=cIsHealing>maximum health</style> by" +
                                           (HealthFromLevel ? " <style=cIsHealing>30%</style> <style=cStack>(+30% per stack)</style>" : "") +
                                           (BaseHealth != 0f && HealthFromLevel ? " +" : "") +
                                           (BaseHealth != 0f ? " <style=cIsHealing>" + BaseHealth + "</style> <style=cStack>(+" + BaseHealth + " per stack)</style>" : "") +
                                           ".";

        public override void Init()
        {
            BaseHealth = ConfigOption(25f, "Health", "Per Stack. Vanilla is 25");
            ROSOption("Whites", 0f, 50f, 5f, "1");
            HealthFromLevel = ConfigOption(false, "Give Health worth a single Level Up?", "Vanilla is false");
            ROSOption("Whites", 0f, 5f, 0.01f, "1");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeHealth;
            if (HealthFromLevel)
            {
                RecalculateStatsAPI.GetStatCoefficients += AddBehaviorLevelHealth;
            }
        }

        public static void ChangeHealth(ILContext il)
        {
            ILCursor c = new(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(25f)
            );
            c.Index += 1;
            c.Next.Operand = BaseHealth;
        }

        public static void AddBehaviorLevelHealth(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.FlatHealth);
                if (stack > 0)
                {
                    args.baseHealthAdd += sender.levelMaxHealth * stack;
                }
            }
        }
    }
}