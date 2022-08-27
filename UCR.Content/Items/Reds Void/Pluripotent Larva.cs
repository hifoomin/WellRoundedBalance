using MonoMod.Cil;
using R2API;
using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Items.VoidReds
{
    public class PluripotentLarva : ItemBase
    {
        public static float Invincibility;
        public static float Delay;

        public override string Name => ":: Items :::::: Voids :: Pluripotent Larva";
        public override string InternalPickupToken => "extraLifeVoid";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "Get a <style=cIsVoid>corrupted</style> extra life. Consumed on use. <style=cIsVoid>Corrupts all Dio's Best Friends.</style>.";

        public override void Init()
        {
            Invincibility = ConfigOption(3f, "Post-Revival Invincibility Duration", "Vanilla is 3");
            Delay = ConfigOption(2f, "Revival Delay", "Vanilla is 2");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterMaster.OnBodyDeath += ChangeDelay;
            IL.RoR2.CharacterMaster.RespawnExtraLifeVoid += ChangeInvinc;
        }

        private void ChangeInvinc(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(3f)))
            {
                c.Next.Operand = Invincibility;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Pluripotent Larva Invincibility hook");
            }
        }

        private void ChangeDelay(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.DLC1Content/Items", "ExtraLifeVoid"),
                x => x.MatchLdcI4(1)))
            {
                c.Index += 5;
                c.Next.Operand = Delay;
                c.Index += 4;
                c.Next.Operand = Mathf.Min(0.1f, Delay - 1);
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Pluripotent Larva Delay hook");
            }
        }
    }
}