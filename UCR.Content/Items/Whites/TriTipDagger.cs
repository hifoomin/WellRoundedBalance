using R2API;
using RoR2;
using UnityEngine;
using MonoMod.Cil;
using UnityEngine.Networking;

namespace UltimateCustomRun
{
    public class TriTipDagger : ItemBase
    {
        public static float chance;
        public static float dur;
        public static bool canStack;

        public override string Name => ":: Items : Whites :: Tri Tip Dagger";
        public override string InternalPickupToken => "bleedOnHit";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public static float actualDamage = 0.2f * 4f * dur;

        public override string DescText => "<style=cIsDamage>" + chance + "%</style> <style=cStack>(+" + chance + "% per stack)</style> chance to <style=cIsDamage>bleed</style> an enemy for <style=cIsDamage>" + d(actualDamage) + "</style> base damage.";
        public override void Init()
        {
            chance = ConfigOption(10f, "Chance", "Per Stack. Vanilla is 10");
            dur = ConfigOption(3f, "Debuff Duration", "Vanilla is 3");
            canStack = ConfigOption(true, "Stack Bleed Debuff?", "Vanilla is true");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeChance;
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeDuration;
            ChangeBehavior();
        }
        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before
                // to be done
            );
        }
        public static void ChangeChance(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdloc(out _),
                x => x.MatchBrtrue(out _),
                x => x.MatchLdcR4(10)
            );
            c.Index += 2;
            c.Next.Operand = chance;
        }
        public static void ChangeDuration(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdfld(typeof(RoR2.DamageInfo).GetField("attacker")),
                // not sure if this is the right way to do it
                x => x.MatchLdcI4(0),
                x => x.MatchLdcR4(3)
            );
            c.Index += 2;
            c.Next.Operand = dur;
        }
        public static void ChangeBehavior()
        {
            var b = Resources.Load<BuffDef>("buffdefs/bleeding");
            b.canStack = canStack;
        }
    }
}
