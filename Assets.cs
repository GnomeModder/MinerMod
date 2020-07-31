using System;
using UnityEngine;

namespace Gnome
{
    public static class Assets
    {
        //public static AssetBundle MainAssetBundle = null;
        //public static AssetBundleResourcesProvider Provider;

        // FX
        //public static GameObject SephHitFx = null;

        // icons
        public static Texture2D minerIcon = LoadTexture2D(MinerMod.Properties.Resources.MinerIcon);
        public static Texture2D minerCrushIcon = LoadTexture2D(MinerMod.Properties.Resources.crushIcon);
        public static Texture2D minerDrillChargeIcon = LoadTexture2D(MinerMod.Properties.Resources.drillChargeIcon);
        public static Texture2D minerBackblastIcon = LoadTexture2D(MinerMod.Properties.Resources.backBlastIcon);
        public static Texture2D minerToTheStarsIcon = LoadTexture2D(MinerMod.Properties.Resources.toTheStarsIcon);
        public static Texture2D minerCrackHammerIcon = LoadTexture2D(MinerMod.Properties.Resources.hammerCrackIcon);
        public static Texture2D minerCaveInIcon = LoadTexture2D(MinerMod.Properties.Resources.caveInIcon);
        public static Texture2D minerTimedExplosiveIcon = LoadTexture2D(MinerMod.Properties.Resources.timedExplosiveIcon);

        public static Sprite minerCrushIconSprite = Sprite.Create(minerCrushIcon, new Rect(0.0f, 0.0f, minerCrushIcon.width, minerCrushIcon.height), new Vector2(0.5f, 0.5f));
        public static Sprite minerDrillChargeIconSprite = Sprite.Create(minerDrillChargeIcon, new Rect(0.0f, 0.0f, minerDrillChargeIcon.width, minerDrillChargeIcon.height), new Vector2(0.5f, 0.5f));
        public static Sprite minerBackblastIconSprite = Sprite.Create(minerBackblastIcon, new Rect(0.0f, 0.0f, minerBackblastIcon.width, minerBackblastIcon.height), new Vector2(0.5f, 0.5f));
        public static Sprite minerToTheStarsIconSprite = Sprite.Create(minerToTheStarsIcon, new Rect(0.0f, 0.0f, minerToTheStarsIcon.width, minerToTheStarsIcon.height), new Vector2(0.5f, 0.5f));
        public static Sprite minerCrackHammerIconSprite = Sprite.Create(minerCrackHammerIcon, new Rect(0.0f, 0.0f, minerCrackHammerIcon.width, minerCrackHammerIcon.height), new Vector2(0.5f, 0.5f));
        public static Sprite minerCaveInIconSprite = Sprite.Create(minerCaveInIcon, new Rect(0.0f, 0.0f, minerCaveInIcon.width, minerCaveInIcon.height), new Vector2(0.5f, 0.5f));
        public static Sprite minerTimedExplosiveIconSprite = Sprite.Create(minerTimedExplosiveIcon, new Rect(0.0f, 0.0f, minerTimedExplosiveIcon.width, minerTimedExplosiveIcon.height), new Vector2(0.5f, 0.5f));

        // zachicons
        public static Texture2D zachMinerGoldRushIcon = LoadTexture2D(MinerMod.Properties.Resources.zachGoldRush);
        public static Texture2D zachMinerCrushIcon = LoadTexture2D(MinerMod.Properties.Resources.zachCrush);
        public static Texture2D zachMinerDrillChargeIcon = LoadTexture2D(MinerMod.Properties.Resources.zachDrillCharge);
        public static Texture2D zachMinerBackblastIcon = LoadTexture2D(MinerMod.Properties.Resources.zachBackblast);
        public static Texture2D zachMinerToTheStarsIcon = LoadTexture2D(MinerMod.Properties.Resources.zachToTheStars);
        public static Texture2D zachMinerCrackHammerIcon = LoadTexture2D(MinerMod.Properties.Resources.zachCrackHammer);
        public static Texture2D zachMinerCaveInIcon = LoadTexture2D(MinerMod.Properties.Resources.zachCaveIn);
        public static Texture2D zachMinerTimedExplosiveIcon = LoadTexture2D(MinerMod.Properties.Resources.zachTimedExplosive);

        public static Sprite zachMinerGoldRushIconSprite = Sprite.Create(zachMinerGoldRushIcon, new Rect(0.0f, 0.0f, zachMinerGoldRushIcon.width, zachMinerGoldRushIcon.height), new Vector2(0.5f, 0.5f));
        public static Sprite zachMinerCrushIconSprite = Sprite.Create(zachMinerCrushIcon, new Rect(0.0f, 0.0f, zachMinerCrushIcon.width, zachMinerCrushIcon.height), new Vector2(0.5f, 0.5f));
        public static Sprite zachMinerDrillChargeIconSprite = Sprite.Create(zachMinerDrillChargeIcon, new Rect(0.0f, 0.0f, zachMinerDrillChargeIcon.width, zachMinerDrillChargeIcon.height), new Vector2(0.5f, 0.5f));
        public static Sprite zachMinerBackblastIconSprite = Sprite.Create(zachMinerBackblastIcon, new Rect(0.0f, 0.0f, zachMinerBackblastIcon.width, zachMinerBackblastIcon.height), new Vector2(0.5f, 0.5f));
        public static Sprite zachMinerToTheStarsIconSprite = Sprite.Create(zachMinerToTheStarsIcon, new Rect(0.0f, 0.0f, zachMinerToTheStarsIcon.width, zachMinerToTheStarsIcon.height), new Vector2(0.5f, 0.5f));
        public static Sprite zachMinerCrackHammerIconSprite = Sprite.Create(zachMinerCrackHammerIcon, new Rect(0.0f, 0.0f, zachMinerCrackHammerIcon.width, zachMinerCrackHammerIcon.height), new Vector2(0.5f, 0.5f));
        public static Sprite zachMinerCaveInIconSprite = Sprite.Create(zachMinerCaveInIcon, new Rect(0.0f, 0.0f, zachMinerCaveInIcon.width, zachMinerCaveInIcon.height), new Vector2(0.5f, 0.5f));
        public static Sprite zachMinerTimedExplosiveIconSprite = Sprite.Create(zachMinerTimedExplosiveIcon, new Rect(0.0f, 0.0f, zachMinerTimedExplosiveIcon.width, zachMinerTimedExplosiveIcon.height), new Vector2(0.5f, 0.5f));

        // skiniconds
        public static Texture2D defSkinIcon = LoadTexture2D(MinerMod.Properties.Resources.texMinerDefaultIcon);
        public static Texture2D masSkinIcon = LoadTexture2D(MinerMod.Properties.Resources.texMinerMasteryIcon);
        public static Texture2D pupSkinIcon = LoadTexture2D(MinerMod.Properties.Resources.texMinerSecretIcon);

        public static Sprite defSkinIconSprite = Sprite.Create(defSkinIcon, new Rect(0.0f, 0.0f, defSkinIcon.width, defSkinIcon.height), new Vector2(0.5f, 0.5f));
        public static Sprite masSkinIconSprite = Sprite.Create(masSkinIcon, new Rect(0.0f, 0.0f, masSkinIcon.width, masSkinIcon.height), new Vector2(0.5f, 0.5f));
        public static Sprite pupSkinIconSprite = Sprite.Create(pupSkinIcon, new Rect(0.0f, 0.0f, pupSkinIcon.width, pupSkinIcon.height), new Vector2(0.5f, 0.5f));

        public static AssetBundle minerAssetBundle = LoadAssetBundle(MinerMod.Properties.Resources.miner);

        public static UInt32 unloadingID = LoadSoundBank(MinerMod.Properties.Resources.MinerSounds);

        // The function code
        private static Texture2D LoadTexture2D(Byte[] resourceBytes)
        {
            //Check to make sure that the byte array supplied is not null, and throw an appropriate exception if they are.
            if (resourceBytes == null) throw new ArgumentNullException(nameof(resourceBytes));

            //Create a temporary texture, then load the texture onto it.
            var tempTex = new Texture2D(128, 128, TextureFormat.RGBAFloat, false);
            ImageConversion.LoadImage(tempTex, resourceBytes, false);

            return tempTex;
        }

        static AssetBundle LoadAssetBundle(Byte[] resourceBytes)
        {
            //Check to make sure that the byte array supplied is not null, and throw an appropriate exception if they are.
            if (resourceBytes == null) throw new ArgumentNullException(nameof(resourceBytes));

            //Actually load the bundle with a Unity function.
            var bundle = AssetBundle.LoadFromMemory(resourceBytes);

            return bundle;
        }

        static UInt32 LoadSoundBank(Byte[] resourceBytes)
        {
            //Check to make sure that the byte array supplied is not null, and throw an appropriate exception if they are.
            if (resourceBytes == null) throw new ArgumentNullException(nameof(resourceBytes));

            //Register the soundbank and return the ID
            return R2API.SoundAPI.SoundBanks.Add(resourceBytes);
        }
    }
}