using MonoMod.Cil;

namespace WellRoundedBalance.Items.Reds
{
    public class DiosBestFriend : ItemBase<DiosBestFriend>
    {
        public override string Name => ":: Items ::: Reds :: Dios Best Friend";
        public override ItemDef InternalPickup => RoR2Content.Items.ExtraLife;

        public override string PickupText => "Cheat death. Consumed on use.";

        public override string DescText => "<style=cIsUtility>Upon death</style>, this item will be <style=cIsUtility>consumed</style>, you will <style=cIsHealing>return to life</style> with <style=cIsHealing>" + invincibilityDuration + " seconds of invulnerability</style> and gain <style=cIsUtility>1</style> Tougher Times.";

        [ConfigField("Invincibility Duration", 5f)]
        public static float invincibilityDuration;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterMaster.RespawnExtraLife += CharacterMaster_RespawnExtraLife1;
            On.RoR2.CharacterMaster.RespawnExtraLife += CharacterMaster_RespawnExtraLife;
        }

        private void CharacterMaster_RespawnExtraLife1(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(3f)))
            {
                c.Next.Operand = invincibilityDuration;
            }
            else
            {
                Logger.LogError("Failed to apply Dios Best Friend Invincibility hook");
            }
        }

        private void CharacterMaster_RespawnExtraLife(On.RoR2.CharacterMaster.orig_RespawnExtraLife orig, CharacterMaster self)
        {
            orig(self);
            if (NetworkServer.active && self.inventory)
            {
                self.inventory.GiveItem(RoR2Content.Items.Bear);
                CharacterMasterNotificationQueue.SendTransformNotification(self, RoR2Content.Items.ExtraLife.itemIndex, RoR2Content.Items.Bear.itemIndex, CharacterMasterNotificationQueue.TransformationType.Default);
            }
        }
    }
}