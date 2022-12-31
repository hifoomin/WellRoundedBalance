using MonoMod.Cil;
using UnityEngine;

namespace WellRoundedBalance.Items.Reds
{
    public class DiosBestFriend : ItemBase
    {
        public override string Name => ":: Items ::: Reds :: Dios Best Friend";
        public override string InternalPickupToken => "extraLife";

        public override string PickupText => "Cheat death. Consumed on use.";

        public override string DescText => "<style=cIsUtility>Upon death</style>, this item will be <style=cIsUtility>consumed</style> and you will <style=cIsHealing>return to life</style> with <style=cIsHealing>5 seconds of invulnerability</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterMaster.RespawnExtraLife += ChangeInvinc;
        }

        private void ChangeInvinc(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(3f)))
            {
                c.Next.Operand = 5f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Dios Best Friend Invincibility hook");
            }
        }
    }
}