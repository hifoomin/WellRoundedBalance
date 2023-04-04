using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Mechanics.Bosses
{
    internal class ForcedBossVariety : MechanicBase<ForcedBossVariety>
    {
        public override string Name => ":: Mechanics ::::: Forced Boss Variety";

        public static int maxStages = 2;	//Set to 2 so that theres always 1 boss left (since most stages have 3 boss choices)

        public static List<GameObject> previousCards = new();

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.Run.Start += Run_Start;
            IL.RoR2.CombatDirector.SetNextSpawnAsBoss += CombatDirector_SetNextSpawnAsBoss;
        }

        private void CombatDirector_SetNextSpawnAsBoss(ILContext il)
        {
            bool error = true;

            //Mark already-used cards as unavailable.
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.After,
                 x => x.MatchCallvirt<DirectorCard>("IsAvailable")))
            {
                c.Emit(OpCodes.Ldloc_3);//WeightedSelection<DirectorCard>.ChoiceInfo
                c.EmitDelegate<Func<bool, WeightedSelection<DirectorCard>.ChoiceInfo, bool>>((flag, choice) =>
                {
                    if (flag)
                    {
                        if (choice.value != null && choice.value.spawnCard != null)
                        {
                            if (previousCards.Contains(choice.value.spawnCard.prefab))
                            {
                                return false;
                            }
                        }
                    }
                    return flag;
                });

                //Add selected cards to the list
                if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchCall<CombatDirector>("PrepareNewMonsterWave")))
                {
                    c.EmitDelegate<Func<DirectorCard, DirectorCard>>(card =>
                    {
                        if (card != null && card.spawnCard != null)
                        {
                            if (previousCards.Count > 0 && previousCards.Count + 1 > maxStages)
                            {
                                previousCards.RemoveAt(0);
                            }
                            previousCards.Add(card.spawnCard.prefab);
                        }
                        return card;
                    });
                    error = false;
                }
            }

            if (error)
            {
                Logger.LogError("Failed to apply Forced Boss Variety hook");
            }
        }

        private void Run_Start(On.RoR2.Run.orig_Start orig, Run self)
        {
            orig(self);
            previousCards.Clear();
        }
    }
}