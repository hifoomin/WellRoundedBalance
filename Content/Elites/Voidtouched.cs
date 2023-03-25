using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Collections;
using WellRoundedBalance.Buffs;
using WellRoundedBalance.Eclipse;
using WellRoundedBalance.Gamemodes.Eclipse;

namespace WellRoundedBalance.Elites
{
    internal class Voidtouched : EliteBase
    {
        public static BuffDef useless;
        public static BuffDef hiddenCooldown;
        public static GameObject spike; // cache this once so dont have to reload each time (i forgor if addressables already does this but just in case)

        public override string Name => ":: Elites :::::: Voidtouched";

        public override void Init()
        {
            useless = ScriptableObject.CreateInstance<BuffDef>();
            useless.name = "Voidtouched Deletion";
            useless.isHidden = true;

            hiddenCooldown = ScriptableObject.CreateInstance<BuffDef>();
            hiddenCooldown.name = "spike cd";
            hiddenCooldown.isHidden = true;

            ContentAddition.AddBuffDef(useless);
            ContentAddition.AddBuffDef(hiddenCooldown);

            spike = Utils.Paths.GameObject.ImpVoidspikeProjectile.Load<GameObject>();

            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            IL.RoR2.AffixVoidBehavior.FixedUpdate += AffixVoidBehavior_FixedUpdate;
            On.RoR2.CharacterBody.OnSkillActivated += OnSkillActivated;
        }

        private void OnSkillActivated(On.RoR2.CharacterBody.orig_OnSkillActivated orig, CharacterBody body, GenericSkill slot)
        {
            orig(body, slot);
            if (!NetworkServer.active || body.HasBuff(hiddenCooldown) || !body.HasBuff(DLC1Content.Buffs.EliteVoid))
            {
                return;
            }

            Vector3 originalPosition = body.corePosition;
            Vector3 aimDirection = body.inputBank.aimDirection;

            for (int i = 0; i < (Eclipse3.CheckEclipse() ? 8 : 4); i++)
            {
                Vector3 position = originalPosition + (aimDirection * (i * 10));
                position.y = Eclipse3.CheckEclipse() ? 60 : 120;
                if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, 1000, ~0))
                {
                    FireProjectileInfo info = new();
                    info.damage = body.damage * 2f;
                    info.damageTypeOverride = DamageType.Nullify;
                    info.crit = false;
                    info.position = position;
                    info.rotation = Quaternion.LookRotation(Vector3.down);
                    info.projectilePrefab = spike;
                    info.owner = body.gameObject;
                    ProjectileManager.instance.FireProjectile(info);
                }
            }
            body.AddTimedBuff(hiddenCooldown, 2);
        }

        private void AffixVoidBehavior_FixedUpdate(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.DLC1Content/Buffs", "BearVoidReady"),
                x => x.MatchCallOrCallvirt<CharacterBody>("AddBuff")))
            {
                c.Remove();
                c.Emit<Useless>(OpCodes.Ldsfld, nameof(Useless.voidtouchedSaferSpaces));
            }
            else
            {
                Logger.LogError("Failed to apply Voidtouched Elite Safer Spaces hook");
            }
        }

        private void GlobalEventManager_OnHitEnemy(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(DLC1Content.Buffs), "EliteVoid")))
            {
                c.Remove();
                c.Emit<Useless>(OpCodes.Ldsfld, nameof(Useless.uselessBuff));
            }
            else
            {
                Logger.LogError("Failed to apply Voidtouched Elite Needletick hook");
            }
        }
    }
}