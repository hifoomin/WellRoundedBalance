using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace UltimateCustomRun.Items.Whites
{
    public class StickyBomb : ItemBase
    {
        public static float Damage;
        public static float Chance;
        public static float Delay;
        public static float Radius;
        public static int Falloff;

        public override string Name => ":: Items : Whites :: Sticky Bomb";
        public override string InternalPickupToken => "stickyBomb";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "<style=cIsDamage>" + Chance + "%</style> <style=cStack>(+" + Chance + "% per stack)</style> Chance on hit to attach a <style=cIsDamage>bomb</style> to an enemy, detonating for <style=cIsDamage>" + d(Damage) + "</style> TOTAL damage.";

        public override void Init()
        {
            Damage = ConfigOption(1.8f, "Damage", "Decimal. Vanilla is 1.8");
            Chance = ConfigOption(5f, "Chance", "Vanilla is 5");
            Delay = ConfigOption(1.5f, "Delay", "Vanilla is 1.5");
            Radius = ConfigOption(10f, "Range", "Vanilla is 10");
            Falloff = ConfigOption(1, "Falloff Type", "1 - Sweetspot, 2 - Linear, 3 - None.\nVanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeChance;
            Changes();
        }

        public static void Changes()
        {
            var StickyBombImpact = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/stickybomb").GetComponent<ProjectileImpactExplosion>();
            StickyBombImpact.blastDamageCoefficient = Damage / 1.8f;
            // weird
            StickyBombImpact.blastRadius = Radius;
            StickyBombImpact.lifetime = Delay;
            StickyBombImpact.falloffModel = Falloff switch
            {
                1 => BlastAttack.FalloffModel.SweetSpot,
                2 => BlastAttack.FalloffModel.Linear,
                3 => BlastAttack.FalloffModel.None,
                _ => BlastAttack.FalloffModel.SweetSpot,
            };
        }

        public static void ChangeChance(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchBle(out _),
                    x => x.MatchLdcR4(5)))
            {
                c.Index += 1;
                c.Next.Operand = Chance;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Sticky Bomb Chance hook");
            }
        }
    }
}