using RoR2;
using RoR2.Projectile;
using UnityEngine;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class StickyBomb : ItemBase
    {
        public static float damage;
        public static float chance;
        public static float dur;
        public static float range;
        public static bool falloff;

        public override string Name => ":: Items : Whites :: Sticky Bomb";
        public override string InternalPickupToken => "stickyBomb";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "<style=cIsDamage>" + chance + "%</style> <style=cStack>(+" + chance + "% per stack)</style> chance on hit to attach a <style=cIsDamage>bomb</style> to an enemy, detonating for <style=cIsDamage>" + d(damage) + "</style> TOTAL damage.";
        public override void Init()
        {
            damage = ConfigOption(1.8f, "Total Damage", "Decimal. Vanilla is 1.8");
            chance = ConfigOption(5f, "Chance", "Vanilla is 5");
            dur = ConfigOption(1.5f, "Delay", "Vanilla is 1.5");
            range = ConfigOption(10f, "Range", "Vanilla is 10");
            falloff = ConfigOption(false, "Falloff Type", "If set to true, use None\nIf set to false, use Sweetspot.\nVanilla is false");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeChance;
            Changes();
        }
        public static void Changes()
        {
            var nowthatssticky = Resources.Load<GameObject>("prefabs/projectiles/stickybomb");
            var s = nowthatssticky.GetComponent<ProjectileImpactExplosion>();
            s.blastDamageCoefficient = damage / 1.8f;
            // weird
            s.blastRadius = range;
            s.lifetime = dur;
            s.falloffModel = falloff ? BlastAttack.FalloffModel.SweetSpot : BlastAttack.FalloffModel.None;
        }
        public static void ChangeChance(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchBle(out _),
                x => x.MatchLdcR4(5)
            );
            c.Index += 1;
            c.Next.Operand = chance;
        }
    }
}
