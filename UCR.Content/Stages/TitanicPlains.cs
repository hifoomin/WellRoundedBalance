using R2API;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateCustomRun.Stages
{
    public static class TitanicPlains
    {
        public static void AddBison()
        {
            var bizzozeron = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscBison"),
                selectionWeight = 1,
                preventOverhead = false,
                minimumStageCompletions = 0,
                requiredUnlockable = "",
                forbiddenUnlockable = "",
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };
            var bizzozeroncard = new DirectorAPI.DirectorCardHolder
            {
                Card = bizzozeron,
                MonsterCategory = DirectorAPI.MonsterCategory.Minibosses,
                InteractableCategory = DirectorAPI.InteractableCategory.None
            };
            DirectorAPI.MonsterActions += (List<DirectorAPI.DirectorCardHolder> list, DirectorAPI.StageInfo stage) =>
            {
                if (stage.stage == DirectorAPI.Stage.TitanicPlains)
                {
                    if (!list.Contains(bizzozeroncard))
                    {
                        list.Add(bizzozeroncard);
                    }
                }
            };
        }
    }
}