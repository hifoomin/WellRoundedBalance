using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace UltimateCustomRun.Items.Yellows
{
    public class MoltenPerforator : ItemBase
    {
        public static float Damage;
        public static float Chance;
        public static float ProcCoefficient;
        public static int Count;

        public override string Name => ":: Items :::: Yellows :: Molten Perforator";
        public override string InternalPickupToken => "fireballsOnHit";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "style=cIsDamage>" + Chance + "%</style> chance on hit to call forth <style=cIsDamage>" + Count + " magma balls</style> from an enemy, dealing <style=cIsDamage>300%</style> <style=cStack>(+300% per stack)</style> TOTAL damage and <style=cIsDamage>igniting</style> all enemies.";

        public override void Init()
        {
            Damage = ConfigOption(3f, "TOTAL Damage", "Decimal. Per Stack. Vanilla is 3");
            Chance = ConfigOption(10f, "Chance", "Vanilla is 10");
            ProcCoefficient = ConfigOption(1f, "Proc Coefficient", "Vanilla is 1");
            Count = ConfigOption(3, "Magma ball Count", "Vanilla is 3");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeCount;
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeChance;
            ChangeProcCoefficient();
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeDamage;
        }

        private void ChangeChance(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchStloc(115),
                x => x.MatchLdcR4(10f)
            );
            c.Index += 1;
            c.Next.Operand = Chance;
        }

        private void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchStloc(117),
                x => x.MatchLdcR4(3f)
            );
            c.Index += 1;
            c.Next.Operand = Damage;
        }

        private void ChangeProcCoefficient()
        {
            var m = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/firemeatball").GetComponent<ProjectileController>();
            m.procCoefficient = ProcCoefficient;
        }

        private void ChangeCount(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcI4(3),
                x => x.MatchStloc(117)
            );
            c.Remove();
            c.Emit(OpCodes.Ldc_I4, Count);
        }
    }
}