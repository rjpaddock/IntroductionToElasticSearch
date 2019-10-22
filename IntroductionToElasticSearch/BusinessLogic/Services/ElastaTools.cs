using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using CsvHelper;
using Elasticsearch.Net;
using IntroductionToElasticSearch.BusinessLogic.Domain;
using IntroductionToElasticSearch.BusinessLogic.ExtensionMethods;
using IntroductionToElasticSearch.ViewModels;
using Nest;

namespace IntroductionToElasticSearch.BusinessLogic.Services
{
    public class ElastaTools
    {
        public static ElasticClient GetElasticClient(string indexName)
        {
            var url  ="http://localhost:9200";

            var node = new Uri(url);
            var settings = new ConnectionSettings(node);

            settings.DefaultIndex(indexName);

            var client = new ElasticClient(settings);
            return client;
        }


        public static void LoadCollectionInfoIndex<T>(List<T> dataToProcess, ElasticClient client, string indexName) where T : class
        {
            //clear the index before load
            var deleteResult = client.DeleteByQuery<T>(del => del
                .Query(q => q.QueryString(qs => qs.Query("*"))).Index(indexName));


            var waitHandle = new CountdownEvent(1);
            var bulkAll = client.BulkAll(dataToProcess, b => b
                .Index(indexName)
                .BackOffRetries(2)
                .BackOffTime("30s")
                .RefreshOnCompleted(true)
                .MaxDegreeOfParallelism(4)
                .Size(10000));

            bulkAll.Subscribe(new BulkAllObserver(
                onNext: (b) => { Console.Write("."); },
                onError: (err) =>
                {
                    var x = err;
                    throw err;
                },
                onCompleted: () => waitHandle.Signal()
            ));
            waitHandle.Wait();
        }


        public static SearchRequest<ImDbBasicTitle> BuildSearchRequest(MovieSearchViewModel vm)
        {

            //1. Container for multiple query filters
            var mustClauses = new List<QueryContainer>();

            //scrub the data
            var criteria = vm.SearchCriteria;
            if (criteria == null)
            {
                criteria = "";
            }
          
            //make sure AND & OR are upper for query engine
            criteria = criteria.Replace(" and ", " AND ");
            criteria = criteria.Replace(" or ", " OR ");

            //limit search fields
            var fields = new List<Nest.Field>();
            if (vm.OnlyOriginalTitle)
            {
                fields.Add(new Nest.Field("originalTitle"));
            }

            //used for
            var queryStringFilter = new QueryStringQuery()
            {
                Query = criteria,   //"toy or book"
                Fields = fields.ToArray()
            };

            //ONLY movies
            var titleTypeFilter = new MatchQuery()
            {
                Field = "titleType",
                Query = "movie"
            };



            //single record query
            if (vm.DocumentId  != null && vm.DocumentId.Length > 0)
            {

                //By Id
                var idFilter = new MatchQuery()
                {
                    Field = "id",
                    Query = vm.DocumentId
                };
                //ONLY ID
                mustClauses.Add(idFilter);

                
            }
            else
            {
                //add google style query
                mustClauses.Add(queryStringFilter);

                //only movies filter
                if (vm.OnlyMovies)
                {
                    mustClauses.Add(titleTypeFilter);
                }


            }


            //sort fields
            var sortFields = new List<ISort>();
            sortFields.Add(new FieldSort()
            {
                Field = "sortYear",
                Order = SortOrder.Ascending,
                UnmappedType = FieldType.Integer,

            });
            
            
            var request = new SearchRequest<ImDbBasicTitle>()
            {
                From = 0,
                Size = 50,  // 10000,
                Sort =  sortFields,
                Query = new BoolQuery { Must = mustClauses } //,

            };

            return request;
        }

        public static ISearchResponse<ImDbBasicTitle> SearchMovies(ElasticClient  client, string index , SearchRequest<ImDbBasicTitle> request)
        {
            var response = client.Search<ImDbBasicTitle>(request);
            return response;
        }

        public static List<ImDbBasicTitle> GetImdbMovieListings()
        {
            var reader =
                new StreamReader(@"D:\Data\clients\DashPoint\ConferenceSessions\MovieSampleData\title.basics.data.tsv");
            var csv = new CsvReader(reader);
            csv.Configuration.HasHeaderRecord = true;
            csv.Configuration.Delimiter = "\t";
            csv.Configuration.BadDataFound = null;
            csv.Configuration.MissingFieldFound = null;
            var retval = new List<ImDbBasicTitle>();
            csv.Read();
            var maxKount = 2000000;
            var kount = 0;
            while (csv.Read())
            {
                ++kount;
                var newRec = new ImDbBasicTitle()
                {
                    Id = csv.GetField(0).CleanImdbData(),
                    TitleType = csv.GetField(1).CleanImdbData(),
                    PrimaryTitle = csv.GetField(2).CleanImdbData(),
                    OriginalTitle = csv.GetField(3).CleanImdbData(),
                    IsAdult = csv.GetField(4).CleanImdbData(),
                    StartYear = csv.GetField(5).CleanImdbData(),
                    EndYear = csv.GetField(6).CleanImdbData(),
                    RuntimeMinutes = csv.GetField(7).CleanImdbData(),
                    Genres = csv.GetField(8).CleanImdbData().Split(','),

                };

                if (newRec.Id == "tt0350945")
                {
                    var x = 1;
                }

                if (newRec.IsAdult == "0" && csv.GetField(2).CleanImdbData().ToLower().IndexOf("porn") < 0 && csv.GetField(8).ToLower().IndexOf("adult") < 0  && newRec.StartYear.Length > 0)
                {
                    retval.Add(newRec);

                }

                ++kount;
                if (kount % 10000 == 0)
                {
                    Console.WriteLine(kount.ToString());
                }

                if (retval.Count >= maxKount)
                {
                    break;
                }

            }


            csv.Dispose();
            reader.Dispose();
            return retval;
        }

        public static void LoadDirectorsTest()
        {
            var client = ElastaTools.GetElasticClient("prarie_devcon_2019_directors");

            var clients = new List<Director>();
            clients.Add(new Director()
            {
                Name = "Steven Spielberg"
            });
            clients[clients.Count - 1].Movies.Add(new Movie()
            {
                DirectorId = clients[clients.Count - 1].Id,
                Name = "Jaws"
            });

            clients[clients.Count - 1].Movies.Add(new Movie()
            {
                DirectorId = clients[clients.Count - 1].Id,
                Name = "1941"
            });

            clients.Add(new Director()
            {
                Name = "George Lucas"
            });
            clients[clients.Count - 1].Movies.Add(new Movie()
            {
                DirectorId = clients[clients.Count - 1].Id,
                Name = "American Graffiti"
            });
            clients[clients.Count - 1].Movies.Add(new Movie()
            {
                DirectorId = clients[clients.Count - 1].Id,
                Name = "Star Wars"
            });
            clients[clients.Count - 1].Movies.Add(new Movie()
            {
                DirectorId = clients[clients.Count - 1].Id,
                Name = "THX 1138"
            });

            foreach (var doc in clients)
            {
                var resp = client.Index(doc, idx => idx.Index("prarie_devcon_2019_directors"));
            }
        }

    }
}