﻿using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class Brainstalks : ItemBase
    {
        public static float dur;
        public override string Name => ":: Items ::: Reds :: Brainstalks";
        public override string InternalPickupToken => "killEliteFrenzy";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "Upon killing an elite monster, <style=cIsDamage>enter a frenzy</style> for <style=cIsDamage>" + dur + "s</style> <style=cStack>(+" + dur + "s per stack)</style> where <style=cIsUtility>skills have no cooldowns</style>.";
        public override void Init()
        {
            dur = ConfigOption(4f, "Buff Duration", "Per Stack. Vanilla is 4");

            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeDuration;
        }
        public static void ChangeDuration(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.RoR2Content/Buffs", "NoCooldowns"),
                x => x.MatchLdloc(out _),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(4f)
            );
            c.Index += 3;
            c.Next.Operand = dur;
        }
    }
}