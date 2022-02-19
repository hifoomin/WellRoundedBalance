using UnityEngine;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class WillOTheWisp : ItemBase
    {
        public static float damage;
        public static float damagestack;
        public static float range;
        public static float rangestack;
        public static float procco;

        public override string Name => ":: Items :: Greens :: Will O The Wisp";
        public override string InternalPickupToken => "explodeOnDeath";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public static float actualdamagestack = damage * damagestack;

        public override string DescText => "On killing an enemy, spawn a <style=cIsDamage>lava pillar</style> in a <style=cIsDamage>" + range + "m</style> <style=cStack>(+" + rangestack + "m per stack)</style> radius for <style=cIsDamage>" + d(damage) + "</style> <style=cStack>(+" + d(actualdamagestack) + " per stack)</style> base damage.";


        public override void Init()
        {
            damage = ConfigOption(3.5f, "Base Damage", "Decimal. Vanilla is 3.5");
            damagestack = ConfigOption(2.8f, "Stack Damage", "Decimal. Per Stack. Vanilla is 2.8");
            procco = ConfigOption(1f, "Proc Coefficient", "Decimal. Vanilla is 1");
            range = ConfigOption(12f, "Base Range", "Vanilla is 12");
            rangestack = ConfigOption(2.4f, "Stack Range", "Per Stack. Vanilla is 2.4");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeDamage;
            IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeRange;
            ChangeProc();
        }
        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(3.5f),
                x => x.MatchLdcR4(1f),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcI4(1),
                x => x.MatchSub(),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.8f)
                // ik this is guh huge but i wanted to uhh change all this stupid shit
            );
            c.Next.Operand = damage;
            c.Index += 6;
            c.Next.Operand = damagestack / damage;

        }
        public static void ChangeRange(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(12f),
                x => x.MatchLdcR4(2.4f)
            );
            c.Next.Operand = range;
            c.Index += 1;
            c.Next.Operand = rangestack;
        }
        public static void ChangeProc()
        {
            var w = Resources.Load<GameObject>("prefabs/networkedobjects/WilloWispDelay").GetComponent<RoR2.DelayBlast>();
            w.procCoefficient = procco;
        }
    }
}
