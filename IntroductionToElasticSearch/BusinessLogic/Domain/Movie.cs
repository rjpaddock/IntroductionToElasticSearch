namespace IntroductionToElasticSearch.BusinessLogic.Domain
{
    public class Movie
    {
        public string Id { get; set; }
        public string DirectorId { get; set; }
        public string Name { get; set; }

        public Movie()
        {
            Id = System.Guid.NewGuid().ToString();
        }
    }
}