using R2API;
using RoR2;
using UnityEngine;
using System.Collections.Generic;

namespace UltimateCustomRun.Stages
{
    public static class RallypointDelta
    {
        public static void AddSCU()
        {
			var scu = new DirectorCard
			{
				spawnCard = Resources.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscRoboBallBoss"),
				selectionWeight = 1,
				allowAmbushSpawn = true,
				preventOverhead = false,
				minimumStageCompletions = 0,
				requiredUnlockable = "",
				forbiddenUnlockable = "",
				spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
			};
			var scucard = new DirectorAPI.DirectorCardHolder
			{
				Card = scu,
				MonsterCategory = DirectorAPI.MonsterCategory.Champions,
				InteractableCategory = DirectorAPI.InteractableCategory.None
			};
			DirectorAPI.MonsterActions += (List<DirectorAPI.DirectorCardHolder> list, DirectorAPI.StageInfo stage) =>
			{
				if (stage.stage == DirectorAPI.Stage.TitanicPlains)
				{
					if (!list.Contains(scucard))
					{
						list.Add(scucard);
					}
				}
			};
		}
    }
}
