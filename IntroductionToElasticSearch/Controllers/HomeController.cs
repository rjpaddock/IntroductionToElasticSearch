﻿using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime;
using CsvHelper;
using Elasticsearch.Net;
using IntroductionToElasticSearch.BusinessLogic.Domain;
using IntroductionToElasticSearch.BusinessLogic.ExtensionMethods;
using IntroductionToElasticSearch.BusinessLogic.Services;
using IntroductionToElasticSearch.ViewModels;
using Nest;

namespace IntroductionToElasticSearch.Controllers
{
   

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var vm = new MovieSearchViewModel();
        
            return View(vm);
        }

        private string GetDemoIndexName()
        {
            return "prarie_devcon_2019_movies";
        }

        [HttpPost]
        public ActionResult Index(MovieSearchViewModel vm)
        {

            var client = ElastaTools.GetElasticClient(GetDemoIndexName());
            var searchRequest = ElastaTools.BuildSearchRequest(vm);
            vm.QueryResults = ElastaTools.SearchMovies(client, GetDemoIndexName(), searchRequest);
            vm.FoundMovies = vm.QueryResults.Documents.ToList();
            vm.QueryJson = client.RequestResponseSerializer.SerializeToString(searchRequest);
            

            return View(vm);
        }

        public ActionResult TestLoad()
        {
            //   ElastaTools.LoadDirectorsTest();

            var movies = ElastaTools.GetImdbMovieListings();
            ElastaTools.LoadCollectionInfoIndex(movies, ElastaTools.GetElasticClient(GetDemoIndexName()),
                GetDemoIndexName());

            return Content("Data has been loaded");
        }
        


        
        
        

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}