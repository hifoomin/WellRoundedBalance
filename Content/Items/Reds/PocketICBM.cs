using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Reds
{
    public class PocketICBM : ItemBase
    {
        public static GameObject bigFuckingMissile;
        public static GameObject bigFuckingMissileGhost;
        public override string Name => ":: Items ::: Reds :: Pocket ICBM";
        public override ItemDef InternalPickup => DLC1Content.Items.MoreMissile;

        public override string PickupText => "All Missile items fire an additional missile. Gain a " + baseMissileChance + "% chance to fire a missile.";

        public override string DescText => "All missile items and equipment fire an additional <style=cIsDamage>missile</style>. Gain a <style=cIsDamage>" + baseMissileChance + "%</style> <style=cStack>(+" + missileChancePerStack + "% per stack)</style> chance to fire a missile that deals <style=cIsDamage>" + d(totalDamage) + "</style> TOTAL damage.";

        [ConfigField("TOTAL Damage", "Decimal.", 1.5f)]
        public static float totalDamage;

        [ConfigField("Base Missile Chance", 10f)]
        public static float baseMissileChance;

        [ConfigField("Missile Chance Per Stack", 10f)]
        public static float missileChancePerStack;

        public override void Init()
        {
            bigFuckingMissile = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.MissileProjectile.Load<GameObject>(), "BigFuckingMissile");

            var missileController = bigFuckingMissile.GetComponent<MissileController>();
            missileController.maxSeekDistance = 10000f;
            missileController.turbulence = 0f;
            missileController.deathTimer = 30f;
            missileController.giveupTimer = 30f;
            missileController.delayTimer = 0f;
            missileController.maxVelocity = 25f * 2.5f;
            missileController.acceleration = 3f * 2.5f;

            var projectileController = bigFuckingMissile.GetComponent<ProjectileController>();

            bigFuckingMissileGhost = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.MissileGhost.Load<GameObject>(), "BigFuckingMissileGhost", false);
            bigFuckingMissileGhost.transform.localScale = new Vector3(9f, 9f, 9f);
            var flare = bigFuckingMissileGhost.transform.GetChild(1);
            flare.gameObject.SetActive(false);

            var icbmTrail = GameObject.Instantiate(Utils.Paths.Material.matMissileTrail.Load<Material>());
            icbmTrail.name = "DSADASD";
            icbmTrail.SetColor("_TintColor", new Color32(255, 129, 102, 255));

            var trail = bigFuckingMissileGhost.transform.GetChild(0);
            var trailRenderer = trail.GetComponent<TrailRenderer>();
            trailRenderer.time = 0.4f * 1.5f;
            trailRenderer.widthMultiplier = 0.5f * 3f;

            // Main.WRBLogger.LogError("pre sharedMaterial is " + trailRenderer.sharedMaterial);

            trailRenderer.sharedMaterial = icbmTrail;

            // Main.WRBLogger.LogError("post sharedMaterial is " + trailRenderer.sharedMaterial);

            var missileModel = bigFuckingMissileGhost.transform.GetChild(2);
            missileModel.transform.eulerAngles = new Vector3(-90f, 0f, 0f);
            var meshRenderer = missileModel.GetComponent<MeshRenderer>();

            var icbmMat = GameObject.Instantiate(Utils.Paths.Material.matMissile.Load<Material>());
            // icbmMat.SetColor("_Color", new Color32(224, 94, 94, 255));
            icbmMat.SetTexture("_MainTex", Main.wellroundedbalance.LoadAsset<Texture2D>("texIcbm.png"));
            icbmMat.EnableKeyword("DITHER");

            meshRenderer.sharedMaterial = icbmMat;

            projectileController.ghostPrefab = bigFuckingMissileGhost;

            PrefabAPI.RegisterNetworkPrefab(bigFuckingMissile);

            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.MissileUtils.FireMissile_Vector3_CharacterBody_ProcChainMask_GameObject_float_bool_GameObject_DamageColorIndex_Vector3_float_bool += Changes;
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeMissileCount;
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        }

        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            if (damageInfo.attacker)
            {
                var body = damageInfo.attacker.GetComponent<CharacterBody>();
                if (body)
                {
                    var inventory = body.inventory;
                    if (inventory)
                    {
                        if (!damageInfo.procChainMask.HasProc(ProcType.Missile))
                        {
                            var stack = inventory.GetItemCount(DLC1Content.Items.MoreMissile);
                            if (stack > 0)
                            {
                                if (Util.CheckRoll((baseMissileChance + missileChancePerStack * (stack - 1)) * damageInfo.procCoefficient, body.master))
                                {
                                    float damage = Util.OnHitProcDamage(damageInfo.damage, body.damage, totalDamage);
                                    MissileUtils.FireMissile(body.corePosition, body, damageInfo.procChainMask, victim, damage, damageInfo.crit, bigFuckingMissile, DamageColorIndex.Item, true);
                                }
                            }
                        }
                    }
                }
            }

            orig(self, damageInfo, victim);
        }

        private void ChangeMissileCount(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
               x => x.MatchLdcI4(1),
               x => x.MatchBr(out _),
               x => x.MatchLdcI4(3)))
            {
                c.Index += 2;
                c.Next.Operand = 2;
            }
            else
            {
                Logger.LogError("Failed to apply Pocket I.C.B.M. Missile Count 1 hook");
            }
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.After, x => x.MatchCallOrCallvirt(typeof(int?).GetMethod("GetValueOrDefault", new Type[] { }))))
            {
                c.Index += 4;
                c.EmitDelegate<Func<float, float>>((orig) => 0f);
                for (int i = 0; c.TryGotoNext(x => x.MatchCallOrCallvirt(typeof(Quaternion).GetMethod("AngleAxis", (System.Reflection.BindingFlags)(-1)))); i++)
                {
                    c.Index--;
                    c.EmitDelegate<Func<float, float>>((orig) => (i % 2 == 0) ? Run.instance.treasureRng.RangeFloat(-55f, 55f) : Run.instance.treasureRng.RangeFloat(15f, 45f) * (-1));
                    c.Index += 2;
                }
                ILLabel label = c.DefineLabel();
                if (c.TryGotoPrev(MoveType.After, x => x.MatchBle(out label)))
                {
                    c.EmitDelegate(() => 1 < 2);
                    c.Emit(OpCodes.Brtrue, label);
                    c.GotoLabel(label, MoveType.Before);
                    c.MoveAfterLabels();
                    c.Emit(OpCodes.Ldloc_0);
                    c.Emit(OpCodes.Ldloc, 4);
                    c.Emit(OpCodes.Ldarg_1);
                    c.Emit(OpCodes.Ldarg, 8);
                    c.EmitDelegate<Action<int, FireProjectileInfo, CharacterBody, Vector3>>((stacks, info, body, initDir) =>
                    {
                        if (stacks > 0)
                        {
                            InputBankTest bank = body.GetComponent<InputBankTest>();
                            for (int i = 0; i < (stacks - 1) * 0 + ((1 == 1) ? 1 : (1 > 2) ? 1 - 2 : 0); i++)
                            {
                                info.rotation = Util.QuaternionSafeLookRotation(Quaternion.AngleAxis(Run.instance.treasureRng.RangeFloat(-55f, 55f) * ((i % 2 == 0) ? 1 : (-1)), bank ? bank.aimDirection : body.transform.position) * initDir);
                                ProjectileManager.instance.FireProjectile(info);
                            }
                        }
                    });
                }
            }
            else
            {
                Logger.LogError("Failed to apply Pocket I.C.B.M. Damage, Count, Stacking hook");
            }

            // BIG thanks to RandomlyAwesome
        }
    }
}