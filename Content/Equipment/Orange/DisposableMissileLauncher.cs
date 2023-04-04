using MonoMod.Cil;
using System;
using WellRoundedBalance.Items;

namespace WellRoundedBalance.Equipment.Orange
{
    public class DisposableMissileLauncher : EquipmentBase<DisposableMissileLauncher>
    {
        public static GameObject dmlMissilePrefab;
        public override string Name => ":: Equipment :: Disposable Missile Launcher";

        public override EquipmentDef InternalPickup => RoR2Content.Equipment.CommandMissile;

        public override string PickupText => "Fire a swarm of missiles.";

        public override string DescText => "Fire a swarm of <style=cIsDamage>" + missileCount + "</style> missiles that deal <style=cIsDamage>" + missileCount + "x" + d(missileDamage) + " damage</style>.";

        [ConfigField("Cooldown", "", 30f)]
        public static float cooldown;

        [ConfigField("Missile Count", "", 12)]
        public static int missileCount;

        [ConfigField("Missile Damage", "", 3f)]
        public static float missileDamage;

        [ConfigField("Proc Coefficient", "", 0f)]
        public static float procCoefficient;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var DML = Utils.Paths.EquipmentDef.CommandMissile.Load<EquipmentDef>();
            DML.cooldown = cooldown;

            Changes();

            IL.RoR2.EquipmentSlot.FireCommandMissile += EquipmentSlot_FireCommandMissile;
            On.RoR2.EquipmentSlot.FireMissile += EquipmentSlot_FireMissile;
        }

        private void EquipmentSlot_FireMissile(On.RoR2.EquipmentSlot.orig_FireMissile orig, EquipmentSlot self)
        {
            // replace with IL later that only replaces the legacyresourcesapi code and changes num

            var missilePrefab = dmlMissilePrefab;
            var damage = missileDamage;
            bool flag = Util.CheckRoll(self.characterBody.crit, self.characterBody.master);
            MissileUtils.FireMissile(self.characterBody.corePosition, self.characterBody, default, null, self.characterBody.damage * damage, flag, missilePrefab, DamageColorIndex.Item, false);
        }

        private void EquipmentSlot_FireCommandMissile(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcI4(12)))
            {
                c.Index += 1;
                c.EmitDelegate<Func<int, int>>((useless) =>
                {
                    return missileCount;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Disposable Missile Launcher Missile Count hook");
            }
        }

        private void Changes()
        {
            dmlMissilePrefab = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.MissileProjectile.Load<GameObject>(), "Disposable Missile");
            var missileProjectileController = dmlMissilePrefab.GetComponent<ProjectileController>();
            missileProjectileController.procCoefficient = procCoefficient * ItemBase.globalProc;
            var ghost = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.MissileGhost.Load<GameObject>(), "Disposable Missile Ghost", false);
            ghost.transform.localScale = new Vector3(2f, 2f, 2f);
            ghost.transform.GetChild(1).gameObject.SetActive(false);

            var missileModel = ghost.transform.GetChild(2);
            missileModel.transform.eulerAngles = new Vector3(-90f, 0f, 0f);
            var meshRenderer = missileModel.GetComponent<MeshRenderer>();

            var dmlMat = GameObject.Instantiate(Utils.Paths.Material.matMissile.Load<Material>());

            dmlMat.SetTexture("_MainTex", Main.wellroundedbalance.LoadAsset<Texture2D>("texDml.png"));
            dmlMat.EnableKeyword("DITHER");
            dmlMat.EnableKeyword("FADECLOSE");
            meshRenderer.sharedMaterial = dmlMat;

            missileProjectileController.ghostPrefab = ghost;

            PrefabAPI.RegisterNetworkPrefab(dmlMissilePrefab);
        }
    }
}