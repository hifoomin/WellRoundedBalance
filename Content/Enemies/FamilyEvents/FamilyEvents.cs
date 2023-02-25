/*
using BepInEx.Configuration;
using System;

namespace WellRoundedBalance.Enemies.FamilyEvents
{
    public static class FamilyEvents
    {
        public static ConfigEntry<bool> enable { get; set; }
        public static FamilyDirectorCardCategorySelection bossFamilyDCCS;
        public static ClassicStageInfo.MonsterFamily bossFamily;

        public static void Init()
        {
            var larvaFamilyEvent = Utils.Paths.FamilyDirectorCardCategorySelection.dccsAcidLarvaFamily.Load<FamilyDirectorCardCategorySelection>();
            larvaFamilyEvent.minimumStageCompletion = int.MaxValue;

            ClassicStageInfo.monsterFamilyChance = 0.03f; // 0.02f in vanilla

            AddBossFamilyEvent();
            On.RoR2.ClassicStageInfo.OnEnable += ClassicStageInfo_OnEnable;

            // ok so uh it works perfectly fine, looks perfectly fine at runtime except it doesnt do anything with 100% chance and really high selection weight idk why
        }

        private static void ClassicStageInfo_OnEnable(On.RoR2.ClassicStageInfo.orig_OnEnable orig, ClassicStageInfo self)
        {
            Array.Resize(ref self.possibleMonsterFamilies, self.possibleMonsterFamilies.Length + 1);
            self.possibleMonsterFamilies[self.possibleMonsterFamilies.Length - 1] = bossFamily;
            orig(self);
        }

        private static void AddBossFamilyEvent()
        {
            bossFamilyDCCS = ScriptableObject.CreateInstance<FamilyDirectorCardCategorySelection>();
            bossFamilyDCCS.selectionChatString = "FAMILY_BOSS";
            bossFamilyDCCS.minimumStageCompletion = 3;
            bossFamilyDCCS.maximumStageCompletion = int.MaxValue;
            bossFamilyDCCS.AddCategory("Champions", 1f);

            var beetleQueenDirectorCard = new DirectorCard()
            {
                spawnCard = Utils.Paths.CharacterSpawnCard.cscBeetleQueen.Load<CharacterSpawnCard>(),
                selectionWeight = 1,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = false,
                minimumStageCompletions = 0,
                requiredUnlockable = null,
                forbiddenUnlockable = null
            };

            var clayDunestriderDirectorCard = new DirectorCard()
            {
                spawnCard = Utils.Paths.CharacterSpawnCard.cscClayBoss.Load<CharacterSpawnCard>(),
                selectionWeight = 1,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = false,
                minimumStageCompletions = 0,
                requiredUnlockable = null,
                forbiddenUnlockable = null
            };

            var grandparentDirectorCard = new DirectorCard()
            {
                spawnCard = Utils.Paths.CharacterSpawnCard.cscGrandparent.Load<CharacterSpawnCard>(),
                selectionWeight = 1,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = false,
                minimumStageCompletions = 0,
                requiredUnlockable = null,
                forbiddenUnlockable = null
            };

            var grovetenderDirectorCard = new DirectorCard()
            {
                spawnCard = Utils.Paths.CharacterSpawnCard.cscGravekeeper.Load<CharacterSpawnCard>(),
                selectionWeight = 1,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = false,
                minimumStageCompletions = 0,
                requiredUnlockable = null,
                forbiddenUnlockable = null
            };

            var impOverlordDirectorCard = new DirectorCard()
            {
                spawnCard = Utils.Paths.CharacterSpawnCard.cscImpBoss.Load<CharacterSpawnCard>(),
                selectionWeight = 1,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = false,
                minimumStageCompletions = 0,
                requiredUnlockable = null,
                forbiddenUnlockable = null
            };

            var magmaWormDirectorCard = new DirectorCard()
            {
                spawnCard = Utils.Paths.CharacterSpawnCard.cscMagmaWorm.Load<CharacterSpawnCard>(),
                selectionWeight = 1,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = false,
                minimumStageCompletions = 0,
                requiredUnlockable = null,
                forbiddenUnlockable = null
            };

            var OverloadingWormDirectorCard = new DirectorCard()
            {
                spawnCard = Utils.Paths.CharacterSpawnCard.cscElectricWorm.Load<CharacterSpawnCard>(),
                selectionWeight = 1,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = false,
                minimumStageCompletions = 0,
                requiredUnlockable = null,
                forbiddenUnlockable = null
            };

            var scavengerDirectorCard = new DirectorCard()
            {
                spawnCard = Utils.Paths.CharacterSpawnCard.cscScav.Load<CharacterSpawnCard>(),
                selectionWeight = 1,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = false,
                minimumStageCompletions = 0,
                requiredUnlockable = null,
                forbiddenUnlockable = null
            };

            var solusControlUnitDirectorCard = new DirectorCard()
            {
                spawnCard = Utils.Paths.CharacterSpawnCard.cscRoboBallBoss.Load<CharacterSpawnCard>(),
                selectionWeight = 1,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = false,
                minimumStageCompletions = 0,
                requiredUnlockable = null,
                forbiddenUnlockable = null
            };

            var stoneTitanDirectorCard = new DirectorCard()
            {
                spawnCard = Utils.Paths.CharacterSpawnCard.cscTitanGolemPlains.Load<CharacterSpawnCard>(),
                selectionWeight = 1,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = false,
                minimumStageCompletions = 0,
                requiredUnlockable = null,
                forbiddenUnlockable = null
            };

            var voidDevastatorSpawnCard = new DirectorCard()
            {
                spawnCard = Utils.Paths.CharacterSpawnCard.cscVoidMegaCrab.Load<CharacterSpawnCard>(),
                selectionWeight = 1,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = false,
                minimumStageCompletions = 0,
                requiredUnlockable = null,
                forbiddenUnlockable = null
            };

            var wanderingVagrantSpawnCard = new DirectorCard()
            {
                spawnCard = Utils.Paths.CharacterSpawnCard.cscVagrant.Load<CharacterSpawnCard>(),
                selectionWeight = 1,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = false,
                minimumStageCompletions = 0,
                requiredUnlockable = null,
                forbiddenUnlockable = null
            };

            var xiConstructSpawnCard = new DirectorCard()
            {
                spawnCard = Utils.Paths.CharacterSpawnCard.cscMegaConstruct.Load<CharacterSpawnCard>(),
                selectionWeight = 1,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                preventOverhead = false,
                minimumStageCompletions = 0,
                requiredUnlockable = null,
                forbiddenUnlockable = null
            };

            var cards = new List<DirectorCard>
            {
                beetleQueenDirectorCard,
                clayDunestriderDirectorCard,
                grandparentDirectorCard,
                grovetenderDirectorCard,
                impOverlordDirectorCard,
                magmaWormDirectorCard,
                OverloadingWormDirectorCard,
                scavengerDirectorCard,
                solusControlUnitDirectorCard,
                stoneTitanDirectorCard,
                voidDevastatorSpawnCard,
                wanderingVagrantSpawnCard,
                xiConstructSpawnCard
            };

            bossFamilyDCCS.categories[0].cards = cards.ToArray();

            bossFamily = new ClassicStageInfo.MonsterFamily()
            {
                monsterFamilyCategories = bossFamilyDCCS,
                familySelectionChatString = "FAMILY_BOSS",
                minimumStageCompletion = 3,
                maximumStageCompletion = 99,
                selectionWeight = 1f
            };

            LanguageAPI.Add("FAMILY_BOSS", "You feel an ominous presence...");
        }
    }
}
*/