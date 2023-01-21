using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine.UI;
using static R2API.DamageAPI;

namespace WellRoundedBalance.Elites
{
    internal class Glacial : EliteBase
    {
        public static BuffDef useless;
        public static BuffDef slow;
        public static GameObject iceExplosionPrefab;
        public override string Name => ":: Elites :::: Glacial";

        public override void Init()
        {
            iceExplosionPrefab = Utils.Paths.GameObject.AffixWhiteExplosion.Load<GameObject>();

            var slow80 = Utils.Paths.Texture2D.texBuffSlow50Icon.Load<Texture2D>();

            useless = ScriptableObject.CreateInstance<BuffDef>();
            useless.isHidden = true;
            useless.name = "Glacial Deletion";

            slow = ScriptableObject.CreateInstance<BuffDef>();
            slow.name = "Glacial Elite Slow";
            slow.buffColor = new Color32(165, 222, 237, 255);
            slow.iconSprite = Sprite.Create(slow80, new Rect(0f, 0f, (float)slow80.width, (float)slow80.height), new Vector2(0f, 0f));
            slow.isDebuff = true;
            slow.canStack = false;
            slow.isHidden = false;

            ContentAddition.AddBuffDef(useless);
            ContentAddition.AddBuffDef(slow);
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            On.RoR2.GlobalEventManager.OnHitAll += GlobalEventManager_OnHitAll;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            IL.RoR2.CharacterModel.UpdateOverlays += CharacterModel_UpdateOverlays;
        }

        private void GlobalEventManager_OnHitAll(On.RoR2.GlobalEventManager.orig_OnHitAll orig, GlobalEventManager self, DamageInfo damageInfo, GameObject hitObject)
        {
            var attacker = damageInfo.attacker;
            if (attacker)
            {
                var attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                if (attackerBody)
                {
                    if (attackerBody.HasBuff(RoR2Content.Buffs.AffixWhite))
                    {
                        var procType = (ProcType)1258907;
                        if (!damageInfo.procChainMask.HasProc(procType))
                        {
                            ProcChainMask mask = new();
                            mask.AddProc(procType);
                            DebuffSphere(slow.buffIndex, attackerBody.teamComponent.teamIndex, damageInfo.position, 4f, 1.5f, iceExplosionPrefab, null, false, true, null);
                        }
                    }
                }
                orig(self, damageInfo, hitObject);
            }
        }

        private void CharacterModel_UpdateOverlays(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                 x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "Slow80")))
            {
                c.Index += 2;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<bool, CharacterModel, bool>>((hasBuff, self) =>
                {
                    return hasBuff || (self.body.HasBuff(slow));
                });
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Glacial Elite Overlay hook");
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(slow))
            {
                args.moveSpeedReductionMultAdd += 0.8f;
            }
        }

        private void GlobalEventManager_OnHitEnemy(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "AffixWhite")))
            {
                c.Remove();
                c.Emit<Glacial>(OpCodes.Ldsfld, nameof(useless));
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Glacial Elite On Hit hook");
            }
        }

        private void DebuffSphere(BuffIndex buff, TeamIndex team, Vector3 position, float radius, float debuffDuration, GameObject effect, GameObject hitEffect, bool ignoreImmunity, bool falloff, NetworkSoundEventDef buffSound)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (effect != null)
            {
                EffectManager.SpawnEffect(effect, new EffectData
                {
                    origin = position,
                    scale = radius
                }, true);
            }
            float radiusHalfwaySqr = radius * radius * 0.25f;
            List<HealthComponent> healthComponentList = new();
            Collider[] colliders = Physics.OverlapSphere(position, radius, LayerIndex.entityPrecise.mask);
            for (int i = 0; i < colliders.Length; i++)
            {
                HurtBox hurtBox = colliders[i].GetComponent<HurtBox>();
                if (hurtBox)
                {
                    var healthComponent = hurtBox.healthComponent;
                    var projectileController = colliders[i].GetComponentInParent<ProjectileController>();
                    if (healthComponent && !projectileController && !healthComponentList.Contains(healthComponent))
                    {
                        healthComponentList.Add(healthComponent);
                        if (healthComponent.body.teamComponent && healthComponent.body.teamComponent.teamIndex != team)
                        {
                            if (ignoreImmunity || (!healthComponent.body.HasBuff(RoR2Content.Buffs.Immune) && !healthComponent.body.HasBuff(RoR2Content.Buffs.HiddenInvincibility)))
                            {
                                float effectiveness = 1f;
                                if (falloff)
                                {
                                    float distSqr = (position - hurtBox.collider.ClosestPoint(position)).sqrMagnitude;
                                    if (distSqr > radiusHalfwaySqr)  //Reduce effectiveness when over half the radius away
                                    {
                                        effectiveness *= 0.5f;  //0.25 is vanilla sweetspot
                                    }
                                }
                                bool alreadyHasBuff = healthComponent.body.HasBuff(buff);
                                healthComponent.body.AddTimedBuff(buff, effectiveness * debuffDuration);
                                if (!alreadyHasBuff)
                                {
                                    if (hitEffect != null)
                                    {
                                        EffectManager.SpawnEffect(hitEffect, new EffectData
                                        {
                                            origin = healthComponent.body.corePosition
                                        }, true);
                                    }
                                    if (buffSound != null)
                                    {
                                        EffectManager.SimpleSoundEffect(buffSound.index, healthComponent.body.corePosition, true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}