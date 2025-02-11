/*using EntityStates.AI;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2.Navigation;
using RoR2.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using RoR2;
using RoR2.CharacterAI;
using UnityEngine;
using BepInEx.Configuration;

namespace WellRoundedBalance.Mechanics.Allies
{
    internal class PingOrdering : MechanicBase<PingOrdering>
    {
        [ConfigField("Order Button", "", KeyCode.Mouse3)]
        public static KeyCode keyCode;

        public static Dictionary<CharacterMaster, List<AwaitOrders>> subordinateDict = new();

        public static KeyboardShortcut button;

        public override string Name => ":: Mechanics :::::::::::::: Ping Ordering";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            button = new KeyboardShortcut(keyCode);
            IL.RoR2.UI.PingIndicator.RebuildPing += PingIndicator_RebuildPing;
            On.RoR2.Highlight.GetColor += Highlight_GetColor;
            On.RoR2.PlayerCharacterMasterController.Update += PlayerCharacterMasterController_Update;
        }

        private void PlayerCharacterMasterController_Update(On.RoR2.PlayerCharacterMasterController.orig_Update orig, PlayerCharacterMasterController self)
        {
            orig(self);
            if (button.IsPressed())
            {
                var minionGroup = MinionOwnership.MinionGroup.FindGroup(self.master.netId);
                if (minionGroup != null)
                {
                    foreach (var minion in minionGroup.members)
                    {
                        if (minion?.gameObject)
                        {
                            var stat = new AwaitOrders();
                            if (!subordinateDict.ContainsKey(self.master))
                            {
                                subordinateDict.Add(self.master, new List<AwaitOrders>());
                            }
                            subordinateDict[self.master].Add(stat);
                            minion?.gameObject?.GetComponent<EntityStateMachine>()?.SetState(stat);
                        }
                    }
                }
            }
        }

        private Color Highlight_GetColor(On.RoR2.Highlight.orig_GetColor orig, Highlight self)
        {
            var ret = orig(self);
            if (ret == Color.magenta && self.highlightColor == (Highlight.HighlightColor)(669))
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
                if (subordinateDict.ContainsKey(ownerMaster) && subordinateDict[ownerMaster].Any())
                {
                    var isEnemy = TeamManager.IsTeamEnemy(ownerMaster.teamIndex, targetMaster.teamIndex);
                    subordinateDict[ownerMaster].ForEach((m) => m.SubmitOrder(isEnemy ? AwaitOrders.Orders.Attack : AwaitOrders.Orders.Assist, self.pingTarget));
                    subordinateDict.Remove(ownerMaster);
                    //Chat.AddMessage(string.Format(Language.GetString("PING_ORDER_ENEMY"),self.pingText.text,Util.GetBestBodyName(subordinateDict[ownerMaster].characterBody),Util.GetBestBodyName(targetMaster.characterBody));
                    self.pingDuration = 1f;
                    return true;
                }
                else if (targetMaster.GetComponent<BaseAI>()?.leader.characterBody?.master == ownerMaster)
                {
                    self.pingOwner.GetComponent<PingerController>().pingIndicator = null;
                    self.pingOwner.GetComponent<PingerController>().pingStock++;
                    subordinateDict.Add(ownerMaster, new List<AwaitOrders>() { new AwaitOrders(self) });
                    targetMaster.GetComponent<EntityStateMachine>().SetState(subordinateDict[ownerMaster][0]);
                    self.pingColor = Color.cyan;
                    self.pingDuration = float.PositiveInfinity;
                    self.enemyPingGameObjects[0].GetComponent<SpriteRenderer>().color = Color.cyan;
                    self.pingHighlight.highlightColor = (Highlight.HighlightColor)(669);
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
                    subordinateDict[ownerMaster].ForEach((m) => m.SubmitOrder(AwaitOrders.Orders.Move, null, self.pingOrigin));
                    subordinateDict.Remove(ownerMaster);
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

        public PingIndicator ping;

        public AwaitOrders(PingIndicator ing = null)
        {
            ping = ing;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            if (!ping)
            {
                ping = UnityEngine.Object.Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/PingIndicator")).GetComponent<PingIndicator>();
                ping.pingOwner = characterMaster.minionOwnership?.ownerMaster?.gameObject;
                ping.pingOrigin = body.transform.position;
                ping.pingNormal = Vector3.zero;
                ping.pingTarget = body.gameObject;
                ping.transform.position = body.transform.position;
                ping.positionIndicator.targetTransform = body.transform;
                ping.positionIndicator.defaultPosition = body.transform.position;
                ping.targetTransformToFollow = body.coreTransform;
                ping.pingDuration = float.PositiveInfinity;
                ping.fixedTimer = float.PositiveInfinity;
                ping.pingColor = Color.cyan;
                ping.pingText.color = ping.textBaseColor * ping.pingColor;
                ping.pingText.text = Util.GetBestMasterName(characterMaster.minionOwnership?.ownerMaster);
                ping.pingObjectScaleCurve.enabled = false;
                ping.pingObjectScaleCurve.enabled = true;
                ping.pingHighlight.highlightColor = (Highlight.HighlightColor)(669);
                ping.pingHighlight.targetRenderer = body.modelLocator?.modelTransform?.GetComponentInChildren<CharacterModel>()?.baseRendererInfos?.First((r) => !r.ignoreOverlays).renderer;
                ping.pingHighlight.strength = 1f;
                ping.pingHighlight.isOn = true;
                foreach (var gameObject in ping.enemyPingGameObjects)
                {
                    gameObject.SetActive(true);
                    var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                    if (spriteRenderer)
                    {
                        spriteRenderer.color = Color.cyan;
                    }
                    var particleSystem = gameObject.GetComponent<ParticleSystem>();
                    if (particleSystem)
                    {
                        var main = particleSystem.main;
                        var startColor = main.startColor;
                        startColor.colorMax = Color.cyan;
                        startColor.colorMin = Color.cyan;
                        startColor.color = Color.cyan;
                    }
                }
            }
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
                        ai.SetGoalPosition(targetPosition);
                        ai.localNavigator.targetPosition = agent.output.nextPosition ?? ai.localNavigator.targetPosition;
                        if (!agent.output.targetReachable)
                        {
                            agent.InvalidatePath();
                        }
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
            if (ping)
            {
                ping.fixedTimer = 0f;
            }
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
}*/