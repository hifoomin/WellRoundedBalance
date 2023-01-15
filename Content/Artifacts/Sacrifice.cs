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

        public float normalChance = 4f;
        public float swarmsChance = 2f;
        public float maxNormalChance = 7f;
        public float maxSwarmsChance = 3.5f;

        private void SacrificeArtifactManager_OnServerCharacterDeath(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(5f)))
            {
                c.EmitDelegate<Func<float, float>>((useless) =>
                {
                    return RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.swarmsArtifactDef) ? swarmsChance : normalChance;
                });

                if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchStloc(0)))
                {
                    c.EmitDelegate<Func<float, float>>((vanillaChance) =>
                    {
                        if (vanillaChance > 0f)
                        {
                            var newNormalChance = normalChance;
                            var newMaxNormalChance = maxNormalChance;

                            if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.swarmsArtifactDef))
                            {
                                newNormalChance = swarmsChance;
                                newMaxNormalChance = maxSwarmsChance;
                            }

                            if (vanillaChance < newNormalChance)
                            {
                                vanillaChance = newNormalChance;
                            }

                            if (vanillaChance > newMaxNormalChance)
                            {
                                vanillaChance = newMaxNormalChance;
                            }
                        }

                        return vanillaChance;
                    });
                }
                else
                {
                    Main.WRBLogger.LogError("Failed to apply Sacrifice All Drop Chances hook");
                }
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Sacrifice Base Drop Chance hook");
            }
        }
    }
}