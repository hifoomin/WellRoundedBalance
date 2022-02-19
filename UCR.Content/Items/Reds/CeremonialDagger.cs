using RoR2;
using RoR2.Projectile;
using UnityEngine;
using MonoMod.Cil;
using System;

namespace UltimateCustomRun
{
    public class CeremonialDagger : ItemBase
    {
        public static float dmg;
        public static float procco;
        public override string Name => ":: Items ::: Reds :: Ceremonial Dagger";
        public override string InternalPickupToken => "dagger";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "Killing an enemy fires out <style=cIsDamage>3</style> <style=cIsDamage>homing daggers</style> that deal <style=cIsDamage>" + d(dmg) + "</style> <style=cStack>(+" + d(dmg) + " per stack)</style> base damage.";
        public override void Init()
        {
            dmg = ConfigOption(1.5f, "Damage per Dagger", "Decimal. Per Stack. Vanilla is 1.5");
            procco = ConfigOption(1f, "Proc Coefficient per Dagger", "Vanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeDamage;
            ChangeProc();
        }
        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchBle(out ILLabel IL_067A),
                x => x.MatchLdcR4(1.5f)
            );
            c.Index += 1;
            c.Next.Operand = dmg;
        }
        public static void ChangeCount(ILContext il)
        {
            /*
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt(typeof(RoR2.Util).GetMethod("CheckRoll", new Type[] { typeof(float), typeof(CharacterMaster) })),
                x => x.MatchLdcI4(3)
            );
            c.Index += 1;
            c.Next.Operand = Main.CeremonialCount.Value;
            // WHY DOES THIS NOT WORK
            // also thanks harb for checkroll match
            */
        }
        public static void ChangeProc()
        {
            var c = Resources.Load<GameObject>("prefabs/projectiles/daggerprojectile").GetComponent<ProjectileController>();
            c.procCoefficient = procco;
        }
    }
}
