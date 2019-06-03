using System.Globalization;
using Nest;

namespace IntroductionToElasticSearch.BusinessLogic.Domain
{
    public class ImDbBasicTitle
    {
        public string Id { get; set; }
        public string TitleType { get; set; }
        public string PrimaryTitle { get; set; }
        [Text]
        public string OriginalTitle { get; set; }

        public string IsAdult { get; set; }
        public string StartYear { get; set; }
        public string EndYear { get; set; }
        public string RuntimeMinutes { get; set; }
        public string[] Genres { get; set; }
        [Number]
        public int SortYear
        {
            get
            {
                var retval = 0;
                if (StartYear.Length > 0)
                {
                    retval = System.Convert.ToInt32(StartYear);
                }

                return retval;
            }
        }

        
    }
}