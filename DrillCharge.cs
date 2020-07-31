using EntityStates;
using RoR2;
using UnityEngine;
using System.Collections.Generic;


namespace Gnome.EntityStatez
{

    public class DrillChargeState : BaseSkillState
    {
        public float baseDuration = 0.4f;
        private float duration;
        
        public GameObject explodePrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFX");
        EffectData effectData;
        
        int frameCounter = 0;
        public int charged;

        BlastAttack blastAttack;

        Animator anim;
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration;

            Ray aimRay = base.GetAimRay();

            anim = characterDirection.modelAnimator;

            blastAttack = new BlastAttack
            {
                radius = 10f,
                procCoefficient = 1,
                position = aimRay.origin,
                attacker = base.gameObject,
                crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master),
                baseDamage = base.characterBody.damage * 1.4f,
                falloffModel = BlastAttack.FalloffModel.SweetSpot,
                baseForce = 3f,
                damageType = DamageType.Generic,
                attackerFiltering = AttackerFiltering.NeverHit,
            };
            blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);

            effectData = new EffectData();
            effectData.scale = 6;
            effectData.color = new Color32(234, 234, 127, 100);

            if (base.isAuthority)
            {
                base.characterBody.AddBuff(BuffIndex.HiddenInvincibility);

                Util.PlaySound("DrillCharge", base.gameObject);

                base.characterMotor.velocity += 75 * aimRay.direction;

                anim.SetBool("isDrillCharge", true);
                float angle = Vector3.Angle(new Vector3(0, -1, 0), aimRay.direction);
                if (angle > 120) { anim.SetInteger("aimAngle", 1); }
                anim.SetFloat("drillSpeedMultiplier", Mathf.Max(1, charged / 10));
            }
        }
        public override void OnExit()
        {
            base.characterBody.RemoveBuff(BuffIndex.HiddenInvincibility);
            base.characterBody.AddTimedBuff(BuffIndex.HiddenInvincibility, StaticValues.lingeringInvincibilityDuration);

            base.characterMotor.velocity.x *= 0.5f;
            base.characterMotor.velocity.y *= 0.5f;
            base.characterMotor.velocity.z *= 0.5f;

            anim.SetBool("isDrillCharge", false);
            anim.SetInteger("aimAngle", 0);

            base.OnExit();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            Ray aimRay = base.GetAimRay();
            blastAttack.position = aimRay.origin;
            effectData.origin = aimRay.origin;

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
            float boost = charged * (.8f + (.2f * base.attackSpeedStat));
            int roundDown = Mathf.FloorToInt(boost);
            int scale = Mathf.Max(2, 40 - roundDown);
            if (frameCounter % scale == 0)
            {
                blastAttack.Fire();
                //EffectManager.SpawnEffect(explodePrefab, effectData, false);
            }
            frameCounter++;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}