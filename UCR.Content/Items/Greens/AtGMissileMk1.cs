using RoR2.Projectile;
using UnityEngine;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class AtGMissileMk1 : ItemBase
    {
        public static float chance;
        public static float damage;
        public static float procco;

        public override string Name => ":: Items :: Greens :: AtG Missile Mk1";
        public override string InternalPickupToken => "missile";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "<style=cIsDamage>" + chance + "%</style> chance to fire a missile that deals <style=cIsDamage>" + d(damage) + "</style> <style=cStack>(+" + d(damage) + " per stack)</style> TOTAL damage.";


        public override void Init()
        {
            chance = ConfigOption(10f, "Chance", "Vanilla is 10");
            damage = ConfigOption(3f, "Total Damage", "Decimal. Per Stack. Vanilla is 3");
            procco = ConfigOption(1f, "Proc Coefficient", "Decimal. Vanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.ProcMissile += ChangeDamage;
            IL.RoR2.GlobalEventManager.ProcMissile += ChangeChance;
            ChangeProc();
        }

        public static void ChangeChance(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(10f)
            );
            c.Next.Operand = chance;
        }
        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(3f)
            );
            c.Next.Operand = damage;
        }
        public static void ChangeProc()
        {
            var mp = Resources.Load<GameObject>("prefabs/projectiles/MissileProjectile").GetComponent<ProjectileController>().procCoefficient;
            mp = procco;
        }
    }
}
