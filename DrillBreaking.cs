using EntityStates;
using RoR2;
using UnityEngine;


namespace Gnome.EntityStatez
{
    public class DrillBreakingState : BaseSkillState
    {
        public GameObject explodePrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFX");
        GameObject chargePrefab = EntityStates.LemurianBruiserMonster.ChargeMegaFireball.chargeEffectPrefab;
        GameObject chargeInstance;

        float chargeDuration = 0.3f;

        Animator anim;

        Vector3 left = new Vector3(0, 0, 1);
        protected bool ShouldKeepChargingAuthority()
        {
            float scale = base.attackSpeedStat * Mathf.Sqrt(base.moveSpeedStat / 10f);
            return (base.fixedAge < chargeDuration / scale);
        }

        protected EntityState GetNextStateAuthority()
        {
            return new DrillBreakState();
        }

        public override void OnEnter()
        {
            base.OnEnter();

            Ray aimRay = base.GetAimRay();
            if (base.isAuthority)
            {
                Vector3 direct = aimRay.direction;

                base.StartAimMode(0.3f, false);

                anim = characterDirection.modelAnimator;
                Vector2 move = new Vector2(characterMotor.moveDirection.x, characterMotor.moveDirection.z);
                Vector2 aim = new Vector2(aimRay.direction.x, aimRay.direction.z);
                float forward = Vector2.Dot(move, aim.normalized);
                Vector2 aimO = new Vector2(aimRay.direction.z, -1 * aimRay.direction.x);
                float right = Vector2.Dot(move, aimO.normalized);
                anim.SetFloat("forward", forward);
                anim.SetFloat("right", right);

                Quaternion major = Quaternion.FromToRotation(left, direct);
                chargeInstance = Object.Instantiate<GameObject>(chargePrefab, aimRay.origin, transform.rotation * major);
                chargeInstance.transform.localScale *= 0.0125f;

                //Util.PlaySound("DrillCharging", base.gameObject);
            }
        }
        public override void OnExit()
        {
            if (chargeInstance)
            {
                EntityState.Destroy(chargeInstance);
            }

            anim.SetFloat("forward", 1f);
            anim.SetFloat("right", 0f);

            Ray aimRay = base.GetAimRay();
            EffectData effectData = new EffectData();
            effectData.scale = 6;
            effectData.origin = aimRay.origin;
            EffectManager.SpawnEffect(explodePrefab, effectData, false);

            base.OnExit();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!ShouldKeepChargingAuthority())
            {
                outer.SetNextState(GetNextStateAuthority());
                return;
            }
            Ray aimRay = base.GetAimRay();
            Vector3 direct = aimRay.direction;
            Quaternion major = Quaternion.FromToRotation(left, direct);
            chargeInstance.transform.rotation = transform.rotation * major;
            chargeInstance.transform.position = aimRay.origin;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
