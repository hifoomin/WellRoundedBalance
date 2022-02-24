using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun.Items.Whites
{
    public class Warbanner : ItemBase
    {
        public static float BaseDamage;
        public static bool StackBaseDamage;
        public static float Radius;
        public static float StackRadius;

        public override string Name => ":: Items : Whites :: Warbanner";
        public override string InternalPickupToken => "wardOnLevel";
        public override bool NewPickup => true;

        public override string PickupText => "Drop a Warbanner on level up or starting the Teleporter event. Grants allies " +
                                             (BaseDamage != 0f ? "base Damage, " : "") +
                                             "movement Speed and attack Speed.";

        public override string DescText => "On <style=cIsUtility>level up</style> or starting the <style=cIsUtility>Teleporter event</style>, drop a banner that strengthens all allies within <style=cIsUtility>" + Radius + "m</style> <style=cStack>(+" + StackRadius + "m per stack)</style>. Raise " +
                                           (BaseDamage != 0f ? "<style=cIsDamage>base Damage</style> by <style=cIsDamage>" + BaseDamage + "</style>, " +
                                           (StackBaseDamage ? "<style=cStack>(+" + BaseDamage + " per stack)</style>. " : "") : "") +
                                           "<style=cIsUtility>movement Speed</style> and <style=cIsDamage>attack Speed</style> by <style=cIsDamage>30%</style>.";

        public override void Init()
        {
            BaseDamage = ConfigOption(0f, "Base Damage", "Per Stack. Vanilla is 0");
            Radius = ConfigOption(16f, "Base Range", "Vanilla is 16");
            StackRadius = ConfigOption(8f, "Stack Range", "Per Stack. Vanilla is 8");
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
                    args.baseDamageAdd += (StackBaseDamage ? BaseDamage * stack : BaseDamage);
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
            c.Next.Operand = Radius - StackRadius;
            c.Index += 1;
            c.Next.Operand = StackRadius;
        }

        public static void ChangeRadiusTP(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(8),
                x => x.MatchLdcR4(8)
            );
            c.Next.Operand = Radius - StackRadius;
            c.Index += 1;
            c.Next.Operand = StackRadius;
        }
    }
}