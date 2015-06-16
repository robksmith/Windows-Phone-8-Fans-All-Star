
#region Usings

using Zengo.WP8.FAS.Models;
using Zengo.WP8.FAS.Resources;

#endregion

namespace Zengo.WP8.FAS.Helpers
{
    public static class PitchConstants
    {
        // Zone Group names
        public static readonly string ZoneKeeper = "Keeper";
        public static readonly string ZoneDefence = "Defence";
        public static readonly string ZoneMidfield = "Midfield";
        public static readonly string ZoneForward = "Forward";
        public static readonly string ZoneManager = "Manager";

        public static readonly int ZoneLeftBackUuid = 6;
        public static readonly int ZoneRightBackUuid = 7;

        // Position Key names
        public static readonly string PositionKeyKeeper = "GK";

        public static readonly string PositionKeyLeftBack = "LB";
        public static readonly string PositionKeyLeftCenterBack = "LCB";
        public static readonly string PositionKeyRightCenterBack = "RCB";
        public static readonly string PositionKeyRightBack = "RB";

        public static readonly string PositionKeyLeftMidfield = "LM";
        public static readonly string PositionKeyLeftCenterMidfield = "LCM";
        public static readonly string PositionKeyRightCenterMidfield = "RCM";
        public static readonly string PositionKeyRightMidfield = "RM";

        public static readonly string PositionKeyLeftForward = "LF";
        public static readonly string PositionKeyRightForward = "RF";

        public static readonly string PositionKeySubKeeper = "SK";
        public static readonly string PositionKeySubDefence = "SD";
        public static readonly string PositionKeySubMidfield = "SM";
        public static readonly string PositionKeySubForward = "SF";

        public static readonly string PositionKeyManager = "MG";

        internal static int PositionIdFromKey(string positionKey, bool isEurope)
        {
            PositionRecord position = App.ViewModel.DbViewModel.Position(positionKey, isEurope);
            if (position != null)
            {
                return position.PositionId;
            }
            return 0;
        }


        /// <summary>
        /// The names shipped in the database are not used - we look up different version here
        /// </summary>
        public static string KeyToName(string positionKey)
        {
            if (positionKey == PositionKeyKeeper)
            {
                return AppResources.GoalKeeperName;
            }

            else if (positionKey == PositionKeyLeftBack)
            {
                return AppResources.LeftBackName;
            }
            else if (positionKey == PositionKeyLeftCenterBack)
            {
                return AppResources.CentreBackName;
            }
            else if (positionKey == PositionKeyRightCenterBack)
            {
                return AppResources.CentreBackName;
            }
            else if (positionKey == PositionKeyRightBack)
            {
                return AppResources.RightBackName;
            }

            else if (positionKey == PositionKeyLeftMidfield)
            {
                return AppResources.MidfieldName;
            }
            else if (positionKey == PositionKeyLeftCenterMidfield)
            {
                return AppResources.MidfieldName;
            }
            else if (positionKey == PositionKeyRightCenterMidfield)
            {
                return AppResources.MidfieldName;
            }
            else if (positionKey == PositionKeyRightMidfield)
            {
                return AppResources.MidfieldName;
            }

            else if (positionKey == PositionKeyLeftForward)
            {
                return AppResources.ForwardName;
            }
            else if (positionKey == PositionKeyRightForward)
            {
                return AppResources.ForwardName;
            }

            else if (positionKey == PositionKeySubKeeper)
            {
                return AppResources.SubstituteKeeperName;
            }
            else if (positionKey == PositionKeySubDefence)
            {
                return AppResources.SubstituteDefenderName;
            }
            else if (positionKey == PositionKeySubMidfield)
            {
                return AppResources.SubstituteMidfielderName;
            }
            else if (positionKey == PositionKeySubForward)
            {
                return AppResources.SubstituteForwardName;
            }

            return string.Empty;
        }

    }
}
