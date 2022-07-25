using MonoMod.Cil;
using UnityEngine;

namespace UltimateCustomRun.Items.Whites
{
    public class EnergyDrink : ItemBase
    {
        public static float Speed;
        public static bool Change;
        public static float SprintingSpeed;
        public override string Name => ":: Items : Whites :: Energy Drink";
        public override string InternalPickupToken => "sprintBonus";
        public override bool NewPickup => true;

        public override string PickupText => "Increase sprint speed by +" + Mathf.Round((Speed / 1.45f) * 100f) + "%.";

        public override string DescText => "<style=cIsUtility>Sprint Speed</style> is improved by <style=cIsUtility>" +
                                           (Change ? SprintingSpeed * 100f + "%</style> <style=cStack>(+" + SprintingSpeed * 100f + "% per stack)</style>." : Mathf.Round((Speed / 1.45f) * 100f) + "%</style> <style=cStack>(+" + Mathf.Round((Speed / 1.45f) * 100f) + "% per stack)</style>.");

        public override void Init()
        {
            Speed = ConfigOption(0.25f, "Speed Increase", "Decimal. Per Stack. Vanilla is 0.25");
            //Change = ConfigOption(false, "Increase the Sprinting Speed Multiplier instead?", "Vanilla is false");
            //SprintingSpeed = ConfigOption(0.25f, "Sprinting Speed Multiplier Increase", "Vanilla is 0\nFormula: (Base Movement Speed * (1 + (1 * Item Speed Increase) + (1 * Item Speed Increase)) * (Sprinting Speed Multiplier + Sprinting Speed Multiplier Increase)");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeSpeed;
            if (Change)
            {
                //On.RoR2.CharacterBody.RecalculateStats += AddBehavior();
            }
        }

        public static void ChangeSpeed(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(0.25f),
                x => x.MatchLdloc(out _)
            );
            c.Index += 1;
            c.Next.Operand = Change ? 0f : Speed;
        }

        /*
        public static void AddBehavior(CharacterBody body)
        {
            if (body.inventory)
            {
                var stack = body.inventory.GetItemCount(RoR2Content.Items.SprintBonus);
                if (stack > 0)
                {
                    body.sprintingSpeedMultiplier += SprintingSpeed * stack;
                }
            }
        }
        */
    }
}