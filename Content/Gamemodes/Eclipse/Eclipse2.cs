using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Util = WellRoundedBalance.Gamemodes.Eclipse.PredictionUtils;

namespace WellRoundedBalance.Gamemodes.Eclipse
{
    internal class Eclipse2 : GamemodeBase<Eclipse2>
    {
        public override string Name => ":: Gamemode : Eclipse";

        public float basePredictionAngle = 45f;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HoldoutZoneController.FixedUpdate += HoldoutZoneController_FixedUpdate;
            IL.EntityStates.BeetleGuardMonster.FireSunder.FixedUpdate += FireSunder_FixedUpdate;
            IL.EntityStates.Bell.BellWeapon.ChargeTrioBomb.FixedUpdate += ChargeTrioBomb_FixedUpdate;
            IL.EntityStates.ClayBoss.ClayBossWeapon.FireBombardment.FireGrenade += FireBombardment_FireGrenade;
            On.EntityStates.ClayGrenadier.ThrowBarrel.ModifyProjectileAimRay += ThrowBarrel_ModifyProjectileAimRay;
            IL.EntityStates.GenericProjectileBaseState.FireProjectile += GenericProjectileBaseState_FireProjectile;
            IL.EntityStates.GreaterWispMonster.FireCannons.OnEnter += FireCannons_OnEnter;
            IL.EntityStates.GravekeeperBoss.FireHook.OnEnter += FireHook_OnEnter;
            IL.EntityStates.LemurianMonster.FireFireball.OnEnter += FireFireball_OnEnter;
            IL.EntityStates.LemurianBruiserMonster.FireMegaFireball.FixedUpdate += FireMegaFireball_FixedUpdate;
            IL.EntityStates.GenericProjectileBaseState.FireProjectile += GenericProjectileBaseState_FireProjectile1;
            IL.EntityStates.GenericProjectileBaseState.FireProjectile += GenericProjectileBaseState_FireProjectile2;
            IL.EntityStates.RoboBallBoss.Weapon.FireEyeBlast.FixedUpdate += FireEyeBlast_FixedUpdate;
            IL.EntityStates.ScavMonster.FireEnergyCannon.OnEnter += FireEnergyCannon_OnEnter;
            IL.EntityStates.VagrantMonster.Weapon.JellyBarrage.FixedUpdate += JellyBarrage_FixedUpdate;
            On.EntityStates.VoidJailer.Weapon.Fire.ModifyProjectileAimRay += Fire_ModifyProjectileAimRay;
            IL.EntityStates.Vulture.Weapon.FireWindblade.OnEnter += FireWindblade_OnEnter;
        }

        private void FireWindblade_OnEnter(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.After,
                 x => x.MatchCall<EntityStates.BaseState>("GetAimRay")
                ))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<Ray, EntityStates.Vulture.Weapon.FireWindblade, Ray>>((aimRay, self) =>
                {
                    if (Run.instance && self.characterBody && !self.characterBody.isPlayerControlled && self.characterBody.isElite)
                    {
                        HurtBox targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                        Ray newAimRay = Util.PredictAimrayPS(aimRay, self.GetTeam(), basePredictionAngle, EntityStates.Vulture.Weapon.FireWindblade.projectilePrefab, targetHurtbox);
                        //Feed it the projectile prefab in case a mod is changing the speed.
                        return newAimRay;
                    }
                    return aimRay;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Eclipse 2 Alloy Vulture hook");
            }
        }

        private Ray Fire_ModifyProjectileAimRay(On.EntityStates.VoidJailer.Weapon.Fire.orig_ModifyProjectileAimRay orig, EntityStates.VoidJailer.Weapon.Fire self, Ray aimRay)
        {
            if (Run.instance && self.characterBody && !self.characterBody.isPlayerControlled && self.characterBody.isElite)
            {
                HurtBox targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                Ray newAimRay = Util.PredictAimrayPS(aimRay, self.GetTeam(), basePredictionAngle, self.projectilePrefab, targetHurtbox);
                return orig(self, newAimRay);
            }
            return orig(self, aimRay);
        }

        private void JellyBarrage_FixedUpdate(ILContext il)
        {
            bool error = true;
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.After,
                 x => x.MatchCall<EntityStates.BaseState>("GetAimRay")
                ))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<Ray, EntityStates.VagrantMonster.Weapon.JellyBarrage, Ray>>((aimRay, self) =>
                {
                    if (Run.instance && self.characterBody && !self.characterBody.isPlayerControlled && self.characterBody.isElite)
                    {
                        HurtBox targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                        Ray newAimRay = Util.PredictAimrayPS(aimRay, self.GetTeam(), basePredictionAngle, EntityStates.VagrantMonster.Weapon.JellyBarrage.projectilePrefab, targetHurtbox);
                        return newAimRay;
                    }
                    return aimRay;
                });
                if (c.TryGotoNext(MoveType.After,
                     x => x.MatchCall<EntityStates.BaseState>("GetAimRay")
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<Ray, EntityStates.VagrantMonster.Weapon.JellyBarrage, Ray>>((aimRay, self) =>
                    {
                        if (Run.instance && self.characterBody && !self.characterBody.isPlayerControlled && self.characterBody.isElite)
                        {
                            HurtBox targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                            Ray newAimRay = Util.PredictAimrayPS(aimRay, self.GetTeam(), basePredictionAngle, EntityStates.VagrantMonster.Weapon.JellyBarrage.projectilePrefab, targetHurtbox);
                            return newAimRay;
                        }
                        return aimRay;
                    });
                    error = false;
                }
            }

            if (error)
            {
                Logger.LogError("Failed to apply Eclipse 2 Wandering Vagrant hook");
            }
        }

        private void FireEnergyCannon_OnEnter(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.After,
                 x => x.MatchCall<EntityStates.BaseState>("GetAimRay")
                ))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<Ray, EntityStates.ScavMonster.FireEnergyCannon, Ray>>((aimRay, self) =>
                {
                    if (Run.instance && self.characterBody && !self.characterBody.isPlayerControlled && self.characterBody.isElite)
                    {
                        HurtBox targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                        Ray newAimRay = Util.PredictAimrayPS(aimRay, self.GetTeam(), basePredictionAngle, EntityStates.ScavMonster.FireEnergyCannon.projectilePrefab, targetHurtbox);
                        return newAimRay;
                    }
                    return aimRay;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Eclipse 2 Scavenger hook");
            }
        }

        private void FireEyeBlast_FixedUpdate(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.After,
                 x => x.MatchCall<EntityStates.BaseState>("GetAimRay")
                ))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<Ray, EntityStates.RoboBallBoss.Weapon.FireEyeBlast, Ray>>((aimRay, self) =>
                {
                    if (Run.instance && self.characterBody && !self.characterBody.isPlayerControlled && self.characterBody.isElite)
                    {
                        Ray newAimRay;
                        HurtBox targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                        float projectileSpeed = self.projectileSpeed;
                        if (projectileSpeed > 0f)
                        {
                            if (self.GetTeam() != TeamIndex.Player) projectileSpeed = Main.GetProjectileSimpleModifiers(projectileSpeed);
                            newAimRay = Util.PredictAimray(aimRay, self.GetTeam(), basePredictionAngle, projectileSpeed, targetHurtbox);
                        }
                        else
                        {
                            newAimRay = Util.PredictAimrayPS(aimRay, self.GetTeam(), basePredictionAngle, self.projectilePrefab, targetHurtbox);
                        }
                        return newAimRay;
                    }
                    return aimRay;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Eclipse 2 Solus Control Unit hook");
            }
        }

        private void GenericProjectileBaseState_FireProjectile2(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.After,
                 x => x.MatchCall<EntityStates.BaseState>("GetAimRay")
                ))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<Ray, EntityStates.GenericProjectileBaseState, Ray>>((aimRay, self) =>
                {
                    if (Run.instance && self.GetType() == typeof(EntityStates.MinorConstruct.Weapon.FireConstructBeam))
                    {
                        if (self.characterBody && !self.characterBody.isPlayerControlled && self.characterBody.isElite)
                        {
                            HurtBox targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                            Ray newAimRay = Util.PredictAimrayPS(aimRay, self.GetTeam(), basePredictionAngle, self.projectilePrefab, targetHurtbox);
                            return newAimRay;
                        }
                    }
                    return aimRay;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Eclipse 2 Alpha Construct hook");
            }
        }

        private void GenericProjectileBaseState_FireProjectile1(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.After,
                 x => x.MatchCall<EntityStates.BaseState>("GetAimRay")
                ))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<Ray, EntityStates.GenericProjectileBaseState, Ray>>((aimRay, self) =>
                {
                    if (Run.instance && self.GetType() == typeof(EntityStates.LunarExploderMonster.Weapon.FireExploderShards))
                    {
                        if (self.characterBody && !self.characterBody.isPlayerControlled && self.characterBody.isElite)
                        {
                            HurtBox targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                            Ray newAimRay = Util.PredictAimrayPS(aimRay, self.GetTeam(), basePredictionAngle, self.projectilePrefab, targetHurtbox);
                            return newAimRay;
                        }
                    }
                    return aimRay;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Eclipse 2 Lunar Exploder hook");
            }
        }

        private void FireMegaFireball_FixedUpdate(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.After,
                 x => x.MatchCall<EntityStates.BaseState>("GetAimRay")
                ))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<Ray, EntityStates.LemurianBruiserMonster.FireMegaFireball, Ray>>((aimRay, self) =>
                {
                    if (Run.instance && self.characterBody && !self.characterBody.isPlayerControlled && self.characterBody.isElite)
                    {
                        HurtBox targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                        Ray newAimRay;

                        float projectileSpeed = EntityStates.LemurianBruiserMonster.FireMegaFireball.projectileSpeed;
                        if (projectileSpeed > 0f)
                        {
                            if (self.GetTeam() != TeamIndex.Player) projectileSpeed = Main.GetProjectileSimpleModifiers(projectileSpeed);
                            newAimRay = Util.PredictAimray(aimRay, self.GetTeam(), basePredictionAngle, projectileSpeed, targetHurtbox);
                        }
                        else
                        {
                            newAimRay = Util.PredictAimrayPS(aimRay, self.GetTeam(), basePredictionAngle, EntityStates.LemurianBruiserMonster.FireMegaFireball.projectilePrefab, targetHurtbox);
                        }

                        return newAimRay;
                    }
                    return aimRay;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Eclipse 2 Elder Lemurian hook");
            }
        }

        private void FireFireball_OnEnter(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.After,
                 x => x.MatchCall<EntityStates.BaseState>("GetAimRay")
                ))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<Ray, EntityStates.LemurianMonster.FireFireball, Ray>>((aimRay, self) =>
                {
                    if (Run.instance && self.characterBody && !self.characterBody.isPlayerControlled && self.characterBody.isElite)
                    {
                        HurtBox targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                        Ray newAimRay = Util.PredictAimrayPS(aimRay, self.GetTeam(), basePredictionAngle, EntityStates.LemurianMonster.FireFireball.projectilePrefab, targetHurtbox);
                        return newAimRay;
                    }
                    return aimRay;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Eclipse 2 Lemurian hook");
            }
        }

        private void FireHook_OnEnter(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.After,
                 x => x.MatchCall<EntityStates.BaseState>("GetAimRay")
                ))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<Ray, EntityStates.GravekeeperBoss.FireHook, Ray>>((aimRay, self) =>
                {
                    if (Run.instance && self.characterBody && !self.characterBody.isPlayerControlled && self.characterBody.isElite)
                    {
                        HurtBox targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                        Ray newAimRay = Util.PredictAimrayPS(aimRay, self.GetTeam(), basePredictionAngle, EntityStates.GravekeeperBoss.FireHook.projectilePrefab, targetHurtbox);
                        return newAimRay;
                    }
                    return aimRay;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Eclipse 2 Grovetender hook");
            }
        }

        private void FireCannons_OnEnter(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.After,
                 x => x.MatchCall<EntityStates.BaseState>("GetAimRay")
                ))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<Ray, EntityStates.GreaterWispMonster.FireCannons, Ray>>((aimRay, self) =>
                {
                    if (Run.instance && self.characterBody && !self.characterBody.isPlayerControlled && self.characterBody.isElite)
                    {
                        HurtBox targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                        Ray newAimRay = Util.PredictAimrayPS(aimRay, self.GetTeam(), basePredictionAngle, self.projectilePrefab, targetHurtbox);
                        return newAimRay;
                    }
                    return aimRay;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Eclipse 2 Greater Wisp hook");
            }
        }

        private void GenericProjectileBaseState_FireProjectile(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.After,
                 x => x.MatchCall<EntityStates.BaseState>("GetAimRay")
                ))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<Ray, EntityStates.GenericProjectileBaseState, Ray>>((aimRay, self) =>
                {
                    if (self.GetType() == typeof(EntityStates.FlyingVermin.Weapon.Spit))
                    {
                        if (Run.instance && self.characterBody && !self.characterBody.isPlayerControlled && self.characterBody.isElite)
                        {
                            HurtBox targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                            Ray newAimRay = Util.PredictAimrayPS(aimRay, self.GetTeam(), basePredictionAngle, self.projectilePrefab, targetHurtbox);
                            return newAimRay;
                        }
                    }
                    return aimRay;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Eclipse 2 Blind Pest hook");
            }
        }

        private Ray ThrowBarrel_ModifyProjectileAimRay(On.EntityStates.ClayGrenadier.ThrowBarrel.orig_ModifyProjectileAimRay orig, EntityStates.ClayGrenadier.ThrowBarrel self, Ray aimRay)
        {
            if (Run.instance && self.characterBody && self.characterBody.isElite)
            {
                HurtBox targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                Ray newAimRay = Util.PredictAimrayPS(aimRay, self.GetTeam(), basePredictionAngle, self.projectilePrefab, targetHurtbox);
                return orig(self, newAimRay);
            }
            return orig(self, aimRay);
        }

        private void FireBombardment_FireGrenade(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.After,
                 x => x.MatchCall<EntityStates.BaseState>("GetAimRay")
                ))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<Ray, EntityStates.LemurianMonster.FireFireball, Ray>>((aimRay, self) =>
                {
                    if (Run.instance && self.characterBody && !self.characterBody.isPlayerControlled && self.characterBody.isElite)
                    {
                        HurtBox targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                        Ray newAimRay = Util.PredictAimrayPS(aimRay, self.GetTeam(), basePredictionAngle, EntityStates.ClayBoss.ClayBossWeapon.FireBombardment.projectilePrefab, targetHurtbox);
                        return newAimRay;
                    }
                    return aimRay;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Eclipse 2 Clay Dunestrider hook");
            }
        }

        private void ChargeTrioBomb_FixedUpdate(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.After,
                 x => x.MatchCall<EntityStates.BaseState>("GetAimRay")
                ))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<Ray, EntityStates.Bell.BellWeapon.ChargeTrioBomb, Ray>>((aimRay, self) =>
                {
                    if (Run.instance && self.characterBody && !self.characterBody.isPlayerControlled && self.characterBody.isElite)
                    {
                        //Uncomment this to improve accuracy further.
                        /*Transform t = self.FindTargetChildTransformFromBombIndex();
                        if (t)
                        {
                            aimRay.origin = t.position;
                        }*/
                        HurtBox targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                        Ray newAimRay = Util.PredictAimrayPS(aimRay, self.GetTeam(), basePredictionAngle, EntityStates.Bell.BellWeapon.ChargeTrioBomb.bombProjectilePrefab, targetHurtbox);
                        return newAimRay;
                    }
                    return aimRay;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Eclipse 2 Brass Contraption hook");
            }
        }

        private void FireSunder_FixedUpdate(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.After,
                     x => x.MatchCall<EntityStates.BaseState>("GetAimRay")))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<Ray, EntityStates.BeetleGuardMonster.FireSunder, Ray>>((aimRay, self) =>
                {
                    if (Run.instance && self.characterBody && !self.characterBody.isPlayerControlled && self.characterBody.isElite)
                    {
                        aimRay.origin = self.handRTransform.position;//Called in Vanilla method, but  call here beforehand before calculating the new aimray.
                        var targetHurtbox = Util.GetMasterAITargetHurtbox(self.characterBody.master);
                        var newAimRay = Util.PredictAimrayPCC(aimRay, self.GetTeam(), basePredictionAngle, EntityStates.BeetleGuardMonster.FireSunder.projectilePrefab, targetHurtbox);
                        return newAimRay;
                    }
                    return aimRay;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Eclipse 2 Beetle Guard hook");
            }
        }

        private void HoldoutZoneController_FixedUpdate(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.5f)))
            {
                c.Next.Operand = 1f;
            }
            else
            {
                Logger.LogError("Failed to apply Eclipse 2 hook");
            }
        }
    }
}