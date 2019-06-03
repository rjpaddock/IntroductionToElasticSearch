using System;
using System.Linq;

namespace IntroductionToElasticSearch.BusinessLogic.ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static string ArrayToString(this string[] str)
        {
            var retval = "";
            retval = String.Join(",",str);
            return retval;
        }
        
            public static string CleanImdbData(this string str)
        {

            
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }

            if (str.Trim().ToUpper() == @"\N")
            {
                return "";
            }

            return str;

        }
    }
}