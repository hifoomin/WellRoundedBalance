using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun.Items.Reds
{
    public class WakeOfVultures : ItemBase
    {
        public static float Duration;
        public static float StackDuration;
        public static float Speed;
        public static float StackSpeed;
        public static bool OnlyUnderBuff;
        public override string Name => ":: Items ::: Reds :: Wake of Vultures";
        public override string InternalPickupToken => "headHunter";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => (Speed > 0 || StackSpeed > 0 ? "Gain " + d(Speed) + "</style> <style=cStack>(+" + d(StackSpeed) + " per stack)</style> movement speed. " : "") +
                                           "Gain the <style=cIsDamage>power</style> of any killed elite monster for <style=cIsDamage>" + Duration + "s</style> <style=cStack>(+" + StackDuration + "s per stack)</style>.";

        public override void Init()
        {
            Duration = ConfigOption(8f, "Duration", "Vanilla is 8");
            StackDuration = ConfigOption(5f, "Stack Duration", "Per Stack. Vanilla is 5");
            Speed = ConfigOption(0f, "Speed Increase", "Vanilla is 0");
            StackSpeed = ConfigOption(0f, "Stack Speed Increase", "Vanilla is 0");
            OnlyUnderBuff = ConfigOption(false, "Should the Speed Increase only work with an Elite buff?", "");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeDuration;
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
        }

        public static void AddBehavior(RoR2.CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.HeadHunter);
                if (stack > 0)
                {
                    if (OnlyUnderBuff == false)
                    {
                        args.moveSpeedMultAdd += Speed + StackSpeed * (stack - 1f);
                    }
                    else
                    {
                        for (int i = 0; i < BuffCatalog.eliteBuffIndices.Length; i++)
                        {
                            var buff = BuffCatalog.eliteBuffIndices[i];
                            if (sender.HasBuff(buff))
                            {
                                args.moveSpeedMultAdd += Speed + StackSpeed * (stack - 1f);
                            }
                        }
                    }
                }
            }
        }

        public static void ChangeDuration(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(3f),
                x => x.MatchLdcR4(5f)
            );
            c.Next.Operand = StackDuration;
            c.Index += 1;
            c.Next.Operand = Duration - StackDuration;
        }
    }
}