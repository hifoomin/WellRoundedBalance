using System;

namespace WellRoundedBalance.Artifacts.Vanilla
{
    internal class Dissonance : ArtifactEditBase<Dissonance>
    {
        public override string Name => ":: Artifacts :::: Dissonance";

        public override void Init()
        {
            hermitCrab = new DirectorCard()
            {
                minimumStageCompletions = 0,
                preventOverhead = false,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                selectionWeight = 1,
                spawnCard = Utils.Paths.CharacterSpawnCard.cscHermitCrab.Load<CharacterSpawnCard>()
            };
            directorCardHolder = new()
            {
                Card = hermitCrab,
                MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters,
            };
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.ClassicStageInfo.HandleMixEnemyArtifact += ClassicStageInfo_HandleMixEnemyArtifact1;
            On.RoR2.ClassicStageInfo.HandleMixEnemyArtifact += ClassicStageInfo_HandleMixEnemyArtifact;
            Stage.onServerStageBegin += Stage_onServerStageBegin;
        }

        private void ClassicStageInfo_HandleMixEnemyArtifact1(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdstr("Basic Monsters")))
            {
                c.Next.Operand = "15920u9099";
            }

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdstr("Minibosses")))
            {
                c.Next.Operand = "75-136613";
            }

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdstr("Champions")))
            {
                c.Next.Operand = "09358-2-1";
            }
        }

        private void Stage_onServerStageBegin(Stage stage)
        {
            applied = false;
        }

        private void ClassicStageInfo_HandleMixEnemyArtifact(On.RoR2.ClassicStageInfo.orig_HandleMixEnemyArtifact orig, DirectorCardCategorySelection monsterCategories, Xoroshiro128Plus rng)
        {
            orig(monsterCategories, rng);
            if (!applied)
            {
                applied = true;

                DirectorAPI.AddCard(RoR2Content.mixEnemyMonsterCards, directorCardHolder);
            }
        }

        public static bool applied = false;
        public static DirectorCard hermitCrab;
        public static DirectorAPI.DirectorCardHolder directorCardHolder;
    }
}