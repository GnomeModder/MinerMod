using EntityStates;
using RoR2;
using UnityEngine;
using KinematicCharacterController;

namespace Gnome.EntityStatez
{
    public class ToTheStarsState : BaseSkillState
    {
        public float baseDuration = 0.4f;
        private float duration;
        public GameObject hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/MissileExplosionVFX");
        public GameObject tracerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/tracers/tracerembers");
        public GameObject smokeEffectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashLoader");
        public GameObject flashEffectPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/muzzleflashfire");

        Quaternion major = Quaternion.FromToRotation(Vector3.forward, Vector3.down);

        Animator anim;
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration;
            anim = characterDirection.modelAnimator;
            if (base.isAuthority)
            {
                base.characterMotor.rootMotion.y += 0.5f;
                base.characterMotor.velocity.y = 30;

                base.gameObject.layer = LayerIndex.fakeActor.intVal;
                base.characterMotor.Motor.RebuildCollidableLayers();

                anim.SetBool("isToTheStars", true);
            }
        }

        public override void OnExit()
        {
            Ray aimRay = base.GetAimRay();

            Vector3 aimer = Vector3.down;

            BulletAttack bulletAttack = new BulletAttack
            {
                owner = base.gameObject,
                weapon = base.gameObject,
                origin = aimRay.origin,
                aimVector = aimer,
                minSpread = 0f,
                maxSpread = base.characterBody.spreadBloomAngle,
                bulletCount = 1U,
                procCoefficient = .5f,
                damage = base.characterBody.damage,
                force = 3,
                falloffModel = BulletAttack.FalloffModel.None,
                tracerEffectPrefab = this.tracerEffectPrefab,
                hitEffectPrefab = this.hitEffectPrefab,
                isCrit = base.RollCrit(),
                HitEffectNormal = false,
                stopperMask = LayerIndex.world.mask,
                smartCollision = true,
                maxDistance = 300f
            };

            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i <= 12; i++)
                {
                    float theta = Random.Range(0.0f, 6.28f);
                    float x = Mathf.Cos(theta);
                    float z = Mathf.Sin(theta);
                    float c = i * 0.3777f;
                    c *= (1f / 12f);
                    aimer.x += c * x;
                    aimer.z += c * z;
                    bulletAttack.aimVector = aimer;
                    bulletAttack.Fire();
                    aimer = Vector3.down;
                }
            }

            EffectData effectData = new EffectData();
            effectData.origin = aimRay.origin + (1 * Vector3.down);
            effectData.scale = 8;
            effectData.rotation = major;

            EffectManager.SpawnEffect(smokeEffectPrefab, effectData, false);
            effectData.scale = 16;
            EffectManager.SpawnEffect(flashEffectPrefab, effectData, false);

            Util.PlaySound("ToTheStars", base.gameObject);

            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();

            anim.SetBool("isToTheStars", false);

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
            return InterruptPriority.PrioritySkill;
        }
    }
}