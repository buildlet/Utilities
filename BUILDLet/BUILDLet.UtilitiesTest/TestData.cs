using System;

namespace BUILDLet.Utilities
{
    public class TestData
    {
        public static readonly string DummyFileName_in_MyDocuments = "TESTFILE";

        public static readonly string WakeOnLan_ConfigFileName = "WOL.conf";
        
        public static string[] DummyMacAddresses =
        {
            "00-00-00-00-00-00",
            "FF-FF-FF-FF-FF-FF",
            "00:00:00:00:00:00",
            "FF:FF:FF:FF:FF:FF",
            "01:23:45:AB:CD:EF",
            "AB:CD:EF:01:23:45",
        };
    }
}
