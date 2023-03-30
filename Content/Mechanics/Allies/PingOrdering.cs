using EntityStates.AI;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2.Navigation;
using RoR2.UI;
using System;

namespace WellRoundedBalance.Mechanics.Allies
{
    internal class PingOrdering : MechanicBase
    {
        public static Dictionary<CharacterMaster, AwaitOrders> subordinateDict = new();
        public override string Name => ":: Mechanics :::::::::::::: Ping Ordering";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.UI.PingIndicator.RebuildPing += PingIndicator_RebuildPing;
            On.RoR2.Highlight.GetColor += Highlight_GetColor;
        }

        private Color Highlight_GetColor(On.RoR2.Highlight.orig_GetColor orig, Highlight self)
        {
            var ret = orig(self);
            if (ret == Color.magenta && self.highlightColor == (Highlight.HighlightColor)(451))
            {
                return Color.cyan + new Color(0.01f, 0, 0);
            }
            return ret;
        }

        private void PingIndicator_RebuildPing(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(x => x.MatchLdstr("PLAYER_PING_ENEMY"));
            c.Index += 6;
            ILLabel l = c.MarkLabel();
            c.GotoPrev(x => x.MatchLdstr("PLAYER_PING_ENEMY"));
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<PingIndicator, bool>>((PingIndicator self) =>
            {
                CharacterMaster ownerMaster = self.pingOwner.GetComponent<CharacterMaster>();
                CharacterMaster targetMaster = self.pingTarget.GetComponent<CharacterBody>().master;
                if (subordinateDict.ContainsKey(ownerMaster))
                {
                    bool flag = TeamManager.IsTeamEnemy(ownerMaster.teamIndex, targetMaster.teamIndex);
                    subordinateDict[ownerMaster].SubmitOrder(flag ? AwaitOrders.Orders.Attack : AwaitOrders.Orders.Assist, self.pingTarget);
                    subordinateDict.Remove(ownerMaster);
                    PingIndicator.instancesList.First((ping) => ping.pingOwner == self.pingOwner && ping.pingColor == Color.cyan).fixedTimer = 0f;
                    //Chat.AddMessage(string.Format(Language.GetString("PING_ORDER_ENEMY"),self.pingText.text,Util.GetBestBodyName(subordinateDict[ownerMaster].characterBody),Util.GetBestBodyName(targetMaster.characterBody));
                    self.pingDuration = 1f;
                    return true;
                }
                else if (targetMaster.GetComponent<BaseAI>()?.leader.characterBody?.master == ownerMaster)
                {
                    self.pingOwner.GetComponent<PingerController>().pingIndicator = null;
                    self.pingOwner.GetComponent<PingerController>().pingStock++;
                    subordinateDict.Add(ownerMaster, new AwaitOrders());
                    targetMaster.GetComponent<EntityStateMachine>().SetState(subordinateDict[ownerMaster]);
                    self.pingColor = Color.cyan;
                    self.pingDuration = float.PositiveInfinity;
                    self.enemyPingGameObjects[0].GetComponent<SpriteRenderer>().color = Color.cyan;
                    self.pingHighlight.highlightColor = (Highlight.HighlightColor)(451);
                    return true;
                }
                return false;
            });
            c.Emit(OpCodes.Brtrue, l);
            c.Index = 0;
            c.GotoNext(x => x.MatchLdstr("PLAYER_PING_DEFAULT"));
            c.Index += 5;
            l = c.MarkLabel();
            c.GotoPrev(x => x.MatchLdstr("PLAYER_PING_DEFAULT"));
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<PingIndicator, bool>>((PingIndicator self) =>
            {
                CharacterMaster ownerMaster = self.pingOwner.GetComponent<CharacterMaster>();
                if (subordinateDict.ContainsKey(ownerMaster))
                {
                    subordinateDict[ownerMaster].SubmitOrder(AwaitOrders.Orders.Move, null, self.pingOrigin);
                    subordinateDict.Remove(ownerMaster);
                    PingIndicator.instancesList.First((ping) => ping.pingOwner == self.pingOwner && ping.pingColor == Color.cyan).fixedTimer = 0f;
                    //Chat.AddMessage(string.Format(Language.GetString("PING_ORDER_ENEMY"),self.pingText.text,Util.GetBestBodyName(subordinateDict[ownerMaster].characterBody),Util.GetBestBodyName(targetMaster.characterBody));
                    self.pingDuration = 1f;
                    return true;
                }
                return false;
            });
            c.Emit(OpCodes.Brtrue, l);
        }
    }

    public class AwaitOrders : BaseAIState
    {
        public enum Orders
        {
            None,
            Move,
            Attack,
            Assist
        }

        public Orders order;

        public Vector3? targetPosition;

        public GameObject target;

        public float sprintThreshold;

        public override void OnEnter()
        {
            base.OnEnter();
            sprintThreshold = ai.skillDrivers.FirstOrDefault((drive) => drive.shouldSprint)?.minDistanceSqr ?? float.PositiveInfinity;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!target && !targetPosition.HasValue)
                AimAt(ref bodyInputs, ai.leader);
            switch (order)
            {
                case Orders.None:
                    {
                        return;
                    }
                case Orders.Attack:
                    {
                        ai.currentEnemy.gameObject = target;
                        ai.enemyAttention = ai.enemyAttentionDuration;
                        outer.SetNextState(new EntityStates.AI.Walker.Combat());
                        break;
                    }
                case Orders.Move:
                    {
                        if (!body || body.moveSpeed == 0)
                        {
                            outer.SetNextStateToMain();
                        }
                        BroadNavigationSystem.Agent agent = ai.broadNavigationAgent;
                        agent.currentPosition = ai.body.footPosition;
                        ai.localNavigator.targetPosition = agent.output.nextPosition ?? ai.localNavigator.targetPosition;
                        if (!agent.output.targetReachable)
                            agent.InvalidatePath();
                        ai.localNavigator.Update(cvAIUpdateInterval.value);
                        bodyInputs.moveVector = ai.localNavigator.moveVector;
                        float sqrMagnitude = (base.body.footPosition - targetPosition.Value).sqrMagnitude;
                        bodyInputs.pressSprint = sqrMagnitude > sprintThreshold;
                        if (ai.localNavigator.wasObstructedLastUpdate)
                            base.ModifyInputsForJumpIfNeccessary(ref bodyInputs);
                        float num = base.body.radius * base.body.radius * 4;
                        if (sqrMagnitude < num)
                            outer.SetNextStateToMain();
                        break;
                    }
                case Orders.Assist:
                    {
                        ai.buddy.gameObject = target;
                        ai.customTarget.gameObject = target;
                        outer.SetNextState(new EntityStates.AI.Walker.Combat());
                        break;
                    }
            };
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public void SubmitOrder(Orders command, GameObject target, Vector3? targetPosition = null)
        {
            order = command;
            this.target = target;
            this.targetPosition = targetPosition;
            if (targetPosition.HasValue)
            {
                BroadNavigationSystem.Agent agent = ai.broadNavigationAgent;
                agent.goalPosition = targetPosition;
                agent.InvalidatePath();
            }
        }
    }
}