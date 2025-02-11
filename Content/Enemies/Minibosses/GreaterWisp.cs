using System;
using BepInEx;

namespace WellRoundedBalance.Enemies.Minibosses
{
    internal class GreaterWisp : EnemyBase<GreaterWisp>
    {
        public override string Name => ":: Minibosses :: Greater Wisp";
        [ConfigField("No Stage One", true)]
        public static bool noStageOne;
        public static CharacterSpawnCard cscGreaterWisp;
        public static List<DirectorCardCategorySelection> whatWeHaveModified = new();

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.GreaterWispMonster.ChargeCannons.OnEnter += ChargeCannons_OnEnter;
            On.EntityStates.GreaterWispMonster.FireCannons.OnEnter += FireCannons_OnEnter;
            Changes();
        }

        private void FireCannons_OnEnter(On.EntityStates.GreaterWispMonster.FireCannons.orig_OnEnter orig, EntityStates.GreaterWispMonster.FireCannons self)
        {
            if (!Main.IsInfernoDef())
                self.damageCoefficient = 3.7f;
            orig(self);
        }

        private void ChargeCannons_OnEnter(On.EntityStates.GreaterWispMonster.ChargeCannons.orig_OnEnter orig, EntityStates.GreaterWispMonster.ChargeCannons self)
        {
            if (!Main.IsInfernoDef())
                self.baseDuration = 1.75f;
            orig(self);
        }

        private void Changes()
        {
            if (noStageOne) {
                cscGreaterWisp = Utils.Paths.CharacterSpawnCard.cscGreaterWisp.Load<CharacterSpawnCard>();

                On.RoR2.DirectorCardCategorySelection.OnSelected += OnSelected;
            }
        }

        private void OnSelected(On.RoR2.DirectorCardCategorySelection.orig_OnSelected orig, DirectorCardCategorySelection self, ClassicStageInfo stageInfo)
        {
            if (!whatWeHaveModified.Contains(self)) {
                foreach (var cat in self.categories) {
                    foreach (var card in cat.cards) {
                        if (card.spawnCard == cscGreaterWisp) {
                            card.minimumStageCompletions = 1;
                        }
                    }
                }

                whatWeHaveModified.Add(self);
            }

            orig(self, stageInfo);
        }
    }
}