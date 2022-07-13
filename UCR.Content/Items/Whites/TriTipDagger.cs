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
        public override bool NewPickup => true;
        public override string PickupText => "Gain +" + Chance + "% chance to bleed enemies on hit.";

        public override string DescText => "<style=cIsDamage>" + Chance + "%</style> <style=cStack>(+" + Chance + "% per stack)</style> Chance to <style=cIsDamage>bleed</style> an enemy for <style=cIsDamage>" + d(0.2f * 4f * 3f) + "</style> base damage.";

        public override void Init()
        {
            Chance = ConfigOption(10f, "Chance", "Per Stack. Vanilla is 10");
            ROSOption("Whites", 0f, 20f, 1f, "1");
            //Duration = ConfigOption(3f, "Debuff Duration", "Vanilla is 3");
            StackDamage = ConfigOption(true, "Stack Bleed Debuff?", "Vanilla is true");
            ROSOption("Whites", 0f, 5f, 0.01f, "1");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeChance;
            ChangeBehavior();
        }

        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before
            // to be done
            );
        }

        public static void ChangeChance(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdarg(0),
                x => x.MatchLdcR4(10f),
                x => x.MatchLdloc(out _),
                x => x.MatchConvR4()
            );
            c.Index += 1;
            c.Next.Operand = Chance;
        }

        public static void ChangeBehavior()
        {
            var b = LegacyResourcesAPI.Load<BuffDef>("buffdefs/bleeding");
            b.canStack = StackDamage;
        }
    }
}