namespace CoreCommandLine
{
    internal class ArgumentProcess
    {
        
        public static int IndexOf(string name, string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (name.ToLower() == args[i].ToLower())
                {
                    return i;
                }
            }

            return -1;
        }

        public static int IndexOf(NameBundle nameBundle, string[] args)
        {
            int index = IndexOf(nameBundle.Name, args);

            if (index < 0)
            {
                index = IndexOf(nameBundle.ShortName, args);
            }

            return index;
        }


        public static bool AreNamesEqual(NameBundle commandName, string name)
        {
            return AreNamesEqual(commandName.Name, name) ||
                   AreNamesEqual(commandName.ShortName, name);
        }

        public static bool AreNamesEqual(string? name1, string? name2)
        {
            if (name1 == null && name2 == null)
            {
                return true;
            }

            if (name1 == null || name2 == null)
            {
                return false;
            }

            return name1.ToLower() == name2.ToLower();
        }
    }
}