using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace UltimateCustomRun.Items.Reds
{
    public class CeremonialDagger : ItemBase
    {
        public static float Damage;
        public static float ProcCoefficient;
        public static int Count;
        public override string Name => ":: Items ::: Reds :: Ceremonial Dagger";
        public override string InternalPickupToken => "dagger";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "Killing an enemy fires out <style=cIsDamage>3</style> <style=cIsDamage>homing daggers</style> that deal <style=cIsDamage>" + d(Damage) + "</style> <style=cStack>(+" + d(Damage) + " per stack)</style> base damage.";

        public override void Init()
        {
            Damage = ConfigOption(1.5f, "Damage per Dagger", "Decimal. Per Stack. Vanilla is 1.5");
            ROSOption("Greens", 0f, 10f, 0.1f, "3");
            ProcCoefficient = ConfigOption(1f, "Proc Coefficient per Dagger", "Vanilla is 1");
            ROSOption("Greens", 0f, 1f, 0.05f, "3");
            Count = ConfigOption(3, "Dagger Count", "Vanilla is 3");
            ROSOption("Greens", 0f, 10f, 1f, "3");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeDamage;
            IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeCount;
            ChangeProc();
        }

        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchBle(out ILLabel IL_067A),
                x => x.MatchLdcR4(1.5f)
            );
            c.Index += 1;
            c.Next.Operand = Damage;
        }

        public static void ChangeCount(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdloc(59),
                x => x.MatchLdcI4(3)
            );
            c.Index += 1;
            c.Remove();
            c.Emit(OpCodes.Ldc_I4, Count);
        }

        public static void ChangeProc()
        {
            var c = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/daggerprojectile").GetComponent<ProjectileController>();
            c.procCoefficient = ProcCoefficient;
        }
    }
}