using System;

namespace WellRoundedBalance.Enemies.Bosses
{
    internal class ClayDunestrider : EnemyBase<ClayDunestrider>
    {
        public override string Name => "::: Bosses :: Clay Dunestrider";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.EntityStates.ClayBoss.Recover.FireTethers += Recover_FireTethers;
            IL.RoR2.TarTetherController.DoDamageTick += TarTetherController_DoDamageTick;
            RoR2.CharacterMaster.onStartGlobal += CharacterMaster_onStartGlobal;
        }

        private void TarTetherController_DoDamageTick(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdnull(),
                x => x.MatchStfld(typeof(DamageInfo), "attacker")))
            {
                c.Index++;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<GameObject, TarTetherController, GameObject>>((orig, self) =>
                {
                    return self.ownerRoot;
                });
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Clay Dunestrider Suck Fix 1 hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdnull(),
                x => x.MatchStfld(typeof(DamageInfo), "inflictor")))
            {
                c.Index++;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<GameObject, TarTetherController, GameObject>>((orig, self) =>
                {
                    return self.ownerRoot;
                });
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Clay Dunestrider Suck Fix 2 hook");
            }
        }

        private void Recover_FireTethers(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                 x => x.MatchCallvirt<BullseyeSearch>("RefreshCandidates")))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<BullseyeSearch, EntityStates.ClayBoss.Recover, BullseyeSearch>>((search, self) =>
                {
                    if (self.GetTeam() == TeamIndex.Player)
                    {
                        search.teamMaskFilter.RemoveTeam(TeamIndex.Player);
                    }
                    return search;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Clay Dunestrider Suck hook");
            }
        }

        private void CharacterMaster_onStartGlobal(CharacterMaster master)
        {
            if (Main.IsInfernoDef())
            {
                return;
            }
            switch (master.name)
            {
                case "ClayBossMaster(Clone)":
                    AISkillDriver Suck1 = (from x in master.GetComponents<AISkillDriver>()
                                           where x.customName == "SukFriends"
                                           select x).First();
                    Suck1.noRepeat = true;

                    AISkillDriver Suck2 = (from x in master.GetComponents<AISkillDriver>()
                                           where x.customName == "SukEnemies"
                                           select x).First();
                    Suck2.noRepeat = true;
                    break;
            }
        }
    }
}