using MonoMod.Cil;
using UnityEngine;
using R2API;
using RoR2;

namespace UltimateCustomRun
{
    public class EnergyDrink : ItemBase
    {
        public static float speed;
        public static bool change;
        public static float sprintingspeed;
        public override string Name => ":: Items : Whites :: Energy Drink";
        public override string InternalPickupToken => "sprintBonus";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public static float actualSpeed = Mathf.Round(speed / 1.45f * 100f);

        public override string DescText => "<style=cIsUtility>Sprint speed</style> is improved by <style=cIsUtility>" + actualSpeed + "%</style> <style=cStack>(+" + actualSpeed + "% per stack)</style>.";
        public override void Init()
        {
            speed = ConfigOption(0.25f, "Speed Increase", "Decimal. Per Stack. Vanilla is 0.25");
            change = ConfigOption(false, "Increase the Sprinting Speed Multiplier instead?", "Vanilla is false");
            sprintingspeed = ConfigOption(0.0357f, "Sprinting Speed Multiplier Increase", "Vanilla is 0\nFormula: (Base Character Speed + Item Speed Increases) * Sprinting Speed Multiplier");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeSpeed;
            if (change)
            {
                RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
            }
        }
        public static void ChangeSpeed(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(0.25f),
                x => x.MatchLdloc(out _)
            );
            c.Index += 1;
            c.Next.Operand = (change ? speed : 0f);
        }
        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.SprintBonus);
                if (stack > 0)
                {
                    sender.sprintingSpeedMultiplier += 0.0357f;
                }
            }
        }
    }
}
