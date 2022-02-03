using UnityEngine;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class Bandolier : Based
    {
        public static float basee;
        public static float exponent;

        public override string Name => ":: Items :: Greens :: Bandolier";
        public override string InternalPickupToken => "bandolier";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public static float actual = (1f - 1f / Mathf.Pow(1f + basee, exponent)) * 100f;
        public static float actualtwo = (1f - 1f / Mathf.Pow(2f + basee, exponent) - 1f - 1f / Mathf.Pow(1f + basee, exponent)) * 100f;
        float firststack = Mathf.Round(actual);
        float secondstack = Mathf.Round(actualtwo);
        
        public override string DescText => "<style=cIsUtility>" + firststack + "% </style> <style=cStack>(+" + secondstack + "% on stack)</style> chance on kill to drop an ammo pack that <style=cIsUtility>resets all skill cooldowns</style>.";


        public override void Init()
        {
            basee = ConfigOption(1f, "Base", "Vanilla is 1\nFormula:\n1 - 1 / (stack + Base)^Exponent) * 100\nIf you want to make stacking better, decrease the Base and increase the Exponent.");
            exponent = ConfigOption(0.33f, "Exponent", "Decimal. Vanilla is 0.33");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeBase;
            IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeExponent;
        }

        public static void ChangeBase(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1f),
                x => x.MatchLdcR4(1f),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcI4(1)
            );
            c.Index += 3;
            c.Next.Operand = basee;
        }
        public static void ChangeExponent(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchAdd(),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.33f)
            );
            c.Index += 2;
            c.Next.Operand = exponent;
        }
    }
}
