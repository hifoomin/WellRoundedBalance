using RoR2;
using UnityEngine;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class FocusCrystal
    {
        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchStfld<DamageInfo>("damageColorIndex"),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(1f),
                x => x.MatchLdloc(out _),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.2f)
            );
            c.Index += 5;
            c.Next.Operand = Main.FocusCrystalDamage.Value;
        }
        public static void ChangeRadius(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<UnityEngine.Vector3>("get_sqrMagnitude"),
                x => x.MatchLdcR4(169f)
            );
            c.Index += 1;
            c.Next.Operand = Main.FocusCrystalRange.Value * Main.FocusCrystalRange.Value;
        }
        public static void ChangeVisual()
        {
            var focus = Resources.Load<GameObject>("Prefabs/NetworkedObjects/NearbyDamageBonusIndicator");
            float actualRange = Main.FocusCrystalRange.Value / 13;
            focus.transform.localScale = new Vector3(actualRange, actualRange, actualRange);
        }
    }
}
