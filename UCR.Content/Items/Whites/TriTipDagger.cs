using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Items.Whites
{
    public class TriTipDagger : ItemBase
    {
        public static float Chance;
        public static float Duration;
        public static bool StackDamage;

        public override string Name => ":: Items : Whites :: Tri Tip Dagger";
        public override string InternalPickupToken => "bleedOnHit";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "<style=cIsDamage>" + Chance + "%</style> <style=cStack>(+" + Chance + "% per stack)</style> Chance to <style=cIsDamage>bleed</style> an enemy for <style=cIsDamage>" + d(0.2f * 4f * Duration) + "</style> base damage.";

        public override void Init()
        {
            Chance = ConfigOption(10f, "Chance", "Per Stack. Vanilla is 10");
            Duration = ConfigOption(3f, "Debuff Duration", "Vanilla is 3");
            StackDamage = ConfigOption(true, "Stack Bleed Debuff?", "Vanilla is true");
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
            c.Next.Operand = Chance;
        }

        public static void ChangeDuration(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdfld(typeof(DamageInfo).GetField("attacker")),
                // not sure if this is the right way to do it
                x => x.MatchLdcI4(0),
                x => x.MatchLdcR4(3)
            );
            c.Index += 2;
            c.Next.Operand = Duration;
        }

        public static void ChangeBehavior()
        {
            var b = Resources.Load<BuffDef>("buffdefs/bleeding");
            b.canStack = StackDamage;
        }
    }
}