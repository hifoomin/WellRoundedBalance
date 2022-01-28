using R2API;
using RoR2;
using UnityEngine;
using MonoMod.Cil;
using UnityEngine.Networking;

namespace UltimateCustomRun
{
    public static class BisonSteak
    {
        public static void ChangeHealth(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(25f)
            );
            c.Index += 1;
            c.Next.Operand = Main.BisonSteakHealth.Value;
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
        public static void AddBehaviorRegen(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {

        }
        public static void ChangeBuffBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var mrb = Resources.Load<BuffDef>("buffdefs/meatregenboost");
            mrb.canStack = Main.BisonSteakRegenStack.Value ? true : false;
            if (sender.inventory)
            {
                int buff = sender.GetBuffCount(RoR2Content.Buffs.MeatRegenBoost);
                if (buff > 0)
                {
                    args.baseRegenAdd += 2 * (1 + 0.2f * (sender.level - 1)) * (buff - 1);
                }
            }
        }
    }
}
