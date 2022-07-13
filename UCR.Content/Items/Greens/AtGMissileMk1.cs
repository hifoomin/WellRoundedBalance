using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace UltimateCustomRun.Items.Greens
{
    public class AtGMissileMk1 : ItemBase
    {
        public static float Chance;
        public static float Damage;
        public static float ProcCoefficient;

        public override string Name => ":: Items :: Greens :: AtG Missile Mk1";
        public override string InternalPickupToken => "missile";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "<style=cIsDamage>" + Chance + "%</style> Chance to fire a missile that deals <style=cIsDamage>" + d(Damage) + "</style> <style=cStack>(+" + d(Damage) + " per stack)</style> TOTAL damage.";

        public override void Init()
        {
            Chance = ConfigOption(10f, "Chance", "Vanilla is 10");
            ROSOption("Greens", 0f, 100f, 1f, "2");
            Damage = ConfigOption(3f, "Total Damage", "Decimal. Per Stack. Vanilla is 3");
            ROSOption("Greens", 0f, 6f, 0.1f, "2");
            ProcCoefficient = ConfigOption(1f, "Proc Coefficient", "Decimal. Vanilla is 1");
            ROSOption("Greens", 0f, 1f, 0.05f, "2");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeDamage;
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeChance;
            ChangeProc();
        }

        public static void ChangeChance(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchBle(out _),
                x => x.MatchLdcR4(10f),
                x => x.MatchLdarg(1)
            );
            c.Index += 1;
            c.Next.Operand = Chance;
        }

        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchBrfalse(out _),
                x => x.MatchLdcR4(3f)
            );
            c.Index += 1;
            c.Next.Operand = Damage;
        }

        public static void ChangeProc()
        {
            var mp = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/MissileProjectile").GetComponent<ProjectileController>();
            mp.procCoefficient = ProcCoefficient;
        }
    }
}