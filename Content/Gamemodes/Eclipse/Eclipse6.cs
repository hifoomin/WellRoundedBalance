using MonoMod.Cil;

namespace WellRoundedBalance.Gamemodes.Eclipse
{
    internal class Eclipse6 : GamemodeBase<Eclipse6>
    {
        [ConfigField("Maximum Movement Speed Gain", "Decimal. Only applies to Eclipse 6 and higher.", 0.42f)]
        public static float maximumMovementSpeedGain;

        [ConfigField("Maximum Attack Speed Gain", "Decimal. Only applies to Eclipse 6 and higher.", 0.35f)]
        public static float maximumAttackSpeedGain;

        [ConfigField("Maximum Cooldown Reduction Gain", "Decimal. Only applies to Eclipse 6 and higher.", 0.35f)]
        public static float maximumCooldownReductionGain;

        [ConfigField("Mithrix Stat Multiplier", "Decimal. Only applies to Eclipse 6 and higher.", 0.9f)]
        public static float mithrixStatMultiplier;

        public override string Name => ":: Gamemode : Eclipse 6";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            if (Mechanics.Bosses.Enrage.instance == null) {
                return;
            }
            IL.RoR2.DeathRewards.OnKilledServer += DeathRewards_OnKilledServer;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.isChampion && sender.healthComponent)
            {
                var increase = Mathf.Clamp01(1f - sender.healthComponent.combinedHealthFraction);
                if (Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse6 && Eclipse6.instance.isEnabled)
                {
                    if (sender.bodyIndex == BodyCatalog.FindBodyIndex("BrotherBody(Clone)") || sender.bodyIndex == BodyCatalog.FindBodyIndex("BrotherHurtBody(Clone)"))
                    {
                        // Main.WRBLogger.LogError("Affecting Mithrix");
                        args.moveSpeedMultAdd += increase * (maximumMovementSpeedGain * mithrixStatMultiplier);
                        args.attackSpeedMultAdd += increase * (maximumAttackSpeedGain * mithrixStatMultiplier);
                        args.cooldownMultAdd -= increase * (maximumCooldownReductionGain * mithrixStatMultiplier);
                    }
                    else
                    {
                        args.moveSpeedMultAdd += increase * maximumMovementSpeedGain;
                        args.attackSpeedMultAdd += increase * maximumAttackSpeedGain;
                        args.cooldownMultAdd -= increase * maximumCooldownReductionGain;
                    }
                }
            }
        }

        private void DeathRewards_OnKilledServer(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.8f)))
            {
                c.Next.Operand = 1f;
            }
            else
            {
                Logger.LogError("Failed to apply Eclipse 6 hook");
            }
        }
    }
}