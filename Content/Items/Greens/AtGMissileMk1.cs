﻿using System;

namespace WellRoundedBalance.Items.Greens
{
    public class AtGMissileMk1 : ItemBase<AtGMissileMk1>
    {
        public override string Name => ":: Items :: Greens :: AtG Missile Mk1";
        public override ItemDef InternalPickup => RoR2Content.Items.Missile;

        public override string PickupText => "Chance to fire a missile.";
        public override string DescText => "<style=cIsDamage>10%</style> chance to fire a missile that deals <style=cIsDamage>" + d(baseTotalDamage) + "</style> <style=cStack>(+" + d(totalDamagePerStack) + " per stack)</style> TOTAL damage.";

        [ConfigField("Base TOTAL Damage", "Decimal.", 3f)]
        public static float baseTotalDamage;

        [ConfigField("TOTAL Damage Per Stack", "Decimal.", 3f)]
        public static float totalDamagePerStack;

        [ConfigField("Improve targeting?", "Affects all missile items and equipment.", true)]
        public static bool improveTargeting;

        [ConfigField("Proc Coefficient", 0.75f)]
        public static float procCoefficient;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.ProcessHitEnemy += GlobalEventManager_ProcessHitEnemy;
            Changes();
        }

        private void GlobalEventManager_ProcessHitEnemy(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchBrfalse(out _),
                x => x.MatchLdcR4(3f),
                x => x.MatchLdloc(out _),
                x => x.MatchConvR4(),
                x => x.MatchMul()))
            {
                c.Index += 1;
                c.Next.Operand = totalDamagePerStack;
                c.Index += 4;
                c.EmitDelegate<Func<float, float>>((self) =>
                {
                    return self + (baseTotalDamage - totalDamagePerStack);
                });
            }
            else
            {
                Logger.LogError("Failed to apply AtG Damage hook");
            }
        }

        public static void Changes()
        {
            var missileProjectile = Utils.Paths.GameObject.MissileProjectile.Load<GameObject>();
            missileProjectile.name = "Generic Missile";
            var missileProjectileController = missileProjectile.GetComponent<ProjectileController>();
            missileProjectileController.procCoefficient = procCoefficient * Items.Greens._ProcCoefficients.globalProc;
            var ghost = missileProjectileController.ghostPrefab;
            ghost.transform.localScale = new Vector3(2f, 2f, 2f);
            ghost.transform.GetChild(1).gameObject.SetActive(false);

            var missileModel = ghost.transform.GetChild(2);
            missileModel.transform.eulerAngles = new Vector3(-90f, 0f, 0f);
            var meshRenderer = missileModel.GetComponent<MeshRenderer>();

            var atgMat = GameObject.Instantiate(Utils.Paths.Material.matMissile.Load<Material>());
            // atgMat.SetColor("_Color", new Color32(224, 94, 94, 255));
            atgMat.SetTexture("_MainTex", Main.wellroundedbalance.LoadAsset<Texture2D>("texAtg.png"));
            atgMat.EnableKeyword("DITHER");
            atgMat.EnableKeyword("FADECLOSE");
            meshRenderer.sharedMaterial = atgMat;

            if (improveTargeting)
            {
                var missileController = missileProjectile.GetComponent<MissileController>();
                missileController.maxSeekDistance = 10000f;
                missileController.turbulence = 0f;
                missileController.deathTimer = 30f;
                missileController.giveupTimer = 30f;
                missileController.delayTimer = 0f;
            }
        }
    }
}