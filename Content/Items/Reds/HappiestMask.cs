using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Reds
{
    public class HappiestMask : ItemBase
    {
        public override string Name => ":: Items ::: Reds :: Happiest Mask";
        public override string InternalPickupToken => "ghostOnKill";

        public override string PickupText => "Chance on killing an enemy to summon a ghost.";

        public override string DescText => "Killing enemies has a <style=cIsDamage>30%</style> chance to <style=cIsDamage>spawn a ghost</style> of the killed enemy with <style=cIsDamage>1500%</style> damage. Lasts <style=cIsDamage>30s</style> <style=cStack>(+30s per stack)</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += Changes;
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchBrfalse(out _),
                    x => x.MatchLdcR4(7f)))
            {
                c.Index += 1;
                c.Remove();
                c.Emit(OpCodes.Ldc_R4, 30f);
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Happiest Mask Chance hook");
            }
        }
    }
}