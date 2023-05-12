namespace WellRoundedBalance.Enemies.Bosses
{
    internal class Scavenger : EnemyBase<Scavenger>
    {
        public override string Name => "::: Bosses :: Scavenger";

        public static BuffDef armorBuff;

        public override void Init()
        {
            base.Init();
            armorBuff = ScriptableObject.CreateInstance<BuffDef>();
            armorBuff.canStack = false;
            armorBuff.isHidden = false;
            armorBuff.isCooldown = false;
            armorBuff.isDebuff = false;
            armorBuff.buffColor = new Color32(214, 201, 58, 255);
            armorBuff.iconSprite = Utils.Paths.BuffDef.bdArmorBoost.Load<BuffDef>().iconSprite;

            ContentAddition.AddBuffDef(armorBuff);
        }

        public override void Hooks()
        {
            IL.EntityStates.ScavMonster.BaseSitState.OnEnter += BaseSitState_OnEnter;
            IL.EntityStates.ScavMonster.BaseSitState.OnExit += BaseSitState_OnExit;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            Changes();
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.HasBuff(armorBuff))
            {
                args.armorAdd += 100f;
            }
        }

        private void BaseSitState_OnExit(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(RoR2Content.Buffs), nameof(RoR2Content.Buffs.ArmorBoost))))
            {
                c.Remove();
                c.Emit<Scavenger>(OpCodes.Ldsfld, nameof(armorBuff));
            }
        }

        private void BaseSitState_OnEnter(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(RoR2Content.Buffs), nameof(RoR2Content.Buffs.ArmorBoost))))
            {
                c.Remove();
                c.Emit<Scavenger>(OpCodes.Ldsfld, nameof(armorBuff));
            }
        }

        private void Changes()
        {
            var grove1 = Utils.Paths.DirectorCardCategorySelection.dccsRootJungleMonsters.Load<DirectorCardCategorySelection>();
            grove1.categories[3].cards[0].minimumStageCompletions = 4;

            var grove2 = Utils.Paths.DirectorCardCategorySelection.dccsRootJungleMonstersDLC1.Load<DirectorCardCategorySelection>();
            grove2.categories[3].cards[0].minimumStageCompletions = 4;
        }
    }
}