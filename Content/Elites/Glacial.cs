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
        public static ModdedDamageType slowDamageType;
        public static GameObject iceExplosionPrefab;
        public override string Name => ":: Elites :::: Glacial";

        public override void Init()
        {
            iceExplosionPrefab = Utils.Paths.GameObject.AffixWhiteExplosion.Load<GameObject>();

            slowDamageType = ReserveDamageType();

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
                        ProcChainMask blastAttackMask = new();
                        blastAttackMask.AddProc(ProcType.LoaderLightning);

                        BlastAttack blastAttack = new()
                        {
                            attacker = attacker,
                            baseDamage = 0f,
                            baseForce = 0f,
                            crit = attackerBody.RollCrit(),
                            damageType = DamageType.Generic,
                            procCoefficient = 0f,
                            radius = 4f,
                            falloffModel = BlastAttack.FalloffModel.None,
                            position = damageInfo.position,
                            attackerFiltering = AttackerFiltering.NeverHitSelf,
                            teamIndex = attackerBody.teamComponent.teamIndex,
                            procChainMask = blastAttackMask
                        };

                        EffectManager.SpawnEffect(iceExplosionPrefab, new EffectData { scale = 4f, origin = damageInfo.position }, true);

                        DamageAPI.AddModdedDamageType(blastAttack, slowDamageType);

                        blastAttack.Fire();

                        var victimBody = hitObject.GetComponent<CharacterBody>();

                        if (DamageAPI.HasModdedDamageType(blastAttack, slowDamageType) && victimBody)
                        {
                            victimBody.AddTimedBuff(slow, 1.5f * damageInfo.procCoefficient);
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
    }
}