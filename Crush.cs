using EntityStates;
using RoR2;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Gnome.EntityStatez
{
    public class CrushState : BaseSkillState
    {
        public float baseDuration = 0.4f;
        private float duration;
        public GameObject explodePrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFX");
        public GameObject slashPrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniImpactVFXSlash");
        public GameObject swingPrefab = Resources.Load<GameObject>("prefabs/effects/lemurianbitetrail");

        Animator anim;

        public static event Action<Run> CompactedAchieved;
        public override void OnEnter()
        {
            base.OnEnter();
            Ray aimRay = base.GetAimRay();
            this.duration = this.baseDuration / base.attackSpeedStat;
            anim = characterDirection.modelAnimator;
            float attackScale = base.characterBody.attackSpeed / base.characterBody.baseAttackSpeed;
            attackScale = (attackScale - 1f) * 0.5f + 1f;
            anim.SetFloat("attackSpeedMultiplier", attackScale);
            if (base.isAuthority)
            {
                int hitCount = 0;

                float theta = Vector3.Angle(new Vector3(0, -1, 0), aimRay.direction);
                theta = Mathf.Min(theta, 90);
                Vector3 theSpot = aimRay.origin + ((1 + (theta / 30)) * aimRay.direction);
                base.StartAimMode(0.4f, false);
                anim.SetTrigger("CrushAttack");

                Vector2 move = new Vector2(characterMotor.moveDirection.x, characterMotor.moveDirection.z);
                Vector2 aim = new Vector2(aimRay.direction.x, aimRay.direction.z);
                float forward = Vector2.Dot(move, aim.normalized);
                Vector2 aimO = new Vector2(aimRay.direction.z, -1 * aimRay.direction.x);
                float right = Vector2.Dot(move, aimO.normalized);
                anim.SetFloat("forward", forward);
                anim.SetFloat("right", right);

                float speedScale = 0.7f * (Mathf.Sqrt(3.5f * base.attackSpeedStat));

                BlastAttack blastAttack = new BlastAttack();
                blastAttack.radius = 5f * Mathf.Sqrt(speedScale - .31f);
                blastAttack.procCoefficient = 1f;
                blastAttack.position = theSpot;
                blastAttack.attacker = base.gameObject;
                blastAttack.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
                blastAttack.baseDamage = base.characterBody.damage * 1.8f;
                blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
                blastAttack.baseForce = 3f;
                blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
                blastAttack.damageType = DamageType.Generic;
                blastAttack.attackerFiltering = AttackerFiltering.NeverHit;
                BlastAttack.Result result = blastAttack.Fire();
                hitCount = result.hitCount;

                EffectData effectData = new EffectData();
                effectData.origin = theSpot;
                effectData.scale = speedScale;
                //EffectManager.SpawnEffect(slashPrefab, effectData, false);
                //EffectManager.SpawnEffect(explodePrefab, effectData, false);

                effectData.scale = 0.1f;
                Vector3 left = Vector3.Cross(aimRay.direction, Vector3.up).normalized;
                effectData.origin = aimRay.origin - (0.5f * left);
                //EffectManager.SpawnEffect(swingPrefab, effectData, false);
                effectData.origin = aimRay.origin + left;
                //EffectManager.SpawnEffect(swingPrefab, effectData, false);

                Util.PlaySound("Crush", base.gameObject);

                if (hitCount >= 7 && CompactedAchieved != null)
                {
                    Action<Run> action = CompactedAchieved;
                    action(Run.instance);
                }
            }
        }
        public override void OnExit()
        {
            base.OnExit();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}