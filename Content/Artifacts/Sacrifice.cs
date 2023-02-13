using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Artifacts
{
    internal class Sacrifice : ArtifactBase
    {
        public override string Name => ":: Artifacts : Sacrifice";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.Artifacts.SacrificeArtifactManager.OnServerCharacterDeath += SacrificeArtifactManager_OnServerCharacterDeath;
        }

        public float baseDropChance = 4f;
        public float swarmDropChance = 2f;
        public float maxBaseDropChance = 7f;
        public float maxSwarmDropChance = 3.5f;

        private void SacrificeArtifactManager_OnServerCharacterDeath(ILContext il)
        {
            ILCursor c = new(il);

            //Change base drop chance
            c.GotoNext(MoveType.After,
                x => x.MatchLdcR4(5f)
                );
            c.EmitDelegate<Func<float, float>>(orig =>
            {
                return RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.swarmsArtifactDef) ? swarmDropChance : baseDropChance;
            });

            //Clamp final drop chance
            c.GotoNext(
                x => x.MatchStloc(0) //Called after GetExpAdjustedDropChancePercent
                );
            c.EmitDelegate<Func<float, float>>(orig =>
            {
                float finalDropChance = orig;

                if (orig > 0f)
                {
                    bool swarmsEnabled = RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.swarmsArtifactDef);

                    float baseChance = baseDropChance;
                    float maxChance = maxBaseDropChance;

                    if (swarmsEnabled)
                    {
                        baseChance = swarmDropChance;
                        maxChance = maxSwarmDropChance;
                    }

                    if (finalDropChance < baseChance)
                    {
                        finalDropChance = baseChance;
                    }

                    if (finalDropChance > maxChance)
                    {
                        finalDropChance = maxChance;
                    }
                }

                return finalDropChance;
            });
        }
    }
}