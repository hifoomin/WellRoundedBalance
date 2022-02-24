using MonoMod.Cil;
using RoR2.Projectile;
using UnityEngine;

namespace UltimateCustomRun.Items.Reds
{
    public class CeremonialDagger : ItemBase
    {
        public static float Damage;
        public static float ProcCoefficient;
        public override string Name => ":: Items ::: Reds :: Ceremonial Dagger";
        public override string InternalPickupToken => "dagger";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "Killing an enemy fires out <style=cIsDamage>3</style> <style=cIsDamage>homing daggers</style> that deal <style=cIsDamage>" + d(Damage) + "</style> <style=cStack>(+" + d(Damage) + " per stack)</style> base Damage.";

        public override void Init()
        {
            Damage = ConfigOption(1.5f, "Damage per Dagger", "Decimal. Per Stack. Vanilla is 1.5");
            ProcCoefficient = ConfigOption(1f, "Proc Coefficient per Dagger", "Vanilla is 1");
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
            c.Next.Operand = Damage;
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
            c.procCoefficient = ProcCoefficient;
        }
    }
}