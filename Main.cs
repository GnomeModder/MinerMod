using EntityStates;
using EntityStates.Croco;
using RoR2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Gnome.EntityStatez
{
    public class MinerMain : GenericCharacterMain
    {
        Animator anim;
        private float moveScale = 1f;

        int moneyTracker = 0;
        float residue = 0;
        int buffCounter = 0;

        public GameObject explodePrefab = Resources.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFX");
        MineTracker mineTracker;

        bool altR = false;
        GenericSkill special;

        bool masteryAchieved = false;
        public static event Action<Run> JunkieAchieved;
        public static event Action<Run> ToolsAchieved;
        public static event Action<Run> SecretAchieved;
        public static event Action<Run> MasteryAchieved;

        public override void OnEnter()
        {
            base.OnEnter();

            if (!base.isAuthority) return;

            anim = characterDirection.modelAnimator;
            anim.enabled = true;
            anim.SetBool("isAlive", true);
            anim.SetFloat("forward", 1f);
            anim.SetFloat("right", 0f);

            moneyTracker = (int)base.characterBody.master.money;

            Loadout loadout = base.characterBody.master.loadout;
            uint specialVar = loadout.bodyLoadoutManager.GetSkillVariant(base.characterBody.bodyIndex, 3);
            if (specialVar == 1) { altR = true; }

            foreach (GenericSkill skill in characterBody.GetComponentsInChildren<GenericSkill>())
            {
                if (skill.skillNameToken == StaticValues.MINER_SPECIAL_TIMEDEXPLOSIVE_NAME)
                {
                    special = skill;
                }
            }

            mineTracker = characterBody.GetComponent<MineTracker>();

            int check1 = Run.instance.loopClearCount;
            int check2 = base.characterBody.inventory.GetTotalItemCountOfTier(ItemTier.Boss) + base.characterBody.inventory.GetTotalItemCountOfTier(ItemTier.Lunar) + base.characterBody.inventory.GetTotalItemCountOfTier(ItemTier.NoTier) + base.characterBody.inventory.GetTotalItemCountOfTier(ItemTier.Tier1) + base.characterBody.inventory.GetTotalItemCountOfTier(ItemTier.Tier2) + base.characterBody.inventory.GetTotalItemCountOfTier(ItemTier.Tier3);
            bool check3 = characterBody.inventory.currentEquipmentIndex == EquipmentIndex.None;
            bool check4 = Run.instance.selectedDifficulty == DifficultyIndex.Hard || Run.instance.selectedDifficulty == DifficultyIndex.Normal;
            bool check5 = RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.bombArtifactDef) || RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.commandArtifactDef) || RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.eliteOnlyArtifactDef) || RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.enigmaArtifactDef) || RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.friendlyFireArtifactDef) || RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.glassArtifactDef) || RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.mixEnemyArtifactDef) || RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.monsterTeamGainsItemsArtifactDef) || RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.randomSurvivorOnRespawnArtifactDef) || RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.sacrificeArtifactDef) || RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.shadowCloneArtifactDef) || RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.singleMonsterTypeArtifactDef) || RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.swarmsArtifactDef) || RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.teamDeathArtifactDef) || RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.weakAssKneesArtifactDef) || RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.wispOnDeath);
            int check6 = 0;
            ReadOnlyCollection<CharacterMaster> readOnlyInstancesList = CharacterMaster.readOnlyInstancesList;
            TeamIndex myTeam = TeamComponent.GetObjectTeam(base.gameObject);
            for (int i = 0; i < readOnlyInstancesList.Count; i++)
            {
                CharacterMaster characterMaster = readOnlyInstancesList[i];
                if (characterMaster.teamIndex == myTeam)
                {
                    check6++;
                }
            }
            //string failed = "failed";
            //if (check1 >= 1) { failed = "passed"; }
            //Chat.AddMessage(failed + " loopCount check");
            //failed = "failed";
            //if (check2 == 0) { failed = "passed"; }
            //Chat.AddMessage(failed + " item check");
            //failed = "failed";
            //if (check3) { failed = "passed"; }
            //Chat.AddMessage(failed + " equipment check");
            //failed = "failed";
            //if (check4) { failed = "passed"; }
            //Chat.AddMessage(failed + " difficulty check");
            //failed = "failed";
            //if (!check5) { failed = "passed"; }
            //Chat.AddMessage(failed + " artifact check");
            //failed = "failed";
            //if (check6 == 1) { failed = "passed"; }
            //Chat.AddMessage(failed + " ally check");

            if (check1 >= 1 && check2 == 0 && check3 && check4 && !check5 && check6 == 1 && ToolsAchieved != null)
            {
                Action<Run> action = ToolsAchieved;
                action(Run.instance);
            }

            if (SceneCatalog.mostRecentSceneDef.baseSceneName == "mysteryspace" && Run.instance.selectedDifficulty == DifficultyIndex.Hard) {masteryAchieved = true; }

            for (int i = 0; i < 6; i++) { secretTimers[i] = 0; }

            //CharacterModel component = characterBody.GetComponent<CharacterModel>();
            //Chat.AddMessage(component.name);
            //TemporaryOverlay temporaryOverlay = base.gameObject.AddComponent<TemporaryOverlay>();
            //temporaryOverlay.duration = 5f;
            //temporaryOverlay.originalMaterial = Resources.Load<Material>("Materials/matIsFrozen");
            //temporaryOverlay.AddToCharacerModel(component);
        }

        public override void Update()
        {
            base.Update();

            anim.SetBool("isMoving", GetModelAnimator().GetBool("isMoving"));
            anim.SetBool("isGrounded", GetModelAnimator().GetBool("isGrounded"));
            
            moveScale = base.characterBody.moveSpeed / base.characterBody.baseMoveSpeed;
            moveScale = (moveScale - 1f) * 0.5f + 1f;
            anim.SetFloat("moveSpeedMultiplier", moveScale);

            float yVelocity = characterMotor.velocity.y;
            anim.SetFloat("yvelo", 0);
            if (yVelocity < 0)
            {
                float value = Mathf.Min(1, yVelocity / -75);
                anim.SetFloat("yvelo", value);
            }

            handleSecret();

            if (Input.GetMouseButtonUp(0))
            {
                anim.SetFloat("forward", 1f);
                anim.SetFloat("right", 0f);
            }

            if (altR)
            {
                mineTracker.mineList.RemoveAll(item => item == null);
                special.SetBonusStockFromBody(buffCounter);

                if (mineTracker.mineList.Count != 0 && (Time.fixedTime - mineTracker.timer) >= 2)
                {
                    mineTracker.mineList.ForEach(detonate);

                    float magnitude = Mathf.Min(80, characterMotor.velocity.magnitude);
                    characterMotor.velocity = magnitude * characterMotor.velocity.normalized;

                    mineTracker.mineList.Clear();
                }
            }

            UpdatePassiveBuff();

            if (buffCounter >= 50 && JunkieAchieved != null) 
            {
                Action<Run> action = JunkieAchieved;
                action(Run.instance);
            }
        }

        public override void OnExit()
        {
            //Vector3 vector = Vector3.up * 3f;
            //if (base.characterMotor)
            //{
            //    vector += base.characterMotor.velocity;
            //    base.characterMotor.enabled = false;
            //}
            //RagdollController component = base.characterDirection.transform.GetComponent<RagdollController>();
            //if (component)
            //{
            //    Chat.AddMessage("nice one");
            //    component.BeginRagdoll(vector);
            //}

            if (base.characterBody.healthComponent.health < 1)
            {
                anim.SetBool("isAlive", false);
            }
            else
            {
                anim.enabled = false;
            }

            mineTracker.mineList.ForEach(detonate);
            mineTracker.mineList.Clear();

            if (masteryAchieved && MasteryAchieved != null)
            {
                Action<Run> action = MasteryAchieved;
                action(Run.instance);
            }

            base.OnExit();
        }

        private void UpdatePassiveBuff()
        {
            int currentCount = base.characterBody.GetBuffCount(MinerPlugin.goldRushIndex);
            int newMoney = (int) base.characterBody.master.money;
            if (moneyTracker < newMoney)
            {
                RefreshExistingStacks(currentCount);
                GiveNewStacks(newMoney);
            }
            moneyTracker = newMoney;

            HandleStackDecay(currentCount);
            HandleSpeedBuff();
        }

        private void RefreshExistingStacks(int currentCount)
        {
            base.characterBody.ClearTimedBuffs(MinerPlugin.goldRushIndex);
            for (int i = 0; i < currentCount; i++)
            {
                base.characterBody.AddTimedBuff(MinerPlugin.goldRushIndex, StaticValues.goldRushDuration);
            }
        }

        private void GiveNewStacks(int newMoney)
        {
            float baseReward = (newMoney - moneyTracker) / Mathf.Pow(Run.instance.difficultyCoefficient, 1.25f);
            residue = (baseReward + residue) % StaticValues.goldRushCost;
            float numStacks = (baseReward + residue) / StaticValues.goldRushCost;
            for (int i = 1; i <= numStacks; i++)
            {
                base.characterBody.AddTimedBuff(MinerPlugin.goldRushIndex, StaticValues.goldRushDuration);
            }
        }

        private void HandleStackDecay(int currentCount)
        {
            if (buffCounter != 0 && currentCount == 0)
            {
                for (int i = 1; i < buffCounter * .5; i++) 
                { 
                    base.characterBody.AddTimedBuff(MinerPlugin.goldRushIndex, 1); 
                }
            }
            buffCounter = base.characterBody.GetBuffCount(MinerPlugin.goldRushIndex);
        }

        private void HandleSpeedBuff()
        {
            if (buffCounter > 0 && !base.characterBody.HasBuff(MinerPlugin.goldRushSpeedIndex)) 
            { 
                base.characterBody.AddBuff(MinerPlugin.goldRushSpeedIndex); 
            }
            if (buffCounter == 0 && base.characterBody.HasBuff(MinerPlugin.goldRushSpeedIndex)) 
            { 
                base.characterBody.RemoveBuff(MinerPlugin.goldRushSpeedIndex); 
            }
        }

        private void detonate(GameObject bomb)
        {
            if (bomb)
            {
                BlastAttack blastAttack = new BlastAttack();
                blastAttack.radius = 10f;
                blastAttack.procCoefficient = 1f;
                blastAttack.position = bomb.transform.position;
                blastAttack.attacker = base.gameObject;
                blastAttack.crit = Util.CheckRoll(base.characterBody.crit, base.characterBody.master);
                blastAttack.baseDamage = base.characterBody.damage * 2 * Mathf.Sqrt(mineTracker.mineList.Count);
                blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
                blastAttack.baseForce = 3f;
                blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
                blastAttack.damageType = DamageType.Generic;
                blastAttack.attackerFiltering = AttackerFiltering.NeverHit;
                blastAttack.Fire();

                EffectData effectData = new EffectData();
                effectData.origin = bomb.transform.position;
                effectData.scale = 6;
                EffectManager.SpawnEffect(explodePrefab, effectData, false);

                Util.PlaySound("Explosive", bomb);

                bombJump(blastAttack);

                Destroy(bomb);
            }
            else
            {
                Chat.AddMessage("DM me if you got this error");
            }
        }

        private void bombJump(BlastAttack ba)
        {
            Vector3 poz = ba.position;
            if (poz != null)
            {
                Vector3 displacement = characterBody.corePosition - poz;
                Vector3 direction = displacement.normalized;
                if (displacement.magnitude < 1.5f) { direction = GetAimRay().direction; }
                characterMotor.velocity += direction * (30f / displacement.magnitude);
            }
        }

        float[] secretTimers = new float[6];

        private void handleSecret()
        {
            if (Input.GetKeyDown(KeyCode.G)) { secretTimers[0] = Time.fixedTime; }
            if (Input.GetKeyDown(KeyCode.N)) { secretTimers[1] = Time.fixedTime; }
            if (Input.GetKeyDown(KeyCode.O)) { secretTimers[2] = Time.fixedTime; }
            if (Input.GetKeyDown(KeyCode.M)) { secretTimers[3] = Time.fixedTime; }
            if (Input.GetKeyDown(KeyCode.E)) { secretTimers[4] = Time.fixedTime; }

            bool successful = true;
            for (int i = 0; i < 5; i++)
            {
                successful = successful && (Time.fixedTime - secretTimers[i]) < 0.5; 
            }

            if (successful && (Time.fixedTime - secretTimers[5]) > 3)
            {
                for (int i = 0; i < 100; i++) { base.characterBody.inventory.GiveItem(ItemIndex.Syringe); }

                Chat.AddMessage("love you");

                if (SecretAchieved != null)
                {
                    Action<Run> action = SecretAchieved;
                    action(Run.instance);
                }

                secretTimers[5] = Time.fixedTime;
            }
        }
    }
}