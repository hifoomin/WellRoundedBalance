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
        public static void AddBehaviorRegen(DamageReport report)
        {
            if (Main.BisonSteakRegenStack.Value)
            {
                if (NetworkServer.active)
                {
                    var stack = report.attackerBody.inventory.GetItemCount(RoR2Content.Items.FlatHealth);
                    if (report.attacker && report.attackerBody && report.attackerBody.inventory && stack > 0)
                    {
                        report.attackerBody.AddTimedBuff(RoR2Content.Buffs.MeatRegenBoost, Main.BisonSteakRegen.Value);
                    }
                }
            }
        }
        public static void ChangeBuffBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var mrb = Resources.Load<BuffDef>("buffdefs/meatregenboost");
            mrb.canStack = true;
            if (sender.inventory)
            {
                var buff = sender.GetBuffCount(RoR2Content.Buffs.MeatRegenBoost);
                if (buff > 0)
                {
                    args.baseRegenAdd += Main.BisonSteakRegen.Value * buff;
                }
            }
        }
    }
}
