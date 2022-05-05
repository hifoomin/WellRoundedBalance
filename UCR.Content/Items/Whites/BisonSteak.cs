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
        public override string InternalPickupToken => "BaseHealth";
        public override bool NewPickup => true;

        public override string PickupText => // (useRegen ? "Regenerate on kill." : "") +
                                             "Gain" +
                                             (HealthFromLevel ? " 30% base health" : "") +
                                             (BaseHealth != 0f && HealthFromLevel ? " +" : "") +
                                             (BaseHealth != 0f ? " " + BaseHealth + " max health" : "") +
                                             ".";

        public override string DescText => // (useRegen ? "Increases <style=cIsHealing>base health regeneration</style> by <style=cIsHealing>" + Regen + "</style>. " : "") +
                                           // (useRegenStack ? "<style=cStack>(+" + Regen + " Per Stack)</style>. " : "") +
                                           "Increases <style=cIsHealing>maximum health</style> by" +
                                           (HealthFromLevel ? " <style=cIsHealing>30%</style> <style=cStack>(+30% per stack)</style>" : "") +
                                           (BaseHealth != 0f && HealthFromLevel ? " +" : "") +
                                           (BaseHealth != 0f ? " <style=cIsHealing>" + BaseHealth + "</style> <style=cStack>(+" + BaseHealth + " per stack)</style>" : "") +
                                           ".";

        public override void Init()
        {
            BaseHealth = ConfigOption(25f, "Health", "Per Stack. Vanilla is 25");
            HealthFromLevel = ConfigOption(false, "Give Health worth a single Level Up?", "Vanilla is false");
            /*
            Regen = Config.Bind<float>(0f, "Regen", "Vanilla is 0");
            StackRegen = Config.Bind<bool>(false, "Stack Regen?", "Vanilla is false");
            */
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeHealth;
            if (HealthFromLevel)
            {
                RecalculateStatsAPI.GetStatCoefficients += AddBehaviorLevelHealth;
            }
            // RecalculateStatsAPI.GetStatCoefficients += ChangeBuffBehavior;
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

        /*
        public static void ChangeBuffBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var mrb = LegacyResourcesAPI.Load<BuffDef>("buffdefs/meatregenboost");
            mrb.canStack = StackRegen;
            if (sender.inventory)
            {
                int buff = sender.GetBuffCount(RoR2Content.Buffs.MeatRegenBoost);
                if (buff > 0)
                {
                    args.baseRegenAdd += 2 * (1 + 0.2f * (sender.level - 1)) * (buff - 1);
                }
            }
        }

        // TODO: Make it add the buff lmao
        */
    }
}