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
            IL.RoR2.CharacterAI.BaseAI.FindEnemyHurtBox += BaseAI_FindEnemyHurtBox1;
        }

        private void BaseAI_FindEnemyHurtBox1(ILContext il)
        {
            ILCursor c = new(il);

            ILLabel label = null;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdarg(0),
                x => x.MatchLdfld("RoR2.CharacterAI.BaseAI", "enemySearch"),
                x => x.MatchLdcI4(1)))
            {
                label = c.MarkLabel();

                c.GotoPrev(MoveType.Before,
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld("RoR2.CharacterAI.BaseAI", "enemySearch"),
                    x => x.MatchLdflda("RoR2.BullseyeSearch", "teamMaskFilter"));

                c.EmitDelegate<Func<bool>>(() =>
                {
                    return RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.friendlyFireArtifactDef);
                });

                c.Emit(OpCodes.Brtrue, label);
            }
            else
            {
                Logger.LogError("Failed to apply Artifact of Chaos CHAOS hook");
            }
        }

        private HurtBox BaseAI_FindEnemyHurtBox(On.RoR2.CharacterAI.BaseAI.orig_FindEnemyHurtBox orig, BaseAI self, float maxDistance, bool full360Vision, bool filterByLoS)
        {
            if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.friendlyFireArtifactDef))
            {
                if (!self.body)
                    return null;
                self.enemySearch.viewer = self.body;
                self.enemySearch.teamMaskFilter = TeamMask.allButNeutral;
                self.enemySearch.sortMode = BullseyeSearch.SortMode.Distance;
                self.enemySearch.minDistanceFilter = 0f;
                self.enemySearch.maxDistanceFilter = maxDistance;
                self.enemySearch.searchOrigin = self.bodyInputBank.aimOrigin;
                self.enemySearch.searchDirection = self.bodyInputBank.aimDirection;
                self.enemySearch.maxAngleFilter = full360Vision ? 180f : 90f;
                self.enemySearch.filterByLoS = filterByLoS;
                self.enemySearch.RefreshCandidates();
                return self.enemySearch.GetResults().FirstOrDefault();
            }
            else
                return orig(self, maxDistance, full360Vision, filterByLoS);
        }
    }
}