using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;

namespace UltimateCustomRun.Enemies.Bosses
{
    public static class BeetleQueen
    {
        public static CharacterMaster master;
        public static SpawnCard beeb;
        public static SpawnCard guard;
        public static CharacterBody body;
        public static CharacterBody bodydir;
        public static void Buff()
        {
            master = Resources.Load<CharacterMaster>("prefabs/charactermasters/BeetleQueenMaster").GetComponent<CharacterMaster>();

            AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                where x.customName == "Chase"
                                select x).First();
            ai.minDistance = 0f;

            body = Resources.Load<CharacterBody>("prefabs/characterbodies/BeetleQueen2Body");
            var a = body.GetComponent<CharacterBody>();
            a.mainRootSpeed = 12f;
            a.baseMoveSpeed = 12f;
            a.rootMotionInMainState = false;
            var b = body.GetComponent<CharacterDirection>();
            b.turnSpeed = 180f;
            b.driveFromRootRotation = false;

            beeb = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscBeetle");
            guard = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscBeetleGuard");

            On.EntityStates.BeetleQueenMonster.SummonEggs.SummonEgg += (orig, self) =>
            {
                if (self.isAuthority)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (Random.Range(1f, 10f) >= 5f)
                        {
                            EntityStates.BeetleQueenMonster.SummonEggs.spawnCard = beeb;
                        }
                        else
                        {
                            EntityStates.BeetleQueenMonster.SummonEggs.spawnCard = guard;
                        }

                    }
                }
                EntityStates.BeetleQueenMonster.SummonEggs.baseDuration = 2f;
                EntityStates.BeetleQueenMonster.SummonEggs.summonDuration = 3f;
                EntityStates.BeetleQueenMonster.SummonEggs.randomRadius = 15f;
                orig(self);
                self.outer.SetNextState(new EntityStates.BeetleQueenMonster.BeginBurrow());
            };

            On.EntityStates.BeetleQueenMonster.BeginBurrow.OnEnter += (orig, self) =>
            {
                EntityStates.BeetleQueenMonster.BeginBurrow.duration = 5f;
                EntityStates.BeetleQueenMonster.BeginBurrow.burrowPrefab = Resources.Load<GameObject>("prefabs/effects/BeetleQueenBurrow");
                orig(self);
            };

            On.EntityStates.BeetleQueenMonster.WeakState.OnEnter += (orig, self) =>
            {
                EntityStates.BeetleQueenMonster.WeakState.weakDuration = 0f;
                EntityStates.BeetleQueenMonster.WeakState.weakToIdleTransitionDuration = 0.1f;

                orig(self);
            };

            On.EntityStates.BeetleQueenMonster.SpawnWards.OnEnter += (orig, self) =>
            {
                EntityStates.BeetleQueenMonster.SpawnWards.baseDuration = 4f;
                EntityStates.BeetleQueenMonster.SpawnWards.orbCountMax = 8;
                orig(self);
            };

        }
    }
}
