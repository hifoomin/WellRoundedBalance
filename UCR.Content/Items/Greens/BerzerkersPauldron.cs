using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using R2API.Utils;
using RoR2;

namespace UltimateCustomRun.Items.Greens
{
    public class BerzerkersPauldron : ItemBase
    {
        public static float Armor;
        public static float BuffArmor;
        public static float BaseBuffDuration;
        public static float StackBuffDuration;
        public static int KillRequirement;

        public override string Name => ":: Items :: Greens :: Berzerkers Pauldron";
        public override string InternalPickupToken => "warCryOnMultiKill";
        public override bool NewPickup => false;

        // remember to Change when KillRequirement works
        public override string PickupText => "";

        // "Enter a frenzy after killing " + KillRequirement + " enemies in quick succession.");

        public override string DescText => (Armor != 0f ? "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>" + Armor + "</style> <style=cStack>(+" + Armor + " per stack)</style>. " : "") +
                                           "<style=cIsDamage>Killing " + KillRequirement + " enemies</style> within <style=cIsDamage>1</style> second sends you into a <style=cIsDamage>frenzy</style> for <style=cIsDamage>" + BaseBuffDuration + "s</style> <style=cStack>(+" + StackBuffDuration + "s per stack)</style>. Increases <style=cIsUtility>movement speed</style> by <style=cIsUtility>50%</style>, <style=cIsDamage>attack speed</style> by <style=cIsDamage>100%</style>" +
                                           (BuffArmor != 0f ? " and <style=cIsHealing>armor</style> by <style=cIsHealing>" + BuffArmor + "</style>." : "");

        public override void Init()
        {
            Armor = ConfigOption(0f, "Unconditional Armor", "Per Stack. Vanilla is 0");
            BuffArmor = ConfigOption(0f, "Buff Armor", "Vanilla is 0");
            BaseBuffDuration = ConfigOption(6f, "Base Buff Duration", "Vanilla is 6");
            StackBuffDuration = ConfigOption(4f, "Stack Buff Duration", "Per Stack. Vanilla is 4");
            KillRequirement = ConfigOption(4, "Buff Kill Requirement", "Vanilla is 4");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.AddMultiKill += ChangeKillCount;
            IL.RoR2.CharacterBody.AddMultiKill += ChangeBuffDuration;
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
        }

        public static void ChangeKillCount(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt(typeof(CharacterBody).GetPropertyGetter(nameof(CharacterBody.multiKillCount))),
                x => x.MatchLdcI4(4)
            );
            c.Index += 1;
            c.Remove();
            c.Emit(OpCodes.Ldc_I4, KillRequirement);
        }

        public static void ChangeBuffDuration(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.RoR2Content/Buffs", "WarCryBuff"),
                x => x.MatchLdcR4(2f),
                x => x.MatchLdcR4(4f)
            );
            c.Index += 1;
            c.Next.Operand = BaseBuffDuration - StackBuffDuration;
            c.Index += 1;
            c.Next.Operand = StackBuffDuration;
        }

        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var buff = sender.HasBuff(RoR2Content.Buffs.WarCryBuff);
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.WarCryOnMultiKill);
                if (stack > 0 && buff)
                {
                    args.armorAdd += BuffArmor;
                }
                if (stack > 0)
                {
                    args.armorAdd += Armor;
                }
            }
        }

        // TODO: Ask Moffein for his Pauldron changes and implement them :plead
    }
}