namespace WellRoundedBalance.Mechanics.Allies
{
    internal class DontRetaliate : MechanicBase<DontRetaliate>
    {
        public override string Name => ":: Mechanics :::::::::::::: Dont Retaliate";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            CharacterMaster.onStartGlobal += CharacterMaster_onStartGlobal;
        }

        private void CharacterMaster_onStartGlobal(CharacterMaster master)
        {
            if (master.teamIndex == TeamIndex.Player)
            {
                var baseAI = master.GetComponent<BaseAI>();
                if (baseAI)
                    baseAI.neverRetaliateFriendlies = true;
            }
        }
    }
}