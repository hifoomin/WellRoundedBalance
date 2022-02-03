using R2API;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class TougherTimes : Based
    {
        public static float blockchance;
        public static float armor;
        public static bool armorstack;

        public override string Name => ":: Items : Whites :: Tougher Times";
        public override string InternalPickupToken => "bear";
        public override bool NewPickup => true;

        public static bool tArmor = armor != 0f;
        public static bool tBlock = blockchance != 0f;

        public override string PickupText => (tBlock ? "Chance to block incoming damage." : "") +
                                             (tArmor ? " Reduce incoming damage." : "");
        public override string DescText => (tBlock ? "<style=cIsHealing>" + blockchance + "%</style> <style=cStack>(+" + blockchance + "% per stack)</style> chance to <style=cIsHealing>block</style> incoming damage. " : "") +
                                           (tArmor ? "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>" + armor + "</style> " +
                                           (armorstack ? "<style=cStack>(+" + armor + " per stack)</style>. " : "") : "") +
                                           "<style=cIsUtility>Unaffected by luck</style>.";
        public override void Init()
        {
            blockchance = ConfigOption(15f, "Block Chance", "Per Stack. Vanilla is 15");
            armor = ConfigOption(0f, "Armor", "Vanilla is 0");
            armorstack = ConfigOption(false, "Stack Armor?", "Vanilla is false");
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
                    args.armorAdd += (armorstack ? armor * stack : armor);
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
            c.Next.Operand = blockchance;
        }
    }
}
