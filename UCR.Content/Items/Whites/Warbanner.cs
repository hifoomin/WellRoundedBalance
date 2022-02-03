using R2API;
using RoR2;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class Warbanner : Based
    {
        public static float basedamage;
        public static bool damstack;
        public static float range;
        public static float rangestack;

        public override string Name => ":: Items : Whites :: Warbanner";
        public override string InternalPickupToken => "wardOnLevel";
        public override bool NewPickup => true;

        public static bool wBaseDamage = basedamage != 0f;
        public static float actualRange = range + rangestack;

        public override string PickupText => "Drop a Warbanner on level up or starting the Teleporter event. Grants allies " +
                                             (wBaseDamage ? "base damage, " : "") +
                                             "movement speed and attack speed.";
        public override string DescText => "On <style=cIsUtility>level up</style> or starting the <style=cIsUtility>Teleporter event</style>, drop a banner that strengthens all allies within <style=cIsUtility>" + actualRange + "m</style> <style=cStack>(+" + rangestack + "m per stack)</style>. Raise " +
                                           (wBaseDamage ? "<style=cIsDamage>base damage</style> by <style=cIsDamage>" + basedamage + "</style>, " +
                                           (damstack ? "<style=cStack>(+" + basedamage + " per stack)</style>. " : "") : "") +
                                           "<style=cIsUtility>movement speed</style> and <style=cIsDamage>attack speed</style> by <style=cIsDamage>30%</style>.";
        public override void Init()
        {
            basedamage = ConfigOption(0f, "Base Damage", "Per Stack. Vanilla is 0");
            range = ConfigOption(16f, "Base Range", "Vanilla is 16");
            rangestack = ConfigOption(8f, "Stack Bleed Debuff?", "Vanilla is 8");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.TeleporterInteraction.ChargingState.OnEnter += ChangeRadiusTP;
            IL.RoR2.Items.WardOnLevelManager.OnCharacterLevelUp += ChangeRadius;
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
        }
        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.WardOnLevel);
                if (sender.HasBuff(RoR2Content.Buffs.Warbanner.buffIndex))
                {
                    args.baseDamageAdd += (damstack ? basedamage * stack : basedamage);
                }
            }
        }
        public static void ChangeRadius(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(8),
                x => x.MatchLdcR4(8)
            );
            c.Next.Operand = range - rangestack;
            c.Index += 1;
            c.Next.Operand = rangestack;
        }
        public static void ChangeRadiusTP(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(8),
                x => x.MatchLdcR4(8)
            );
            c.Next.Operand = range - rangestack;
            c.Index += 1;
            c.Next.Operand = rangestack;
        }
    }
}
