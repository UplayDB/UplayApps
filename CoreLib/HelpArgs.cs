namespace CoreLib
{
    public class HelpArgs
    {
        public static void PrintHelp()
        {
            Console.WriteLine();
            Console.WriteLine("\t\tCoreLib Helper");
            Console.WriteLine();
            Console.WriteLine("\t Standard Arguments\t Arguments Description");
            Console.WriteLine();
            Console.WriteLine("\t -b64\t\t\t Login with providen Base64 of email and password");
            Console.WriteLine("\t -username\t\t Using that email to login");
            Console.WriteLine("\t -user\t\t\t Using that email to login (short version)");
            Console.WriteLine("\t -password\t\t Using that password to login");
            Console.WriteLine("\t -pass\t\t\t Using that password to login (short version)");
            Console.WriteLine("\t -trustedid\t\t Login with that trusted Id, if not present, make with trustedname");
            Console.WriteLine("\t -trustedname\t\t Name of trusted device, if not given name, using MachineName (PC name)");
            Console.WriteLine("\t -remember-me\t\t Using the user to remember and use that information next time on logon");
            Console.WriteLine("\t -remember-device\t Using trusted device to save into (use with remember-me for after long way login)");
            Console.WriteLine("\t -accid\t\t\t Another way to use combining remember-me and this (no need for username)");
            Console.WriteLine("\t -usefilestore\t\t\t Use normal file for storing credentials");
            Console.WriteLine("\t -printstore\t\t\t Printing the credentails stored. (and its place)");
            Console.WriteLine();
            WritePadder();
        }

        public static void WritePadder()
        {
            string padder = "";
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                padder += "=";
            }
            Console.WriteLine(padder);
        }
    }
}
