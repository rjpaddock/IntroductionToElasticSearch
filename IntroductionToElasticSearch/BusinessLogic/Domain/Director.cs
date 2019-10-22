using System.Collections.Generic;
using IntroductionToElasticSearch.Controllers;

namespace IntroductionToElasticSearch.BusinessLogic.Domain
{
    public class Director
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public List<Movie> Movies { get; set; }

        public Director()
        {
            Id = System.Guid.NewGuid().ToString();
            Movies = new List<Movie>();
            Address = "Address" + System.Guid.NewGuid().ToString();

        }
    }
}