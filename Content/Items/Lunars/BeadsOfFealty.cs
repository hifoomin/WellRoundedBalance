using MonoMod.Cil;
using R2API.Utils;
using RoR2;

namespace WellRoundedBalance.Items.Lunars
{
    public class BeadsOfFealty : ItemBase<BeadsOfFealty>
    {
        public override string Name => ":: Items ::::: Lunars :: Beads of Fealty";
        public override ItemDef InternalPickup => RoR2Content.Items.LunarTrinket;

        public override string PickupText => "Gain access to a hidden path... <color=#FF7F7F>BUT enemies are more numerous.</color>\n";

        public override string DescText => (baseRegenerationGain > 0 ? "Increase <style=cIsHealing>base health regeneration</style> by <style=cIsHealing>+" + baseRegenerationGain + " hp/s</style>." + (regenerationGainPerStack > 0 ? " <style=cStack>(+" + regenerationGainPerStack + " per stack)</style> " : " ") : " ") +
                                           " Increase <style=cIsUtility>combat director credits</style> by <style=cIsUtility>" + d(baseCombatDirectorCreditMultiplier) + "</style> " +
                                           (combatDirectorCreditMultiplierAddPerStack > 0 ? "<style=cStack>(+" + d(combatDirectorCreditMultiplierAddPerStack) + " per stack)</style>. " : ". ") +
                                           "Gain access to a <style=cIsUtility>hidden path</style> that yields <style=cIsUtility>1</style> <style=cStack>(+1 per stack)</style> <style=cIsUtility>rewards</style>.";

        [ConfigField("Base Regeneration Gain", 1.5f)]
        public static float baseRegenerationGain;

        [ConfigField("Regeneration Gain Per Stack", 0f)]
        public static float regenerationGainPerStack;

        [ConfigField("Base Combat Director Credit Multiplier Add", 0.2f)]
        public static float baseCombatDirectorCreditMultiplier;

        [ConfigField("Combat Director Credit Multiplier Add Per Stack", 0.2f)]
        public static float combatDirectorCreditMultiplierAddPerStack;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.CombatDirector.Awake += CombatDirector_Awake;
            On.RoR2.ScriptedCombatEncounter.BeginEncounter += ScriptedCombatEncounter_BeginEncounter;
            On.EntityStates.Missions.LunarScavengerEncounter.FadeOut.OnEnter += FadeOut_OnEnter;
            On.EntityStates.ScavMonster.Death.OnPreDestroyBodyServer += Death_OnPreDestroyBodyServer;
        }

        private void Death_OnPreDestroyBodyServer(On.EntityStates.ScavMonster.Death.orig_OnPreDestroyBodyServer orig, EntityStates.ScavMonster.Death self)
        {
            if (self.outer.name.StartsWith("ScavLunar"))
            {
                self.shouldDropPack = true;
            }
            orig(self);
        }

        public static int rewardCounter;

        private void FadeOut_OnEnter(On.EntityStates.Missions.LunarScavengerEncounter.FadeOut.orig_OnEnter orig, EntityStates.Missions.LunarScavengerEncounter.FadeOut self)
        {
            orig(self);
            if (rewardCounter > 1)
            {
                var newCounter = rewardCounter;
                if (newCounter > 10)
                {
                    newCounter = 10;
                }
                var newDuration = EntityStates.Missions.LunarScavengerEncounter.FadeOut.duration / 4f * (newCounter - 1);
                Reflection.SetFieldValue(self, "duration", EntityStates.Missions.LunarScavengerEncounter.FadeOut.duration + newDuration);
            }
        }

        private void ScriptedCombatEncounter_BeginEncounter(On.RoR2.ScriptedCombatEncounter.orig_BeginEncounter orig, ScriptedCombatEncounter self)
        {
            if (self.name.Equals("ScavLunarEncounter"))
            {
                var stack = Util.GetItemCountForTeam(TeamIndex.Player, RoR2Content.Items.LunarTrinket.itemIndex, false);
                rewardCounter = stack;
                for (int i = 0; i < stack; i++)
                {
                    self.hasSpawnedServer = false;
                    orig(self);
                }
            }
            else
            {
                orig(self);
            }
        }

        private void CombatDirector_Awake(On.RoR2.CombatDirector.orig_Awake orig, CombatDirector self)
        {
            orig(self);
            var stack = Util.GetItemCountForTeam(TeamIndex.Player, RoR2Content.Items.LunarTrinket.itemIndex, false);
            if (stack > 0)
            {
                self.creditMultiplier += baseCombatDirectorCreditMultiplier + combatDirectorCreditMultiplierAddPerStack * (stack - 1);
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.LunarTrinket);
                if (stack > 0)
                {
                    var regenStack = baseRegenerationGain + (regenerationGainPerStack * (stack - 1));
                    args.baseRegenAdd += regenStack + 0.2f * regenStack * (sender.level - 1);
                }
            }
        }
    }
}