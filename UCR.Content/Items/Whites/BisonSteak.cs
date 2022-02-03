using R2API;
using RoR2;
using UnityEngine;
using MonoMod.Cil;
using UnityEngine.Networking;

namespace UltimateCustomRun
{
    public class BisonSteak : Based
    {
        public static float flathealth;
        public static bool levelhealth;
        public static float regen;
        public static bool regenstack;
        public override string Name => ":: Items : Whites :: Bison Steak";
        public override string InternalPickupToken => "flathealth";
        public override bool NewPickup => true;

        public static bool useFlatHealthSteak = flathealth != 0f;
        public static bool useLevelHealthSteak = levelhealth;
        public static bool useBothHealthSteak = useFlatHealthSteak && useLevelHealthSteak;
        public static bool useRegen = /* regen != 0f; */ false;
        public static bool useRegenStack = /* regenstack; */ false;

        public override string PickupText => (useRegen ? "Regenerate on kill." : "") +
                                             "Gain" +
                                             (useLevelHealthSteak ? " 30% base health" : "") +
                                             (useBothHealthSteak ? " +" : "") +
                                             (useFlatHealthSteak ? " " + flathealth + " max health" : "") +
                                             ".";
        public override string DescText => (useRegen ? "Increases <style=cIsHealing>base health regeneration</style> by <style=cIsHealing>" + regen + "</style>. " : "") +
                                           (useRegenStack ? "<style=cStack>(+" + regen + " Per Stack)</style>. " : "") +
                                           "Increases <style=cIsHealing>base health</style> by" +
                                           (useLevelHealthSteak ? " <style=cIsHealing>30%</style> <style=cStack>(+30% per stack)</style>" : "") +
                                           (useBothHealthSteak ? " +" : "") +
                                           (useFlatHealthSteak ? " <style=cIsHealing>" + flathealth + "</style> <style=cStack>(+" + flathealth + " per stack)</style>" : "") +
                                           ".";


        public override void Init()
        {
            flathealth = ConfigOption(25f, "Health", "Per Stack. Vanilla is 25");
            levelhealth = ConfigOption(false, "Give Health worth a single Level Up?", "Vanilla is false");
            /*
            regen = Config.Bind<float>(0f, "Regen", "Vanilla is 0");
            regenstack = Config.Bind<bool>(false, "Stack Regen?", "Vanilla is false");
            */
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeHealth;
            if (levelhealth)
            {
                RecalculateStatsAPI.GetStatCoefficients += AddBehaviorLevelHealth;
            }
            // RecalculateStatsAPI.GetStatCoefficients += ChangeBuffBehavior;
        }
        public static void ChangeHealth(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(25f)
            );
            c.Index += 1;
            c.Next.Operand = flathealth;
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
        public static void ChangeBuffBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var mrb = Resources.Load<BuffDef>("buffdefs/meatregenboost");
            mrb.canStack = regenstack ? true : false;
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
    }
}
