using RoR2;
using UnityEngine;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class FocusCrystal : Based
    {
        public static float damage;
        public static float range;

        public override string Name => ":: Items : Whites :: Focus Crystal";
        public override string InternalPickupToken => "nearbyDamageBonus";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Increase damage to enemies within <style=cIsDamage>" + range + "m</style> by <style=cIsDamage>" + d(damage) + "</style> <style=cStack>(+" + d(damage) + " per stack)</style>.";
        public override void Init()
        {
            damage = ConfigOption(0.2f, "Damage Coefficient", "Decimal. Per Stack. Vanilla is 0.2");
            range = ConfigOption(13f, "Range", "Vanilla is 13");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += ChangeDamage;
            IL.RoR2.HealthComponent.TakeDamage += ChangeRadius;
            ChangeVisual();
        }
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
            c.Next.Operand = damage;
        }
        public static void ChangeRadius(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<UnityEngine.Vector3>("get_sqrMagnitude"),
                x => x.MatchLdcR4(169f)
            );
            c.Index += 1;
            c.Next.Operand = range * range;
        }
        public static void ChangeVisual()
        {
            var focus = Resources.Load<GameObject>("Prefabs/NetworkedObjects/NearbyDamageBonusIndicator");
            float actualRange = range / 13f;
            focus.transform.localScale = new Vector3(actualRange, actualRange, actualRange);
        }
    }
}
