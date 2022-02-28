using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace UltimateCustomRun.Items.Reds
{
    public class HappiestMask : ItemBase
    {
        public static float Chance;
        public static int Duration;
        public override string Name => ":: Items ::: Reds :: Happiest Mask";
        public override string InternalPickupToken => "ghostOnKill";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "Killing enemies has a <style=cIsDamage>" + Chance + "%</style> chance to <style=cIsDamage>spawn a ghost</style> of the killed enemy with <style=cIsDamage>1500%</style> damage. Lasts <style=cIsDamage>" + Duration + "s</style> <style=cStack>(+" + Duration + "s per stack)</style>.";

        public override void Init()
        {
            Chance = ConfigOption(7f, "Chance", "Vanilla is 7");
            Duration = ConfigOption(30, "Duration", "Per Stack. Vanilla is 30");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeChance;
            IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeDuration;
        }

        public static void ChangeChance(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchBrfalse(out _),
                x => x.MatchLdcR4(7f)
            );
            c.Index += 1;
            c.Remove();
            c.Emit(OpCodes.Ldc_R4, Chance);
        }

        public static void ChangeDuration(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcI4(30),
                x => x.MatchMul()
            );
            c.Remove();
            c.Emit(OpCodes.Ldc_I4, Duration);
        }
    }
}