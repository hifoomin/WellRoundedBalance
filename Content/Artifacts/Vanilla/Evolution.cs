using R2API.Utils;
using RoR2.Artifacts;
using System;

namespace WellRoundedBalance.Artifacts.Vanilla
{
    internal class Evolution : ArtifactEditBase<Evolution>
    {
        public override string Name => ":: Artifacts :::::: Evolution";

        [ConfigField("Item Count", "", 2)]
        public static int itemCount;

        public static BasicPickupDropTable tier1 = Utils.Paths.BasicPickupDropTable.dtMonsterTeamTier1Item.Load<BasicPickupDropTable>();
        public static BasicPickupDropTable tier2 = Utils.Paths.BasicPickupDropTable.dtMonsterTeamTier2Item.Load<BasicPickupDropTable>();
        public static BasicPickupDropTable tier3 = Utils.Paths.BasicPickupDropTable.dtMonsterTeamTier3Item.Load<BasicPickupDropTable>();

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.Init += MonsterTeamGainsItemsArtifactManager_Init;
            IL.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.GrantMonsterTeamItem += MonsterTeamGainsItemsArtifactManager_GrantMonsterTeamItem;
            IL.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.EnsureMonsterItemCountMatchesStageCount += MonsterTeamGainsItemsArtifactManager_EnsureMonsterItemCountMatchesStageCount;
        }

        private void MonsterTeamGainsItemsArtifactManager_Init(On.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.orig_Init orig)
        {
            orig();
            var origPattern = MonsterTeamGainsItemsArtifactManager.dropPattern.ToList();

            var duplicatedPattern = origPattern.SelectMany(x =>
            Enumerable.Repeat(x, itemCount)).ToArray();

            MonsterTeamGainsItemsArtifactManager.dropPattern = duplicatedPattern;

            Logger.LogError("dropPattern has this many elements: " + MonsterTeamGainsItemsArtifactManager.dropPattern.Length);
        }

        private void MonsterTeamGainsItemsArtifactManager_EnsureMonsterItemCountMatchesStageCount(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchAdd()))
            {
                c.Index++;
                c.Emit(OpCodes.Ldc_I4, itemCount);
                c.Emit(OpCodes.Mul);
            }
            else
            {
                Logger.LogError("Failed to apply Evolution Item Count 3 hook");
            }
        }

        private void MonsterTeamGainsItemsArtifactManager_GrantMonsterTeamItem(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld(typeof(PickupDef), "itemIndex"),
                x => x.MatchLdcI4(1)))
            {
                c.Index += 2;
                c.EmitDelegate<Func<int, int>>((useless) =>
                {
                    return itemCount;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Evolution Item Count 1 hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcI4(1),
                x => x.MatchAdd()))
            {
                c.Index++;
                c.EmitDelegate<Func<int, int>>((useless) =>
                {
                    return itemCount;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Evolution Item Count 2 hook");
            }
        }
    }
}