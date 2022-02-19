using R2API;
using RoR2;
using UnityEngine;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class Aegis : ItemBase
    {
        public static float armor;
        public static bool armorstack;
        public static float overhealpercent;
        public static bool dynamicdecay;

        public override string Name => ":: Items ::: Reds :: Aegis";
        public override string InternalPickupToken => "barrierOnOverheal";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => (armor != 0f ? "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>" + armor + "</style>. " +
                                           (armorstack ? "<style=cStack>(+" + armor + " per stack)</style>" : "") : "") +
                                           "Healing past full grants you a <style=cIsHealing>temporary barrier</style> for <style=cIsHealing>" + d(overhealpercent) + " <style=cStack>(+" + d(overhealpercent) + " per stack)</style></style> of the amount you <style=cIsHealing>healed</style>.";
        public override void Init()
        {
            armor = ConfigOption(0f, "Armor", "Vanilla is 0");
            armorstack = ConfigOption(false, "Stack Armor?", "Vanilla is false");
            overhealpercent = ConfigOption(0.5f, "Overheal Percent", "Decimal. Per Stack. Vanilla is 0.5");
            dynamicdecay = ConfigOption(false, "Make Barrier Decay Dynamic?", "Vanilla is false");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.Heal += ChangeOverheal;
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
            if (dynamicdecay)
            {
                On.RoR2.CharacterBody.FixedUpdate += ChangeBarrierDecay;
            }
        }
        public static void ChangeBarrierDecay(On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody self)
        {
            if (self.inventory)
            {
                var stack = self.inventory.GetItemCount(RoR2Content.Items.BarrierOnOverHeal);
                if (stack > 0)
                {
                    self.barrierDecayRate = Mathf.Max(1f, self.healthComponent.barrier / self.barrierDecayRate);
                }
            }
            orig(self);
        }
        // sender.barrierDecayRate didnt work SOOO lets try the dumb way ig
        public static void ChangeOverheal(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdfld("RoR2.HealthComponent/ItemCounts", "barrierOnOverHeal"),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.5f)
            );
            c.Index += 2;
            c.Next.Operand = overhealpercent;
        }
        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.BarrierOnOverHeal);
                if (stack > 0)
                {
                    args.armorAdd += (armorstack ? armor * stack : armor);
                }
            }
        }
    }
}
