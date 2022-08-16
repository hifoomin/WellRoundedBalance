using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun.Items.Whites
{
    public class LensMakersGlasses : ItemBase
    {
        public static float Crit;
        public static float CritDamageMultiplier;
        public override string Name => ":: Items : Whites :: Lens Makers Glasses";
        public override string InternalPickupToken => "critGlasses";
        public override bool NewPickup => true;

        public override string PickupText => "Gain " + Crit + "% chance for hits to 'Critically Strike', dealing" +
                                             (CritDamageMultiplier > 0 ? " much more" : " double") +
                                             " damage.";

        public override string DescText => "Your attacks have a <style=cIsDamage>" + Crit + "%</style> <style=cStack>(+" + Crit + "% per stack)</style> Chance to '<style=cIsDamage>Critically Strike</style>', dealing<style=cIsDamage>" +
                                           (CritDamageMultiplier != 0f ? " much more" : " double") +
                                           " damage</style>.";

        public override void Init()
        {
            Crit = ConfigOption(10f, "Crit Chance", "Per Stack. Vanilla is 10");
            CritDamageMultiplier = ConfigOption(0f, "Crit Damage Increase", "Decimal. Per Stack. Vanilla is 0");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeCrit;
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
        }

        public static void ChangeCrit(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(10f),
                    x => x.MatchMul(),
                    x => x.MatchAdd()))
            {
                c.Next.Operand = Crit;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Lens Maker's Glasses Crit hook");
            }
        }

        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.CritGlasses);
                if (stack > 0)
                {
                    args.critDamageMultAdd += CritDamageMultiplier * stack;
                }
            }
        }
    }
}