using System;

namespace Gnome
{
    class StaticValues
    {
        public static String MINER_DESCRIPTION = "The Miner is a fast paced and highly mobile melee survivor who prioritizes getting long kill combos to build stacks of his passive" +
                "\n " + "\n <!> Once you get a good number of stacks of Adrenaline, Crush will be your best source of damage" +
                "\n " + "\n <!> Note that charging Drill Charge only affects damage dealt. Aim at the ground or into enemies to deal concentrated damage" +
                "\n " + "\n <!> You can tap Backblast to travel a short distance. Hold it to go further" +
                "\n " + "\n <!> To The Stars when used low to the ground is great at dealing high amounts of damage to enemies with large hitboxes";

        public static String MINER_PRIMARY_CRUSH_NAME = "Crush";
        public static String MINER_PRIMARY_CRUSH_DESCRIPTION = "Crush nearby enemies for 180% damage.";

        public static String MINER_SECONDARY_DRILLCHARGE_NAME = "Drill Charge";
        public static String MINER_SECONDARY_DRILLCHARGE_DESCRIPTION = "Charge for 1 second. Dash into enemies for up to 12x140% damage. You cannot be hit during and following the dash.";

        public static String MINER_UTILITY_BACKBLAST_NAME = "Backblast";
        public static String MINER_UTILITY_BACKBLAST_DESCRIPTION = "Blast backwards a variable distance for 500% damage, stunning all enemies in a large radius. You cannot be hit while dashing.";

        public static String MINER_SPECIAL_TOTHESTARS_NAME = "To The Stars";
        public static String MINER_SPECIAL_TOTHESTARS_DESCRIPTION = "Jump into the air, shooting a spray of projectiles downwards for 39x100% damage total.";

        public static String MINER_PASSIVE_GOLDRUSH_NAME = "Gold Rush";
        public static String MINER_PASSIVE_GOLDRUSH_DESCRIPTION = "Gain ADRENALINE on receiving gold, increasing attack speed, movement speed, and health regen. Any increase in gold refreshes all stacks.";

        public static String MINER_SECONDARY_CRACKHAMMER_NAME = "Crack Hammer";
        public static String MINER_SECONDARY_CRACKHAMMER_DESCRIPTION = "Dash forward, exploding for 400% damage on contact with an enemy. You cannot be hit during and following the dash.";

        public static String MINER_UTILITY_CAVEIN_NAME = "Cave In";
        public static String MINER_UTILITY_CAVEIN_DESCRIPTION = "Blast backwards a short distance, stunning all enemies in a large radius a pulling them together";

        public static String MINER_SPECIAL_TIMEDEXPLOSIVE_NAME = "Timed Explosive";
        public static String MINER_SPECIAL_TIMEDEXPLOSIVE_DESCRIPTION = "Toss a charge of C4. Tossing a new explosive resets all timers. More explosives placed means more damage per charge";

        public static float lingeringInvincibilityDuration = 1f;
        public static int goldRushCost = 5;
        public static int goldRushDuration = 5;
        public static float goldRushAttackBuffValue = 0.12f;
        public static float goldRushHealBuffValue = 1f;

        public static String MINER_LORE = "\nAmong the cold steel of the freighter's ruins and the air of the freezing night, the dim blue light of an audio journal catches your eye. The journal lies in utter disrepair among the remains of the cargo, and yet it persists, remaining functional. The analog buttons on the device's casing pulse slowly, as if inviting anybody to listen to their warped entries, ever-decaying in coherency..." +
                "\n\n<color=#8990A7><CLICK></color>" +
                "\n\n<color=#8990A7>/ / - - L  O  G     1 - - / /</color>" +
                "\n''First log. Date, uhhh... Oh-five, oh-one, twenty-fifty-five. Brass decided to pull us out of the mining job. After months of digging, after months of blood, swe<color=#8990A7>-///////-</color>nd millions spent on this operation, we're just dropping it? Bull<color=#8990A7>-//-</color>.''" +
                "\n\n<color=#8990A7>/ / - - L  O  G     5 - - / /</color>" +
                "\n''<color=#8990A7>-////////-</color>nter of the asteroid. I've never seen anything like it. It was like a dr<color=#8990A7>-////////-</color>nd fractal... The inside is almost like a kaleidoscope. It's gotta be worth<color=#8990A7>-/////-</color>nd brass wants us gone, ASAP. Well, they can kiss m<color=#8990A7>-////-</color>. And I'm going out there tonight to dig the rest of it out... If I can fence this, the next thousand paychecks won't hold a candle. I've been waiting for an opportunity like this.''" +
                "\n\n<color=#8990A7>/ / - - L  O  G     7 - - / /</color>" +
                "\n''Pretty sure they know everything. They're questioning people now. I stashed the artifact in a vent in<color=#8990A7>-//////-</color>, should be safe. Freighter called the UE<color=#8990A7>-////-</color>t's docking in a few months. I just have to hold out and sneak on.''" +
                "\n\n<color=#8990A7>/ / - - L  O  G     15 - - / /</color>" +
                "\n''They're looking for me. Not long before they find me. Hiding in the walls of the ship like a f<color=#8990A7>-//////-</color>rat. Freighter's off schedule. I gave everything for this. I gave <i>everything</i> for this.''" +
                "\n\n<color=#8990A7>/ / - - L  O  G     16 - - / /</color>" +
                "\n''Somewhere in the hull. Nobody comes back here. Not even maintenance. I should be safe until the ship arrives.''" +
                "\n\n<color=#8990A7>/ / - - L  O  G     18 - - / /</color>" +
                "\n''Ship still on hold. Can't get an idea of when it won't be.'' <color=#8990A7><Groan>.</color> ''This is ago<color=#8990A7>-//-</color>zing.''" +
                "\n\n <color=#8990A7>/ / - - L  O  G     29 - - / /</color>" +
                "\n''<color=#8990A7>-//////////-</color>'s here. Finally. Finally. Have to get the artifact on board. Security everywhere, though. Mercs...'' <color=#8990A7><Distant voices have an indiscernible conversation.></color> ''Okay. Shut up. Shut up and go. I'm going. Going. Tonight.''" +
                "\n\n <color=#8990A7>/ / - - L  O  G     31 - - / /</color>" +
                "\n''<color=#8990A7>-//////-</color>omething's happening to the ship. Cargo flying out. Lost the artifact. Lost everything. . . . Lost everythi<color=#8990A7>-//-</color>.''" +
                "\n\nThe audio journal's screen sparks and pops, leaving you in complete darkness, complemented by the deafening silence brought about by the ominous last words of the miner.";
    }
}
