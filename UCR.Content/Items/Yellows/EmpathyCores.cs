using MonoMod.Cil;
using RoR2;

namespace UltimateCustomRun.Items.Yellows
{
    public class EmpathyCores : ItemBase
    {
        public static float Damage;

        public override string Name => ":: Items :::: Yellows :: Empathy Cores";
        public override string InternalPickupToken => "roboBallBuddy";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Every 30 seconds, <style=cIsUtility>summon two Solus Probes</style> that gain <style=cIsDamage>+" + d(Damage) + "</style> <style=cStack>(+" + d(Damage) + " per stack)</style> damage per <style=cIsUtility>ally on your team</style>.";

        public override void Init()
        {
            Damage = ConfigOption(1f, "Damage Increase per Ally", "Decimal. Per Stack. Vanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeDamage;
        }

        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchStloc(out _),
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdloc(out _),
                    x => x.MatchMul(),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(1f)))
            {
                c.Index += 6;
                c.Next.Operand = Damage;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Empathy Cores Damage hook");
            }
        }
    }
}