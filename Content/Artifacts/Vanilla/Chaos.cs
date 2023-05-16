using System;

namespace WellRoundedBalance.Artifacts.Vanilla
{
    internal class Chaos : ArtifactEditBase<Chaos>
    {
        public override string Name => ":: Artifacts : Chaos";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CharacterAI.BaseAI.FindEnemyHurtBox += BaseAI_FindEnemyHurtBox;
        }

        private HurtBox BaseAI_FindEnemyHurtBox(On.RoR2.CharacterAI.BaseAI.orig_FindEnemyHurtBox orig, BaseAI self, float maxDistance, bool full360Vision, bool filterByLoS)
        {
            if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.friendlyFireArtifactDef))
            {
                if (!self.body)
                    return null;
                self.enemyAttentionDuration = 1f;
                self.enemySearch.viewer = self.body;
                self.enemySearch.teamMaskFilter = TeamMask.allButNeutral;
                self.enemySearch.sortMode = BullseyeSearch.SortMode.Distance;
                self.enemySearch.minDistanceFilter = 0f;
                self.enemySearch.maxDistanceFilter = maxDistance;
                self.enemySearch.searchOrigin = self.bodyInputBank.aimOrigin;
                self.enemySearch.searchDirection = self.bodyInputBank.aimDirection;
                self.enemySearch.maxAngleFilter = full360Vision ? 180f : 90f;
                self.enemySearch.filterByLoS = filterByLoS;
                self.enemySearch.FilterOutGameObject(self.gameObject);
                self.enemySearch.RefreshCandidates();
                return self.enemySearch.GetResults().FirstOrDefault();
            }
            else
                return orig(self, maxDistance, full360Vision, filterByLoS);
        }
    }
}