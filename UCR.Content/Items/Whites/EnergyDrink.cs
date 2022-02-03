using MonoMod.Cil;
using UnityEngine;

namespace UltimateCustomRun
{
    public class EnergyDrink : Based
    {
        public static float speed;

        public override string Name => ":: Items : Whites :: Energy Drink";
        public override string InternalPickupToken => "sprintBonus";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public static float actualSpeed = Mathf.Round(speed / 1.45f * 100f);

        public override string DescText => "<style=cIsUtility>Sprint speed</style> is improved by <style=cIsUtility>" + actualSpeed + "%</style> <style=cStack>(+" + actualSpeed + "% per stack)</style>.";
        public override void Init()
        {
            speed = ConfigOption(0.25f, "Speed Increase", "Decimal. Per Stack. Vanilla is 0.25 / 1.45");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeSpeed;
        }
        // ene is actually a 17.2% increase because its divided by the sprint mult (1.45) for some reason
        // i wanna change that later down the road but im not into cbt 
        public static void ChangeSpeed(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(0.25f),
                x => x.MatchLdloc(out _)
            );
            c.Index += 1;
            c.Next.Operand = speed;
        }
    }
}
