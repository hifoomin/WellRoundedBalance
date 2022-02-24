using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Items.Whites
{
    public class FocusCrystal : ItemBase
    {
        public static float Damage;
        public static float Radius;

        public override string Name => ":: Items : Whites :: Focus Crystal";
        public override string InternalPickupToken => "nearbyDamageBonus";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Increase Damage to enemies within <style=cIsDamage>" + Radius + "m</style> by <style=cIsDamage>" + d(Damage) + "</style> <style=cStack>(+" + d(Damage) + " per stack)</style>.";

        public override void Init()
        {
            Damage = ConfigOption(0.2f, "Damage Coefficient", "Decimal. Per Stack. Vanilla is 0.2");
            Radius = ConfigOption(13f, "Range", "Vanilla is 13");
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
            c.Next.Operand = Damage;
        }

        public static void ChangeRadius(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<UnityEngine.Vector3>("get_sqrMagnitude"),
                x => x.MatchLdcR4(169f)
            );
            c.Index += 1;
            c.Next.Operand = Radius * Radius;
        }

        public static void ChangeVisual()
        {
            var focus = Resources.Load<GameObject>("Prefabs/NetworkedObjects/NearbyDamageBonusIndicator");
            float actualRange = Radius / 13f;
            focus.transform.localScale = new Vector3(actualRange, actualRange, actualRange);
        }
    }
}