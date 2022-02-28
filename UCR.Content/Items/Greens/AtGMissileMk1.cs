using MonoMod.Cil;
using RoR2.Projectile;
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
            Damage = ConfigOption(3f, "Total Damage", "Decimal. Per Stack. Vanilla is 3");
            ProcCoefficient = ConfigOption(1f, "Proc Coefficient", "Decimal. Vanilla is 1");
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
            c.Next.Operand = Chance;
        }

        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(3f)
            );
            c.Next.Operand = Damage;
        }

        public static void ChangeProc()
        {
            var mp = Resources.Load<GameObject>("prefabs/projectiles/MissileProjectile").GetComponent<ProjectileController>();
            mp.procCoefficient = ProcCoefficient;
        }
    }
}