using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatefulAPI.Controllers;
using StatefulAPI.Models.EntityFramework;

namespace StatefulAPI.Tests
{
    [TestClass]
    public class SeriesControllerTests
    {
        private readonly List<Serie> expectedSeries = new List<Serie>
    {
        new Serie
        {
            Serieid = 1,
            Titre = "Scrubs",
            Resume = "J.D. est un jeune médecin qui débute sa carrière dans l'hôpital du Sacré-Coeur. Il vit avec son meilleur ami Turk, qui lui est chirurgien dans le même hôpital. Très vite, Turk tombe amoureux d'une infirmière Carla. Elliot entre dans la bande. C'est une étudiante en médecine quelque peu surprenante. Le service de médecine est dirigé par l'excentrique Docteur Cox alors que l'hôpital est géré par le diabolique Docteur Kelso. A cela viennent s'ajouter plein de personnages hors du commun : Todd le chirurgien obsédé, Ted l'avocat dépressif, le concierge qui trouve toujours un moyen d'embêter JD... Une belle galerie de personnage !",
            Nbsaisons = 9,
            Nbepisodes = 184,
            Anneecreation = 2001,
            Network = "ABC (US)"
        },

        new Serie
        {
            Serieid = 2,
            Titre = "James May's 20th Century",
            Resume = "The world in 1999 would have been unrecognisable to anyone from 1900. James May takes a look at some of the greatest developments of the 20th century, and reveals how they shaped the times we live in now.",
            Nbsaisons = 1,
            Nbepisodes = 6,
            Anneecreation = 2007,
            Network = "BBC Two"
        },

        new Serie
        {
            Serieid = 3,
            Titre = "True Blood",
            Resume = "Ayant trouvé un substitut pour se nourrir sans tuer (du sang synthétique), les vampires vivent désormais parmi les humains. Sookie, une serveuse capable de lire dans les esprits, tombe sous le charme de Bill, un mystérieux vampire. Une rencontre qui bouleverse la vie de la jeune femme...",
            Nbsaisons = 7,
            Nbepisodes = 81,
            Anneecreation = 2008,
            Network = "HBO"
        }
    };
        private void CompareSeries(Serie expected, Serie actual)
        {
            Assert.AreEqual(expected.Serieid, actual.Serieid);
            Assert.AreEqual(expected.Titre, actual.Titre);
            Assert.AreEqual(expected.Resume, actual.Resume);
            Assert.AreEqual(expected.Nbsaisons, actual.Nbsaisons);
            Assert.AreEqual(expected.Nbepisodes, actual.Nbepisodes);
            Assert.AreEqual(expected.Anneecreation, actual.Anneecreation);
            Assert.AreEqual(expected.Network, actual.Network);
        }
        private SeriesController controller;

        public SeriesControllerTests()
        {
            var builder = new DbContextOptionsBuilder<SeriesDbContext>()
                .UseNpgsql("Server=localhost;Port=5432;Database=SeriesDB;Username=postgres;Password=postgres;");

            SeriesDbContext context = new SeriesDbContext(builder.Options);

            controller = new SeriesController(context);
        }

        [TestMethod]
        public void TestGetSeries()
        {
            var result = controller.GetSerie().Result;

            var seriesRetrieved = result.Value
                .Where(s => s.Serieid <= 3)
                .ToList();

            Assert.AreEqual(expectedSeries.Count, seriesRetrieved.Count);

            for (int i = 0; i < expectedSeries.Count; i++)
            {
                CompareSeries(expectedSeries[i], seriesRetrieved[i]);
            }
        }

        [TestMethod]
        public void TestGetSeriesByID()
        {
            var result = controller.GetSerie(1).Result;

            CompareSeries(expectedSeries[0], result.Value);
        }


        [TestMethod]
        public void TestGetSeriesByIDNotFound()
        {
            var result = controller.GetSerie(222222222).Result;

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }


        [TestMethod]
        public void TestPostSeries()
        {
            var serie = new Serie
            {
                Titre = "Just a test",
                Resume = "test",
                Nbsaisons = 1,
                Nbepisodes = 4,
                Anneecreation = 2026,
                Network = "Test network"
            };

            var result = controller.PostSerie(serie).Result;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result);

            var createdResult = (CreatedAtActionResult)result.Result;

            Assert.AreEqual(StatusCodes.Status201Created, createdResult.StatusCode);

            var serieCreated = (Serie)createdResult.Value;

            Assert.IsNotNull(serieCreated);

            serie.Serieid = serieCreated.Serieid;

            CompareSeries(serie, serieCreated);

            controller.DeleteSerie(serieCreated.Serieid).Wait();
        }

        [TestMethod]
        public void TestPostSeriesWithoutTitle()
        {
            var serie = new Serie
            {
                Resume = "test",
                Nbsaisons = 1,
                Nbepisodes = 4,
                Anneecreation = 2026,
                Network = "Test network"
            };

            Assert.ThrowsExactly<AggregateException>(
                () => controller.PostSerie(serie).Result
            );
        }

        [TestMethod]
        public void TestPutSeries()
        {
            var serie = new Serie
            {
                Titre = "Just a test",
                Resume = "test",
                Nbsaisons = 1,
                Nbepisodes = 4,
                Anneecreation = 2026,
                Network = "Test network"
            };

            var result = controller.PostSerie(serie).Result;

            var createdResult = (CreatedAtActionResult)result.Result;
            var serieCreated = (Serie)createdResult.Value;


            serieCreated.Titre = "Just a test (updated)";

            var PutResult = controller.PutSerie(serieCreated.Serieid, serieCreated).Result;


            Assert.IsInstanceOfType(PutResult, typeof(NoContentResult));


            var getResult = controller.GetSerie(serieCreated.Serieid).Result;
            var updatedSerie = getResult.Value;

            Assert.IsNotNull(updatedSerie);
            Assert.AreEqual("Just a test (updated)", updatedSerie.Titre);

            controller.DeleteSerie(serieCreated.Serieid).Wait();

        }


        [TestMethod]
        public void TestPutSeriesBadRequest()
        {
            var serie = new Serie
            {
                Serieid = 1,
                Titre = "Test",
                Resume = "Test",
                Nbsaisons = 1,
                Nbepisodes = 1,
                Anneecreation = 2026,
                Network = "Test"
            };

            var result = controller.PutSerie(987654321, serie).Result;

            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }


        [TestMethod]
        public void TestDeleteSeries()
        {
            var serie = new Serie
            {
                Titre = "DELETE test",
                Resume = "Test",
                Nbsaisons = 1,
                Nbepisodes = 1,
                Anneecreation = 2026,
                Network = "Test"
            };

            var postResult = controller.PostSerie(serie).Result;
            var createdResult = (CreatedAtActionResult)postResult.Result;
            var createdSerie = (Serie)createdResult.Value;

            var result = controller.DeleteSerie(createdSerie.Serieid).Result;

            Assert.IsInstanceOfType(result, typeof(NoContentResult));

            var deletionResult = controller.GetSerie(createdSerie.Serieid).Result;

            Assert.IsInstanceOfType(deletionResult.Result, typeof(NotFoundResult));
        }


        [TestMethod]
        public void TestDeleteSeriesNotFound()
        {
            var result = controller.DeleteSerie(987654321).Result;

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }



}