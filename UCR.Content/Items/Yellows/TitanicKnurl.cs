using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun.Items.Yellows
{
    public class TitanicKnurl : ItemBase
    {
        public static float FlatHealth;
        public static float PercentHealth;
        public static bool StackPercentHealth;
        public static float Regen;
        public static float Armor;
        public static bool StackArmor;

        public override string Name => ":: Items :::: Yellows :: Titanic Knurl";
        public override string InternalPickupToken => "knurl";
        public override bool NewPickup => true;

        public override string PickupText => "Boosts health" +
                                             (Armor != 0f ? " , armor" : "") +
                                             " and regeneration.";

        public override string DescText => "Increases <style=cIsHealing>maximum health</style> by" +
                                           (FlatHealth != 0f ? " <style=cIsHealing>" + FlatHealth + "</style> <style=cStack>(+" + FlatHealth + " per stack)</style>" : "") +
                                           (FlatHealth != 0f && PercentHealth != 0f ? " +" : "") +
                                           (PercentHealth != 0f ? " <style=cIsHealing>" + d(PercentHealth) + "</style>" : "") +
                                           (StackPercentHealth ? " <style=cStack>(+" + d(PercentHealth) + " per stack)</style>" : "") +
                                           (Armor != 0f && FlatHealth != 0f ? ", armor by " + "<style=cIsHealing>" + Armor + "</style>" : "") +
                                           (StackArmor ? " <style=cStack>(+" + Armor + " per stack)</style>" : "") +
                                           (Regen != 0f ? " and <style=cIsHealing>base health regeneration</style> by <style=cIsHealing>" + Regen + " hp/s <style=cStack>(+" + Regen + "hp/s per stack)</style>." : ".");

        public override void Init()
        {
            FlatHealth = ConfigOption(40f, "Flat Health", "Per Stack. Vanilla is 40");
            PercentHealth = ConfigOption(0f, "Percent Health", "Decimal. Vanilla is 0");
            StackPercentHealth = ConfigOption(false, "Stack Percent Health?", "Vanilla is false");
            Regen = ConfigOption(1.6f, "Regen", "Per Stack. Vanilla is 1.6");
            Armor = ConfigOption(0f, "Armor", "Vanilla is 0");
            StackArmor = ConfigOption(false, "Stack Armor?", "Vanilla is false");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeHealth;
            IL.RoR2.CharacterBody.RecalculateStats += ChangeRegen;
            RecalculateStatsAPI.GetStatCoefficients += AddBehaviorArmor;
            RecalculateStatsAPI.GetStatCoefficients += AddBehaviorPercentHealth;
        }

        public static void ChangeHealth(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(40f),
                x => x.MatchMul()
            );
            c.Index += 1;
            c.Next.Operand = FlatHealth;
        }

        public static void ChangeRegen(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(1.6f),
                x => x.MatchMul()
            );
            c.Index += 1;
            c.Next.Operand = Regen;
        }

        public static void AddBehaviorPercentHealth(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.Knurl);
                if (stack > 0)
                {
                    args.healthMultAdd += (StackPercentHealth ? PercentHealth * stack : PercentHealth);
                }
            }
        }

        public static void AddBehaviorArmor(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.Knurl);
                if (stack > 0)
                {
                    args.armorAdd += (StackArmor ? Armor * stack : Armor);
                }
            }
        }
    }
}