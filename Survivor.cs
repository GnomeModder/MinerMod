using System;
using System.Collections.Generic;
using BepInEx;
using EntityStates;
using R2API;
using R2API.AssetPlus;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using RoR2.Achievements;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Dak.AchievementLoader.CustomAchievement;
using Gnome.EntityStatez;
using JetBrains.Annotations;
using KinematicCharacterController;
using System.Net.NetworkInformation;
using RoR2.CharacterAI;
using IL.RoR2.Achievements.Commando;
//using UnityEngine.Networking;

namespace Gnome
{
    [R2APISubmoduleDependency("SurvivorAPI")]
    [R2APISubmoduleDependency("PrefabAPI")]
    [R2APISubmoduleDependency("LoadoutAPI")]
    [R2APISubmoduleDependency("LanguageAPI")]
    [R2APISubmoduleDependency("BuffAPI")]
    [R2APISubmoduleDependency("SoundAPI")]
    [R2APISubmoduleDependency("ResourcesAPI")]
    [R2APISubmoduleDependency("AssetAPI")]
    //[R2APISubmoduleDependency("CommandHelper")]
    [BepInDependency("com.bepis.r2api")]
    [BepInDependency("com.dakkhuza.plugins.achievementloader")]
    [BepInPlugin(
        "com.Gnome.MinerMod",
        "MinerMod",
        "1.2.2")]
    public class MinerPlugin : BaseUnityPlugin
    {
        public GameObject minerPrefab;
        //internal static GameObject CentralNetworkObject;
        //private static GameObject _centralNetworkObjectSpawned;
        public GameObject doppelganger;

        static BuffDef goldRushDef = new BuffDef
        {
            name = "Gold Rush",
            iconPath = "Textures/BuffIcons/texBuffOnFireIcon",
            buffColor = Color.yellow,
            canStack = true,
            isDebuff = false
        };

        static CustomBuff goldRush = new CustomBuff(goldRushDef);
        public static BuffIndex goldRushIndex = BuffAPI.Add(goldRush);

        static BuffDef goldRushSpeedDef = new BuffDef
        {
            name = "Gold Rush Speed",
            iconPath = "Textures/BuffIcons/texMovespeedBuffIcon",
            buffColor = Color.yellow,
            canStack = false,
            isDebuff = false
        };

        static CustomBuff goldRushSpeed = new CustomBuff(goldRushSpeedDef);
        public static BuffIndex goldRushSpeedIndex = BuffAPI.Add(goldRushSpeed);

        public void Awake()
        {
            giveCommandoAnExtraSkin();

            registerCharacter();

            float iconDice = UnityEngine.Random.Range(0.0f, 1.0f);

            registerSkills(iconDice);

            registerAlternateSkills(iconDice);

            registerAlternateSkins();

            definePassiveBuff();

            CreateDoppelganger();

            minerPrefab.AddComponent<MineTracker>();

            //registerNetworking();
        }

        private void giveCommandoAnExtraSkin()
        {
            GameObject comMan = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody");
            ModelLocator modell = comMan.GetComponent<ModelLocator>();
            SkinDef[] skins = modell.modelTransform.GetComponent<ModelSkinController>().skins;

            //float tff = 255;

            //Color pupTop = new Color(86f / tff, 18f / tff, 93f / tff);
            //Color pupBot = new Color(75f / tff, 63f / tff, 39f / tff);
            //Color pupLeft = new Color(0f, 0f, 0f);
            //Color pupRight = new Color(235f / tff, 209f / tff, 93f / tff);
            //Color pupLine = new Color(75f / tff, 27f / tff, 26f / tff, 0.5f);
            //Sprite pupSkinIcon = LoadoutAPI.CreateSkinIcon(pupTop, pupRight, pupBot, pupLeft, pupLine);

            SkinDef minerPupSkin = skinCopy(skins[0]);
            minerPupSkin.icon = Assets.pupSkinIconSprite;
            minerPupSkin.name = "skinMinerAlt2";
            minerPupSkin.nameToken = "GNOMEMINER_SKIN_ALT2_NAME";
            minerPupSkin.unlockableName = "Skins.GnomeMiner.Alt2";

            LoadoutAPI.AddSkinToCharacter(comMan, minerPupSkin);
        }

        private void registerCharacter()
        {
            //Load your base character body, from somewhere in the game.
            minerPrefab = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").InstantiateClone("MinerPlayerBody");
            ///Get the model without any components.
            GameObject gameObject = minerPrefab.GetComponent<ModelLocator>().modelBaseTransform.gameObject;

            registerModelSwap(gameObject);

            foreach (GenericSkill skill in minerPrefab.GetComponentsInChildren<GenericSkill>())
            {
                DestroyImmediate(skill);
            }
            SkillLocator skillLocator = minerPrefab.GetComponent<SkillLocator>();
            skillLocator.SetFieldValue<GenericSkill[]>("allSkills", new GenericSkill[0]);

            //Do the following for each of primary, secondary, utility and special.

            skillLocator.primary = minerPrefab.AddComponent<GenericSkill>();
            SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            newFamily.variants = new SkillFamily.Variant[1];
            LoadoutAPI.AddSkillFamily(newFamily);
            skillLocator.primary.SetFieldValue("_skillFamily", newFamily);

            skillLocator.secondary = minerPrefab.AddComponent<GenericSkill>();
            SkillFamily nawFamily = ScriptableObject.CreateInstance<SkillFamily>();
            nawFamily.variants = new SkillFamily.Variant[1];
            LoadoutAPI.AddSkillFamily(nawFamily);
            skillLocator.secondary.SetFieldValue("_skillFamily", nawFamily);

            skillLocator.utility = minerPrefab.AddComponent<GenericSkill>();
            SkillFamily nowFamily = ScriptableObject.CreateInstance<SkillFamily>();
            nowFamily.variants = new SkillFamily.Variant[1];
            LoadoutAPI.AddSkillFamily(nowFamily);
            skillLocator.utility.SetFieldValue("_skillFamily", nowFamily);

            skillLocator.special = minerPrefab.AddComponent<GenericSkill>();
            SkillFamily nywFamily = ScriptableObject.CreateInstance<SkillFamily>();
            nywFamily.variants = new SkillFamily.Variant[1];
            LoadoutAPI.AddSkillFamily(nywFamily);
            skillLocator.special.SetFieldValue("_skillFamily", nywFamily);

            BodyCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(minerPrefab);
            };

            CharacterBody component = minerPrefab.GetComponent<CharacterBody>();
            component.baseDamage = 12f;
            component.levelDamage = 1.6f;
            component.baseMaxHealth = 110f;
            component.levelMaxHealth = 40f;
            component.baseArmor = 10f;
            component.baseRegen = 1f;
            component.levelRegen = 0.2f;
            component.baseMoveSpeed = 7f;
            component.levelMoveSpeed = 0f;
            component.baseAttackSpeed = 1f;
            component.name = "Miner";
            component.baseNameToken = "GNOMEMINER_NAME";
            component.portraitIcon = Assets.minerIcon;
            component.subtitleNameToken = "GNOMEMINER_SUBTITLE";

            LanguageAPI.Add("GNOMEMINER_NAME", "Miner");
            LanguageAPI.Add("GNOMEMINER_SUBTITLE", "Destructive Drug Addict");
            LanguageAPI.Add("GNOMEMINER_LORE", StaticValues.MINER_LORE);

            LanguageAPI.Add("ACHIEVEMENT_MINERJUNKIE_NAME", "Miner: Junkie");
            LanguageAPI.Add("ACHIEVEMENT_MINERJUNKIE_DESCRIPTION", "As Miner, gain 50 stacks of Adrenaline");

            LanguageAPI.Add("ACHIEVEMENT_MINERCOMPACTED_NAME", "Miner: Compacted");
            LanguageAPI.Add("ACHIEVEMENT_MINERCOMPACTED_DESCRIPTION", "As Miner, hit 7 enemies with one Crush");

            LanguageAPI.Add("ACHIEVEMENT_MINERBROUGHTHISOWNTOOLS_NAME", "Miner: Brought His Own Tools");
            LanguageAPI.Add("ACHIEVEMENT_MINERBROUGHTHISOWNTOOLS_DESCRIPTION", "As Miner, loop back to the first stage on Rainstorm or Monsoon, no artifacts, allies, or items");

            LanguageAPI.Add("GNOMEMINER_SKIN_DEF_NAME", "Default");

            LanguageAPI.Add("GNOMEMINER_SKIN_ALT1_NAME", "Molten");
            LanguageAPI.Add("ACHIEVEMENT_MINERMASTERY_NAME", "Miner: Mastery");
            LanguageAPI.Add("ACHIEVEMENT_MINERMASTERY_DESCRIPTION", "As Miner, Obliterate yourself at the Obelisk on Monsoon");

            LanguageAPI.Add("GNOMEMINER_SKIN_ALT2_NAME", "Puple");
            LanguageAPI.Add("ACHIEVEMENT_MINERSECRET_NAME", "Miner: The Five Keys");
            LanguageAPI.Add("ACHIEVEMENT_MINERSECRET_DESCRIPTION", "As Miner, Discover G-N-O-M-E's debugging secret");

            AssetBundleResourcesProvider provider = new AssetBundleResourcesProvider("@MinerMod", Assets.minerAssetBundle);
            ResourcesAPI.AddProvider(provider);

            minerPrefab.GetComponent<CharacterBody>().preferredPodPrefab = Resources.Load<GameObject>("Prefabs/CharacterBodies/toolbotbody").GetComponent<CharacterBody>().preferredPodPrefab;

            var stateMachine = component.GetComponent<EntityStateMachine>();
            stateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStatez.MinerMain));

            SurvivorDef survivorDef = new SurvivorDef
            {
                bodyPrefab = minerPrefab,
                descriptionToken = StaticValues.MINER_DESCRIPTION + "\r\n",
                displayPrefab = Assets.minerAssetBundle.LoadAsset<GameObject>("rorMiner2"),
                primaryColor = new Color(0.956862745f, 0.874509803f, 0.603921568f),
                name = "Miner",
                unlockableName = ""
            };
            SurvivorAPI.AddSurvivor(survivorDef);
        }

        private void registerSkills(float iconDice)
        {
            var crushDef = ScriptableObject.CreateInstance<SkillDef>();
            crushDef.activationState = new SerializableEntityStateType(typeof(EntityStatez.CrushState));
            crushDef.activationStateMachineName = "Weapon";
            crushDef.baseMaxStock = 1;
            crushDef.baseRechargeInterval = 0f;
            crushDef.beginSkillCooldownOnSkillEnd = true;
            crushDef.canceledFromSprinting = false;
            crushDef.fullRestockOnAssign = true;
            crushDef.interruptPriority = InterruptPriority.Any;
            crushDef.isBullets = false;
            crushDef.isCombatSkill = false;
            crushDef.mustKeyPress = false;
            crushDef.noSprint = false;
            crushDef.rechargeStock = 1;
            crushDef.requiredStock = 1;
            crushDef.shootDelay = 0.5f;
            crushDef.stockToConsume = 1;
            crushDef.icon = Assets.minerCrushIconSprite;
            if (iconDice >= .5f) { crushDef.icon = Assets.zachMinerCrushIconSprite; }
            crushDef.skillDescriptionToken = StaticValues.MINER_PRIMARY_CRUSH_DESCRIPTION;
            crushDef.skillName = StaticValues.MINER_PRIMARY_CRUSH_NAME;
            crushDef.skillNameToken = StaticValues.MINER_PRIMARY_CRUSH_NAME;

            LoadoutAPI.AddSkillDef(crushDef);

            var drillChargeDef = ScriptableObject.CreateInstance<SkillDef>();
            drillChargeDef.activationState = new SerializableEntityStateType(typeof(EntityStatez.DrillChargingState));
            drillChargeDef.activationStateMachineName = "Weapon";
            drillChargeDef.baseMaxStock = 1;
            drillChargeDef.beginSkillCooldownOnSkillEnd = true;
            drillChargeDef.canceledFromSprinting = false;
            drillChargeDef.fullRestockOnAssign = true;
            drillChargeDef.interruptPriority = InterruptPriority.Frozen;
            drillChargeDef.isBullets = false;
            drillChargeDef.isCombatSkill = false;
            drillChargeDef.mustKeyPress = true;
            drillChargeDef.noSprint = false;
            drillChargeDef.rechargeStock = 1;
            drillChargeDef.requiredStock = 1;
            drillChargeDef.shootDelay = 0.5f;
            drillChargeDef.stockToConsume = 1;
            drillChargeDef.icon = Assets.minerDrillChargeIconSprite;
            if (iconDice >= .5f) { drillChargeDef.icon = Assets.zachMinerDrillChargeIconSprite; }
            drillChargeDef.skillDescriptionToken = StaticValues.MINER_SECONDARY_DRILLCHARGE_DESCRIPTION;
            drillChargeDef.skillName = StaticValues.MINER_SECONDARY_DRILLCHARGE_NAME;
            drillChargeDef.skillNameToken = StaticValues.MINER_SECONDARY_DRILLCHARGE_NAME;
            drillChargeDef.baseRechargeInterval = 7f;

            LoadoutAPI.AddSkillDef(drillChargeDef);

            var backBlastDef = ScriptableObject.CreateInstance<SkillDef>();
            backBlastDef.activationState = new SerializableEntityStateType(typeof(EntityStatez.BackBlastState));
            backBlastDef.activationStateMachineName = "Weapon";
            backBlastDef.baseMaxStock = 1;
            backBlastDef.beginSkillCooldownOnSkillEnd = true;
            backBlastDef.canceledFromSprinting = false;
            backBlastDef.fullRestockOnAssign = true;
            backBlastDef.interruptPriority = InterruptPriority.Skill;
            backBlastDef.isBullets = false;
            backBlastDef.isCombatSkill = false;
            backBlastDef.mustKeyPress = true;
            backBlastDef.noSprint = false;
            backBlastDef.rechargeStock = 1;
            backBlastDef.requiredStock = 1;
            backBlastDef.shootDelay = 0.5f;
            backBlastDef.stockToConsume = 1;
            backBlastDef.icon = Assets.minerBackblastIconSprite;
            if (iconDice >= .5f) { backBlastDef.icon = Assets.zachMinerBackblastIconSprite; }
            backBlastDef.skillDescriptionToken = StaticValues.MINER_UTILITY_BACKBLAST_DESCRIPTION;
            backBlastDef.skillName = StaticValues.MINER_UTILITY_BACKBLAST_NAME;
            backBlastDef.skillNameToken = StaticValues.MINER_UTILITY_BACKBLAST_NAME;
            backBlastDef.baseRechargeInterval = 5f;

            LoadoutAPI.AddSkillDef(backBlastDef);

            var toTheStarsDef = ScriptableObject.CreateInstance<SkillDef>();
            toTheStarsDef.activationState = new SerializableEntityStateType(typeof(EntityStatez.ToTheStarsState));
            toTheStarsDef.activationStateMachineName = "Weapon";
            toTheStarsDef.baseMaxStock = 1;
            toTheStarsDef.beginSkillCooldownOnSkillEnd = true;
            toTheStarsDef.canceledFromSprinting = false;
            toTheStarsDef.fullRestockOnAssign = true;
            toTheStarsDef.interruptPriority = InterruptPriority.Frozen;
            toTheStarsDef.isBullets = false;
            toTheStarsDef.isCombatSkill = false;
            toTheStarsDef.mustKeyPress = true;
            toTheStarsDef.noSprint = true;
            toTheStarsDef.rechargeStock = 1;
            toTheStarsDef.requiredStock = 1;
            toTheStarsDef.shootDelay = 0.5f;
            toTheStarsDef.stockToConsume = 1;
            toTheStarsDef.icon = Assets.minerToTheStarsIconSprite;
            if (iconDice >= .5f) { toTheStarsDef.icon = Assets.zachMinerToTheStarsIconSprite; }
            toTheStarsDef.skillDescriptionToken = StaticValues.MINER_SPECIAL_TOTHESTARS_DESCRIPTION;
            toTheStarsDef.skillName = StaticValues.MINER_SPECIAL_TOTHESTARS_NAME;
            toTheStarsDef.skillNameToken = StaticValues.MINER_SPECIAL_TOTHESTARS_NAME;
            toTheStarsDef.baseRechargeInterval = 5f;

            LoadoutAPI.AddSkillDef(toTheStarsDef);

            SkillLocator skillLocator = minerPrefab.GetComponent<SkillLocator>();

            var skillFamily = skillLocator.primary.skillFamily;

            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = crushDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(crushDef.skillNameToken, false, null)
            };

            skillFamily = skillLocator.secondary.skillFamily;
            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = drillChargeDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(drillChargeDef.skillNameToken, false, null)
            };

            skillFamily = skillLocator.utility.skillFamily;
            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = backBlastDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(backBlastDef.skillNameToken, false, null)
            };

            skillFamily = skillLocator.special.skillFamily;
            skillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = toTheStarsDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node(toTheStarsDef.skillNameToken, false, null)
            };

            skillLocator.passiveSkill.enabled = true;
            skillLocator.passiveSkill.skillDescriptionToken = StaticValues.MINER_PASSIVE_GOLDRUSH_DESCRIPTION;
            skillLocator.passiveSkill.skillNameToken = StaticValues.MINER_PASSIVE_GOLDRUSH_NAME;
            skillLocator.passiveSkill.icon = Assets.zachMinerGoldRushIconSprite;
        }

        private void registerAlternateSkills(float iconDice)
        {
            var drillBreakDef = ScriptableObject.CreateInstance<SkillDef>();
            drillBreakDef.activationState = new SerializableEntityStateType(typeof(EntityStatez.DrillBreakingState));
            drillBreakDef.activationStateMachineName = "Weapon";
            drillBreakDef.baseMaxStock = 1;
            drillBreakDef.beginSkillCooldownOnSkillEnd = true;
            drillBreakDef.canceledFromSprinting = false;
            drillBreakDef.fullRestockOnAssign = true;
            drillBreakDef.interruptPriority = InterruptPriority.Frozen;
            drillBreakDef.isBullets = false;
            drillBreakDef.isCombatSkill = false;
            drillBreakDef.mustKeyPress = true;
            drillBreakDef.noSprint = false;
            drillBreakDef.rechargeStock = 1;
            drillBreakDef.requiredStock = 1;
            drillBreakDef.shootDelay = 0.5f;
            drillBreakDef.stockToConsume = 1;
            drillBreakDef.icon = Assets.minerCrackHammerIconSprite;
            if (iconDice >= .5f) { drillBreakDef.icon = Assets.zachMinerCrackHammerIconSprite; }
            drillBreakDef.skillDescriptionToken = StaticValues.MINER_SECONDARY_CRACKHAMMER_DESCRIPTION;
            drillBreakDef.skillName = StaticValues.MINER_SECONDARY_CRACKHAMMER_NAME;
            drillBreakDef.skillNameToken = StaticValues.MINER_SECONDARY_CRACKHAMMER_NAME;
            drillBreakDef.baseRechargeInterval = 3f;

            LoadoutAPI.AddSkillDef(drillBreakDef);

            var vaultDef = ScriptableObject.CreateInstance<SkillDef>();
            vaultDef.activationState = new SerializableEntityStateType(typeof(EntityStatez.VaultState));
            vaultDef.activationStateMachineName = "Weapon";
            vaultDef.baseMaxStock = 1;
            vaultDef.beginSkillCooldownOnSkillEnd = true;
            vaultDef.canceledFromSprinting = false;
            vaultDef.fullRestockOnAssign = true;
            vaultDef.interruptPriority = InterruptPriority.Skill;
            vaultDef.isBullets = false;
            vaultDef.isCombatSkill = false;
            vaultDef.mustKeyPress = true;
            vaultDef.noSprint = false;
            vaultDef.rechargeStock = 1;
            vaultDef.requiredStock = 1;
            vaultDef.shootDelay = 0.5f;
            vaultDef.stockToConsume = 1;
            vaultDef.icon = Assets.minerCaveInIconSprite;
            if (iconDice >= .5f) { vaultDef.icon = Assets.zachMinerCaveInIconSprite; }
            vaultDef.skillDescriptionToken = StaticValues.MINER_UTILITY_CAVEIN_DESCRIPTION;
            vaultDef.skillName = StaticValues.MINER_UTILITY_CAVEIN_NAME;
            vaultDef.skillNameToken = StaticValues.MINER_UTILITY_CAVEIN_NAME;
            vaultDef.baseRechargeInterval = 5f;

            LoadoutAPI.AddSkillDef(vaultDef);

            var cleaveDef = ScriptableObject.CreateInstance<SkillDef>();
            cleaveDef.activationState = new SerializableEntityStateType(typeof(EntityStatez.CleaveState));
            cleaveDef.activationStateMachineName = "Weapon";
            cleaveDef.baseMaxStock = 3;
            cleaveDef.beginSkillCooldownOnSkillEnd = true;
            cleaveDef.canceledFromSprinting = false;
            cleaveDef.fullRestockOnAssign = true;
            cleaveDef.interruptPriority = InterruptPriority.Skill;
            cleaveDef.isBullets = false;
            cleaveDef.isCombatSkill = false;
            cleaveDef.mustKeyPress = true;
            cleaveDef.noSprint = false;
            cleaveDef.rechargeStock = 3;
            cleaveDef.requiredStock = 1;
            cleaveDef.shootDelay = 0.5f;
            cleaveDef.stockToConsume = 1;
            cleaveDef.icon = Assets.minerTimedExplosiveIconSprite;
            if (iconDice >= .5f) { cleaveDef.icon = Assets.zachMinerTimedExplosiveIconSprite; }
            cleaveDef.skillDescriptionToken = StaticValues.MINER_SPECIAL_TIMEDEXPLOSIVE_DESCRIPTION;
            cleaveDef.skillName = StaticValues.MINER_SPECIAL_TIMEDEXPLOSIVE_NAME;
            cleaveDef.skillNameToken = StaticValues.MINER_SPECIAL_TIMEDEXPLOSIVE_NAME;
            cleaveDef.baseRechargeInterval = 6f;

            LoadoutAPI.AddSkillDef(cleaveDef);

            SkillLocator skillLocator = minerPrefab.GetComponent<SkillLocator>();

            var skillFamily = skillLocator.secondary.skillFamily;

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[1] = new SkillFamily.Variant
            {
                skillDef = drillBreakDef,
                unlockableName = "Skills.GnomeMiner.CrackHammer",
                viewableNode = new ViewablesCatalog.Node(drillBreakDef.skillNameToken, false, null)
            };

            skillFamily = skillLocator.utility.skillFamily;

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[1] = new SkillFamily.Variant
            {
                skillDef = vaultDef,
                unlockableName = "Skills.GnomeMiner.CaveIn",
                viewableNode = new ViewablesCatalog.Node(vaultDef.skillNameToken, false, null)
            };

            skillFamily = skillLocator.special.skillFamily;

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[1] = new SkillFamily.Variant
            {
                skillDef = cleaveDef,
                unlockableName = "Skills.GnomeMiner.TimedExplosive",
                viewableNode = new ViewablesCatalog.Node(cleaveDef.skillNameToken, false, null)
            };
        }

        private void registerAlternateSkins()
        {
            ModelLocator modell = minerPrefab.GetComponent<ModelLocator>();
            SkinDef[] skins = modell.modelTransform.GetComponent<ModelSkinController>().skins;

            //float tff = 255f;

            //Color defTop = new Color(90f / tff, 83f / tff, 31f / tff);
            //Color defBot = new Color(49f / tff, 45f / tff, 42f / tff);
            //Color defLeft = new Color(0f, 0f, 0f);
            //Color defRight = new Color(202f / tff, 184f / tff, 99f / tff);
            //Color defLine = new Color(49f / tff, 45f / tff, 42f / tff, 0.5f);
            //Sprite defSkinIcon = LoadoutAPI.CreateSkinIcon(defTop, defRight, defBot, defLeft, defLine);

            SkinDef minerDefSkin = skinCopy(skins[0]);
            minerDefSkin.icon = Assets.defSkinIconSprite;
            minerDefSkin.name = "skinMinerDef";
            minerDefSkin.nameToken = "GNOMEMINER_SKIN_DEF_NAME";
            skins[0] = minerDefSkin;

            //Color masTop = new Color(118f / tff, 41f / tff, 15f / tff);
            //Color masBot = new Color(74f / tff, 27f / tff, 26f / tff);
            //Color masLeft = new Color(37f / tff, 0f, 0f);
            //Color masRight = new Color(210f / tff, 125f / tff, 82f / tff);
            //Color masLine = new Color(74f / tff, 27f / tff, 26f / tff, 0.5f);
            //Sprite masSkinIcon = LoadoutAPI.CreateSkinIcon(masTop, masRight, masBot, masLeft, masLine);

            SkinDef minerMasSkin = skinCopy(skins[0]);
            minerMasSkin.icon = Assets.masSkinIconSprite;
            minerMasSkin.name = "skinMinerAlt1";
            minerMasSkin.nameToken = "GNOMEMINER_SKIN_ALT1_NAME";
            minerMasSkin.unlockableName = "Skins.GnomeMiner.Alt1";
            skins[1] = minerMasSkin;
        }

        private SkinDef skinCopy(SkinDef orig)
        {
            LoadoutAPI.SkinDefInfo outputInfo = new LoadoutAPI.SkinDefInfo();
            outputInfo.BaseSkins = orig.baseSkins;
            outputInfo.GameObjectActivations = orig.gameObjectActivations;
            outputInfo.Icon = orig.icon;
            outputInfo.MeshReplacements = orig.meshReplacements;
            outputInfo.Name = orig.name;
            outputInfo.NameToken = orig.nameToken;
            outputInfo.RendererInfos = orig.rendererInfos;
            outputInfo.RootObject = orig.rootObject;
            outputInfo.UnlockableName = orig.unlockableName;
            return LoadoutAPI.CreateNewSkinDef(outputInfo);
        }

        private void definePassiveBuff()
        {
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                    x => x.MatchLdloc(50),
                    x => x.MatchLdloc(51)
                    );
                    //x => x.MatchLdloc(52),
                    //x => x.MatchDiv(),
                    //x => x.MatchMul()
                    //);
                c.Index += 2;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<CharacterBody, float>>((charBody) =>
                {
                    float output = 0f;
                    if (charBody.HasBuff(goldRushSpeedIndex))
                    {
                        output = 0.4f;
                    }
                    return output;
                });
                c.Emit(OpCodes.Add);
            };

            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                    x => x.MatchMul(),
                    x => x.MatchAdd(),
                    x => x.MatchStloc(58)
                    );
                c.Index += 2;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<CharacterBody, float>>((charBody) =>
                {
                    float output = 0f;
                    if (charBody.HasBuff(goldRushIndex))
                    {
                        output = charBody.GetBuffCount(goldRushIndex) * StaticValues.goldRushAttackBuffValue;
                    }
                    return output;
                });
                c.Emit(OpCodes.Add);
            };

            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                    x => x.MatchMul(),
                    x => x.MatchLdloc(43),
                    x => x.MatchMul(),
                    x => x.MatchStloc(47)
                    );
                c.Index += 3;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<CharacterBody, float>>((charBody) =>
                {
                    float output = 0f;
                    if (charBody.HasBuff(goldRushIndex))
                    {
                        output = charBody.GetBuffCount(goldRushIndex) * StaticValues.goldRushHealBuffValue;
                    }
                    return output;
                });
                c.Emit(OpCodes.Add);
            };

            //int moneyTracker = 0;
            //float residue = 0;
            //int buffCount = 0;
            //On.RoR2.UI.HUD.Update += (orig, self) =>
            //{
            //    if (self.targetBodyObject != null)
            //    {
            //        CharacterBody charBody = self.targetBodyObject.GetComponent<CharacterBody>();
            //        if (charBody.baseNameToken == "GNOME_MINER_NAME")
            //        {
            //            int currentCount = charBody.GetBuffCount(goldRushIndex);
            //            if (self.moneyText)
            //            {
            //                int newMoney = (int)(self.targetMaster ? self.targetMaster.money : 0U);
            //                if (moneyTracker < newMoney)
            //                {
            //                    charBody.ClearTimedBuffs(goldRushIndex);
            //                    for (int i = 0; i < currentCount; i++)
            //                    {
            //                        charBody.AddTimedBuff(goldRushIndex, StaticValues.goldRushDuration);
            //                    }
            //                    float baseReward = (newMoney - moneyTracker) / Mathf.Pow(Run.instance.difficultyCoefficient, 1.25f);
            //                    residue = (baseReward + residue) % StaticValues.goldRushCost;
            //                    float numStacks = (baseReward + residue) / StaticValues.goldRushCost;
            //                    for (int i = 1; i <= numStacks; i++)
            //                    {
            //                        charBody.AddTimedBuff(goldRushIndex, StaticValues.goldRushDuration);
            //                    }
            //                }
            //                moneyTracker = newMoney;
            //            }
            //            if (buffCount != 0 && currentCount == 0)
            //            {
            //                for (int i = 1; i < buffCount * .5; i++) { charBody.AddTimedBuff(goldRushIndex, 1); }
            //            }
            //            buffCount = charBody.GetBuffCount(goldRushIndex);

            //            if (buffCount > 0 && !charBody.HasBuff(goldRushSpeedIndex)) { charBody.AddBuff(goldRushSpeedIndex); }
            //            if (buffCount == 0 && charBody.HasBuff(goldRushSpeedIndex)) { charBody.RemoveBuff(goldRushSpeedIndex); }
            //        }
            //    }
            //    orig(self);
            //};
        }

        private void CreateDoppelganger()
        {
            // set up the doppelganger for artifact of vengeance here
            // quite simple, gets a bit more complex if you're adding your own ai, but commando ai will do

            doppelganger = Resources.Load<GameObject>("Prefabs/CharacterMasters/MageMonsterMaster").InstantiateClone("MinerMonsterMaster");
            AISkillDriver[] drivers = doppelganger.GetComponents<AISkillDriver>();

            for (int i = 0; i < drivers.Length; i++)
            {
                AISkillDriver driver = drivers[i];
                
                if (driver.skillSlot == SkillSlot.Primary)
                {
                    driver.minDistance = 0;
                    driver.maxDistance = 4;
                }
                if (driver.skillSlot == SkillSlot.Special)
                {
                    Destroy(driver);
                }
                driver.minDistance = 0;
            }

            GameObject commast = Resources.Load<GameObject>("Prefabs/CharacterMasters/CommandoMonsterMaster");
            AISkillDriver[] otherDrivers = commast.GetComponents<AISkillDriver>();

            for (int i = 0; i < drivers.Length; i++)
            {
                AISkillDriver driver = drivers[i];
                if (driver.skillSlot == SkillSlot.Special)
                {
                    AISkillDriver copy = doppelganger.AddComponent<AISkillDriver>();
                    copy.activationRequiresAimConfirmation = driver.activationRequiresAimConfirmation;
                    copy.activationRequiresTargetLoS = driver.activationRequiresTargetLoS;
                    copy.aimType = driver.aimType;
                    copy.customName = driver.customName;
                    copy.driverUpdateTimerOverride = driver.driverUpdateTimerOverride;
                    copy.ignoreNodeGraph = driver.ignoreNodeGraph;
                    copy.maxDistance = driver.maxDistance;
                    copy.maxTargetHealthFraction = driver.maxTargetHealthFraction;
                    copy.maxUserHealthFraction = driver.maxUserHealthFraction;
                    copy.minDistance = 0;
                    copy.minTargetHealthFraction = driver.minTargetHealthFraction;
                    copy.minUserHealthFraction = driver.minUserHealthFraction;
                    copy.moveInputScale = driver.moveInputScale;
                    copy.movementType = driver.movementType;
                    copy.moveTargetType = driver.moveTargetType;
                    copy.noRepeat = driver.noRepeat;
                    copy.requiredSkill = driver.requiredSkill;
                    copy.requireEquipmentReady = driver.requireEquipmentReady;
                    copy.requireSkillReady = driver.requireSkillReady;
                    copy.resetCurrentEnemyOnNextDriverSelection = driver.resetCurrentEnemyOnNextDriverSelection;
                    copy.selectionRequiresTargetLoS = driver.selectionRequiresTargetLoS;
                    copy.shouldFireEquipment = driver.shouldFireEquipment;
                    copy.shouldSprint = driver.shouldSprint;
                    copy.shouldTapButton = driver.shouldTapButton;
                    copy.skillSlot = driver.skillSlot;
                }
            }

            MasterCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(doppelganger);
            };

            CharacterMaster component = doppelganger.GetComponent<CharacterMaster>();
            component.bodyPrefab = minerPrefab;
        }

        private void registerModelSwap(GameObject DisplayPrefab)
        {
            // swap the display model first
            var skinnedMeshes = DisplayPrefab.GetComponentsInChildren<SkinnedMeshRenderer>();
            var meshRenderers = DisplayPrefab.GetComponentsInChildren<MeshRenderer>();
            // sephs model GO
            var minerDisplayGO = Assets.minerAssetBundle.LoadAsset<GameObject>("rorMinerM").InstantiateClone("MinerDisplay", false);
            bool swapped = false;
            foreach (var skinMesh in skinnedMeshes)
            {
                if (!swapped)
                {
                    minerDisplayGO.transform.parent = skinMesh.transform.parent;
                    minerDisplayGO.transform.position = Vector3.zero;
                    swapped = true;
                }
                skinMesh.gameObject.SetActive(false);
            }
            foreach (var mesh in meshRenderers)
            {
                mesh.gameObject.SetActive(false);
            }

            // give sephiroth his ingame model swap component
            minerPrefab.AddComponent<MinerModelSwap>();
        }

        //private void registerNetworking()
        //{
        //    //We create an empty gameobject to hold all our networked components. The name of this GameObject is largely irrelevant.
        //    var tmpGo = new GameObject("timpGo");
        //    //Add the networkidentity so that Unity knows which Object it's going to be networking all about.
        //    tmpGo.AddComponent<NetworkIdentity>();
        //    //Thirdly, we use InstantiateClone from the PrefabAPI to make sure we have full control over our GameObject.
        //    CentralNetworkObject = tmpGo.InstantiateClone("MinerNetworkingObject");
        //    // Delete the now useless temporary GameObject
        //    GameObject.Destroy(tmpGo);
        //    //Finally, we add a specific component that we want networked. In this example, we will only be adding one, but you can add as many components here as you like. Make sure these components inherit from NetworkBehaviour.
        //    CentralNetworkObject.AddComponent<MyNetworkComponent>();

        //    //In this specific example, we use a console command. You can look at https://github.com/risk-of-thunder/R2Wiki/wiki/Console-Commands for more information on that.
        //    CommandHelper.AddToConsoleWhenReady();
        //}

        //[ConCommand(commandName = "debuglog_on_all", flags = ConVarFlags.ExecuteOnServer, helpText = "Logs a network message to all connected people.")]
        //private static void CCNetworkLog(ConCommandArgs args)
        //{
        //    //Although here it's not relevant, you can ensure you are the server by checking if the NetworkServer is active.
        //    if (NetworkServer.active)
        //    {
        //        //Before we can Invoke our NetworkMessage, we need to make sure our centralized networkobject is spawned.
        //        // For doing that, we Instantiate the CentralNetworkObject, we obviously check if we don't already have one that is already instantiated and activated in the current scene.
        //        // Note : Make sure you Instantiate the gameobject, and not spawn it directly, it would get deleted otherwise on scene change, even with DontDestroyOnLoad.
        //        if (!_centralNetworkObjectSpawned)
        //        {
        //            _centralNetworkObjectSpawned =
        //                UnityEngine.Object.Instantiate(CentralNetworkObject);
        //            NetworkServer.Spawn(_centralNetworkObjectSpawned);
        //        }
        //        //This readOnlyInstancesList is great for going over all players in general, 
        //        // so it might be worth commiting to memory.
        //        foreach (NetworkUser user in NetworkUser.readOnlyInstancesList)
        //        {
        //            //Args.userArgs is a list of all words in the command arguments.
        //            MyNetworkComponent.Invoke(user, string.Join(" ", args.userArgs));
        //        }
        //    }
        //}

        [CustomUnlockable("Skills.GnomeMiner.CrackHammer", "ACHIEVEMENT_JUNKIE_NAME")]
        [MinerAchievementAttribute("MinerJunkie", "Skills.GnomeMiner.CrackHammer", null, null)]
        public class JunkieAchievement : BaseAchievement
        {
            public override void OnInstall()
            {
                base.OnInstall();
                MinerMain.JunkieAchieved += AutoGrant;
            }
            public override void OnUninstall()
            {
                base.OnUninstall();
                MinerMain.JunkieAchieved -= AutoGrant;
            }
            public void AutoGrant(Run run)
            {
                Grant();
            }
        }

        [CustomUnlockable("Skills.GnomeMiner.CaveIn", "ACHIEVEMENT_COMPACTED_NAME")]
        [MinerAchievementAttribute("MinerCompacted", "Skills.GnomeMiner.CaveIn", null, null)]
        public class CompactedAchievement : BaseAchievement
        {
            public override void OnInstall()
            {
                base.OnInstall();
                CrushState.CompactedAchieved += AutoGrant;
            }
            public override void OnUninstall()
            {
                base.OnUninstall();
                CrushState.CompactedAchieved -= AutoGrant;
            }
            public void AutoGrant(Run run)
            {
                Grant();
            }
        }

        [CustomUnlockable("Skills.GnomeMiner.TimedExplosive", "ACHIEVEMENT_BROUGHTHISOWNTOOLS_NAME")]
        [MinerAchievementAttribute("MinerBroughtHisOwnTools", "Skills.GnomeMiner.TimedExplosive", null, null)]
        public class BroughtHisOwnToolsAchievement : BaseAchievement
        {
            public override void OnInstall()
            {
                base.OnInstall();
                MinerMain.ToolsAchieved += AutoGrant;
            }
            public override void OnUninstall()
            {
                base.OnUninstall();
                MinerMain.ToolsAchieved -= AutoGrant;
            }
            public void AutoGrant(Run run)
            {
                Grant();
            }
        }

        [CustomUnlockable("Skins.GnomeMiner.Alt1", "ACHIEVEMENT_MINERMASTERY_NAME")]
        [MinerAchievementAttribute("MinerMastery", "Skins.GnomeMiner.Alt1", null, null)]
        public class MasteryAchievement : BaseAchievement
        {
            public override void OnInstall()
            {
                base.OnInstall();
                MinerMain.MasteryAchieved += AutoGrant;
            }
            public override void OnUninstall()
            {
                base.OnUninstall();
                MinerMain.MasteryAchieved -= AutoGrant;
            }
            public void AutoGrant(Run run)
            {
                Grant();
            }
        }

        [CustomUnlockable("Skins.GnomeMiner.Alt2", "ACHIEVEMENT_MINERSECRET_NAME")]
        [MinerAchievementAttribute("MinerSecret", "Skins.GnomeMiner.Alt2", null, null)]
        public class SecretAchievement : BaseAchievement
        {
            public override void OnInstall()
            {
                base.OnInstall();
                MinerMain.SecretAchieved += AutoGrant;
            }
            public override void OnUninstall()
            {
                base.OnUninstall();
                MinerMain.SecretAchieved -= AutoGrant;
            }
            public void AutoGrant(Run run)
            {
                Grant();
            }
        }
    }

    public class MinerAchievementAttribute : CustomAchievementAttribute
    {
        public MinerAchievementAttribute([NotNull] string identifier, string unlockableRewardIdentifier, string prerequisiteAchievementIdentifier, Type serverTrackerType = null) : base(identifier, unlockableRewardIdentifier, prerequisiteAchievementIdentifier, serverTrackerType) { }
        
        public override string ProviderPrefix
        {
            get { return "@MinerMod:"; }
        }
    }

    public class MinerModelSwap : MonoBehaviour
    {
        CharacterDirection direction;
        CharacterBody self;
        CharacterMotor motor;

        MinerSkins[] minerSkins;
        ModelSkinController modellCont;
        int skindex = -1;
        void Start()
        {
            // swap the model
            var miner = Instantiate(Assets.minerAssetBundle.LoadAsset<GameObject>("rorMiner1"));
            direction = this.transform.root.GetComponentInChildren<CharacterDirection>();
            self = this.transform.root.GetComponentInChildren<CharacterBody>();
            motor = this.transform.root.GetComponentInChildren<CharacterMotor>();

            // set this stuffs
            foreach (var thisItem in direction.modelAnimator.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                thisItem.gameObject.SetActive(false);
            }
            foreach (var thisItem in direction.modelAnimator.GetComponentsInChildren<MeshRenderer>())
            {
                thisItem.gameObject.SetActive(false);
            }
            self.crosshairPrefab = Resources.Load<GameObject>("prefabs/crosshair/simpledotcrosshair");
            miner.transform.position = direction.modelAnimator.transform.position;
            miner.transform.rotation = direction.modelAnimator.transform.rotation;
            miner.transform.SetParent(direction.modelAnimator.transform);
            direction.modelAnimator = miner.GetComponentInChildren<Animator>();

            // DISABLE ITEM DISPLAYING
            var model = self.GetComponentInChildren<CharacterModel>();
            if (model != null)
                model.itemDisplayRuleSet = null;

            minerSkins = miner.GetComponentsInChildren<MinerSkins>();
            ModelLocator modell = GetComponent<ModelLocator>();
            modellCont = modell.modelTransform.GetComponent<ModelSkinController>();
        }

        void Update()
        {
            int newdex = modellCont.currentSkinIndex;
            for (int i = 0; i < 3; i++)
            {
                minerSkins[i].index = skindex;
            }
            skindex = newdex;
        }
    }

    //internal class MyNetworkComponent : NetworkBehaviour
    //{
    //    // We only ever have one instance of the networked behaviour here.
    //    private static MyNetworkComponent _instance;

    //    private void Awake()
    //    {
    //        _instance = this;
    //    }
    //    public static void Invoke(NetworkUser user, string msg)
    //    {
    //        _instance.TargetLog(user.connectionToClient, msg);
    //    }

    //    // While we can't find the entirety of the Unity Script API in here, we can provide links to them.
    //    // This attribute is explained here: https://docs.unity3d.com/2017.3/Documentation/ScriptReference/Networking.TargetRpcAttribute.html
    //    [TargetRpc]
    //    //Note that the doc explictly says "These functions [-> Functions with the TargetRPC attribute] must begin with the prefix "Target" and cannot be static." 
    //    private void TargetLog(NetworkConnection target, string msg)
    //    {
    //        Debug.Log(msg);
    //    }
    //}
}