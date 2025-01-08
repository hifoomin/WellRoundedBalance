using System.Collections;
using R2API.Utils;

namespace WellRoundedBalance.Mechanics.Interactables
{
    internal class FasterInteractables : MechanicBase<FasterInteractables>
    {
        public override string Name => ":: Mechanics ::::::::::::: Faster Interactables";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.TimerQueue.CreateTimer += TimerQueue_CreateTimer;
            On.RoR2.Stage.Start += Stage_Start;
            On.EntityStates.Duplicator.Duplicating.DropDroplet += Duplicating_DropDroplet;
            On.RoR2.ShrineChanceBehavior.AddShrineStack += ShrineChanceBehavior_AddShrineStack;
            On.RoR2.EntityLogic.DelayedEvent.CallDelayed += DelayedEvent_CallDelayed;
        }

        private void DelayedEvent_CallDelayed(On.RoR2.EntityLogic.DelayedEvent.orig_CallDelayed orig, RoR2.EntityLogic.DelayedEvent self, float timer)
        {
            if (self.ToString().Contains("Duplicator"))
            {
                //Nothing
            }
            else
            {
                orig(self, timer);
            }
        }

        private void ShrineChanceBehavior_AddShrineStack(On.RoR2.ShrineChanceBehavior.orig_AddShrineStack orig, ShrineChanceBehavior self, Interactor activator)
        {
            orig(self, activator);
            self.refreshTimer = 0.3f;
        }

        private void Duplicating_DropDroplet(On.EntityStates.Duplicator.Duplicating.orig_DropDroplet orig, EntityStates.Duplicator.Duplicating self)
        {
            orig(self);

            if (NetworkServer.active && self.hasDroppedDroplet)
            {
                self.GetComponent<PurchaseInteraction>().Networkavailable = true;
            }
        }

        private IEnumerator Stage_Start(On.RoR2.Stage.orig_Start orig, Stage self)
        {
            yield return orig(self);
            typeof(EntityStates.Duplicator.Duplicating).SetFieldValue("initialDelayDuration", 0.3f);
            typeof(EntityStates.Duplicator.Duplicating).SetFieldValue("timeBetweenStartAndDropDroplet", 0.3f);

            typeof(EntityStates.Scrapper.WaitToBeginScrapping).SetFieldValue("duration", 0.3f);
            typeof(EntityStates.Scrapper.ScrappingToIdle).SetFieldValue("duration", 0.3f);
            typeof(EntityStates.Scrapper.Scrapping).SetFieldValue("duration", 0.3f);

            yield return null;
        }

        private TimerQueue.TimerHandle TimerQueue_CreateTimer(On.RoR2.TimerQueue.orig_CreateTimer orig, TimerQueue self, float time, System.Action action)
        {
            if (action.Target.ToString().Contains("LunarCauldron"))
            {
                return orig(self, 0.4f, action);
            }
            if (action.Target.ToString().Contains("ShrineCleanse"))
            {
                return orig(self, 0.5f, action);
            }
            return orig(self, time, action);
        }
    }
}