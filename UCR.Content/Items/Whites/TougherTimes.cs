using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun.Items.Whites
{
    public class TougherTimes : ItemBase
    {
        public static float BlockChance;
        public static float Armor;
        public static bool StackArmor;

        public override string Name => ":: Items : Whites :: Tougher Times";
        public override string InternalPickupToken => "bear";
        public override bool NewPickup => true;

        public override string PickupText => (BlockChance != 0f ? "Chance to block incoming damage." : "") +
                                             (Armor != 0f ? " Reduce incoming damage." : "");

        public override string DescText => (BlockChance != 0f ? "<style=cIsHealing>" + BlockChance + "%</style> <style=cStack>(+" + BlockChance + "% per stack)</style> Chance to <style=cIsHealing>block</style> incoming damage. " : "") +
                                           (Armor != 0f ? "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>" + Armor + "</style> " +
                                           (StackArmor ? "<style=cStack>(+" + Armor + " per stack)</style>. " : "") : "") +
                                           "<style=cIsUtility>Unaffected by luck</style>.";

        public override void Init()
        {
            BlockChance = ConfigOption(15f, "Block Chance", "Per Stack. Vanilla is 15");
            Armor = ConfigOption(0f, "Armor", "Vanilla is 0");
            StackArmor = ConfigOption(false, "Stack Armor?", "Vanilla is false");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += ChangeBlock;
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
        }

        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.Bear);
                if (stack > 0)
                {
                    args.armorAdd += (StackArmor ? Armor * stack : Armor);
                }
            }
        }

        public static void ChangeBlock(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                 x => x.MatchLdcI4(0),
                 x => x.Match(OpCodes.Ble_S),
                 x => x.MatchLdcR4(15f)
            );
            c.Index += 2;
            c.Next.Operand = BlockChance;
        }
    }
}