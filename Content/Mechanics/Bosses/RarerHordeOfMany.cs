using R2API.Utils;
using System;

namespace WellRoundedBalance.Mechanics.Bosses
{
    public class RarerHordeOfMany : MechanicBase<RarerHordeOfMany>
    {
        public override string Name => ":: Mechanics ::::: Rarer Horde of Many";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CombatDirector.Spawn += CombatDirector_Spawn;
            IL.RoR2.TeleporterInteraction.ChargingState.OnEnter += ChargingState_OnEnter;
        }

        public static List<float> latestCosts = new();

        private bool CombatDirector_Spawn(On.RoR2.CombatDirector.orig_Spawn orig, CombatDirector self, SpawnCard spawnCard, EliteDef eliteDef, Transform spawnTarget, DirectorCore.MonsterSpawnDistance spawnDistance, bool preventOverhead, float valueMultiplier, DirectorPlacementRule.PlacementMode placementMode)
        {
            var ret = orig(self, spawnCard, eliteDef, spawnTarget, spawnDistance, preventOverhead, valueMultiplier, placementMode);
            if (spawnCard)
            {
                latestCosts.Add(spawnCard.directorCreditCost);
                Logger.LogError("added " + spawnCard.directorCreditCost + " to latest costs");

                if (latestCosts.Count > 6)
                {
                    latestCosts.RemoveAt(0);
                    Logger.LogError("   >removing oldest cost<   ");
                }
                // max capacity of 6
            }

            return ret;
        }

        private void ChargingState_OnEnter(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(600f)))
            {
                c.Index++;
                c.EmitDelegate<Func<float, float>>((self) =>
                {
                    var average = latestCosts.Average();
                    return self + average;
                    // averaged latest costs are added on top to make proper bosses more common
                });
            }
            else
            {
                Logger.LogError("Failed to apply Teleporter Boss Credits hook");
            }
        }
    }
}