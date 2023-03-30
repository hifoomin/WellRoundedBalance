namespace WellRoundedBalance.Mechanics.Interactables
{
    internal class MountainShrine : MechanicBase<MountainShrine>
    {
        public static BuffDef mountainShrineBuff;
        public override string Name => ":: Mechanics :::::: Mountain Shrine";

        [ConfigField("Attack Speed Gain per Mountain Shrine", "", 0.1f)]
        public static float attackSpeedGainPerMountainShrine;

        [ConfigField("Movement Speed Gain per Mountain Shrine", "", 0.1f)]
        public static float movementSpeedGainPerMountainShrine;

        public override void Init()
        {
            mountainShrineBuff = ScriptableObject.CreateInstance<BuffDef>();
            mountainShrineBuff.isHidden = false;
            mountainShrineBuff.canStack = true;
            mountainShrineBuff.isDebuff = false;
            mountainShrineBuff.buffColor = new Color32(72, 180, 255, 255);
            mountainShrineBuff.iconSprite = Main.wellroundedbalance.LoadAsset<Sprite>("texBuffMountainShrine.png");

            ContentAddition.AddBuffDef(mountainShrineBuff);
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CombatDirector.OnEnable += CombatDirector_OnEnable;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody body)
        {
            var teleporter = TeleporterInteraction.instance;
            if (teleporter != null && teleporter.isCharging)
            {
                for (int i = 0; i < teleporter.shrineBonusStacks; i++)
                {
                    if (body.teamComponent.teamIndex != TeamIndex.Player) body.AddBuff(mountainShrineBuff);
                }
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            args.attackSpeedMultAdd += attackSpeedGainPerMountainShrine * sender.GetBuffCount(mountainShrineBuff);
            args.moveSpeedMultAdd += movementSpeedGainPerMountainShrine * sender.GetBuffCount(mountainShrineBuff);
        }

        private void CombatDirector_OnEnable(On.RoR2.CombatDirector.orig_OnEnable orig, CombatDirector self)
        {
            self.minRerollSpawnInterval /= Director.CombatDirector.minimumRerollSpawnIntervalMultiplier;
            self.maxRerollSpawnInterval /= Director.CombatDirector.minimumRerollSpawnIntervalMultiplier;
            self.creditMultiplier += Director.CombatDirector.creditMultiplier;
            self.eliteBias *= Director.CombatDirector.eliteBiasMultiplier;
            var teleporter = TeleporterInteraction.instance;
            if (teleporter != null)
            {
                for (int i = 0; i < teleporter.shrineBonusStacks; i++)
                {
                    self.creditMultiplier *= Director.CombatDirector.creditMultiplierForEachMountainShrine;
                    self.expRewardCoefficient *= Director.CombatDirector.goldAndExperienceMultiplierForEachMountainShrine;
                    self.goldRewardCoefficient *= Director.CombatDirector.goldAndExperienceMultiplierForEachMountainShrine;
                }
            }
            orig(self);
        }
    }
}