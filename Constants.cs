using System;

namespace ChrisLajoie.DupSelection
{
    static class Constants
    {
        public const string VERSION = "2.0";

        public const string guidDupSelectionPkgString = "e5f7e157-f686-46b7-a588-85b08cdaa5f0";
        public const string guidDupSelectionCmdSetString = "85dcd5f2-19a5-4ee2-a99b-4fac4dc5c4ca";

        public static readonly Guid guidDupSelectionCmdSet = new Guid(guidDupSelectionCmdSetString);

        public const int cmdidDupSelection = 0x0100;
    }
}
