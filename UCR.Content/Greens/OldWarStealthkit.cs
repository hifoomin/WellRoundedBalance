using MonoMod.Cil;
using RoR2;
using R2API;

namespace UltimateCustomRun
{
    public static class OldWarStealthkit
    {
        public static void ChangeCooldown(ILContext il)
        {
            
        }

        public static void ChangeDuration(ILContext il)
        {

        }
        public static void ChangeBuff(ILContext il)
        {

        }

        // i feel like these dont really matter, the item seems fine with a slightly higher threshold?
        // i was thinking of an armor buff so that blazings and overloadings later on dont one shot you lmao

        // just realized that the buff is shared with bandit, cant really think of a better workaround than the one below

        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.Phasing);
                var buffp1 = sender.HasBuff(RoR2.RoR2Content.Buffs.Cloak);
                var buffp2 = sender.HasBuff(RoR2.RoR2Content.Buffs.CloakSpeed);
                // periphery 1 sucks, periphery 2 best cry about it
                if (stack > 0 && buffp1 && buffp2)
                {
                    if (Main.OldWarArmorStack.Value)
                    {
                        args.armorAdd += Main.OldWarArmor.Value * stack;
                    }
                    else
                    {
                        args.armorAdd += Main.OldWarArmor.Value;
                    }
                }
            }
        }

    }
}
