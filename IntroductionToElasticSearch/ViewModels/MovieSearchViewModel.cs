using System.Collections.Generic;
using IntroductionToElasticSearch.BusinessLogic.Domain;
using Nest;

namespace IntroductionToElasticSearch.ViewModels
{
    public class MovieSearchViewModel
    {
        public string SearchCriteria { get; set; }
        public List<ImDbBasicTitle> FoundMovies { get; set; }
        public ISearchResponse<ImDbBasicTitle> QueryResults {get; set; }
        public string QueryJson { get; set; }
        public bool OnlyMovies { get; set; }
        public string DocumentId { get; set; }
        public bool OnlyOriginalTitle { get; set; }

        public MovieSearchViewModel()
        {
            SearchCriteria = "";
            FoundMovies = new List<ImDbBasicTitle>();
            QueryResults = null;
            QueryJson = "";
            DocumentId = "";
            OnlyMovies = false;
            OnlyOriginalTitle = false;
        }
    }
}