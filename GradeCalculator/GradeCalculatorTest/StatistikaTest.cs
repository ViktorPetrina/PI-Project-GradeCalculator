using GradeCalculator.Adapter;
using GradeCalculator.Controllers;
using GradeCalculator.Models;
using GradeCalculator.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeCalculatorTest
{
    public class StatistikaTest
    {

        [Fact]
        public void Statistika_Shown()
        {
            
            var mockStatistika = new Mock<IUkupnaStatistika>();
            var mockContext = new Mock<PiGradeCalculatorContext>();
            var mockKorisnikService = new Mock<IKorisnikService>();
            var mockKorisnikAdapter = new Mock<IKorisnikAdapter>();

          
            var ocjene = new Dictionary<int, double>
            {{ 0, 20 },{ 1, 30 },{ 2, 10 },{ 3, 25 },{ 4, 15 }};
            mockStatistika
                .Setup(s => s.KalkulacijaUkupnihProsjeka())
                .Returns(ocjene);


            var controller = new KorisnikController(
                mockContext.Object,
                mockStatistika.Object,
                mockKorisnikService.Object,
                mockKorisnikAdapter.Object);


            var result = controller.GetDataPoints() as JsonResult;

            Assert.NotNull(result); 
            var dataPoints = Assert.IsType<List<DataPoint>>(result.Value); 

            Assert.Equal(6, dataPoints.Count); 
            Assert.Equal("0", dataPoints[0].Label);
            Assert.Equal(20, dataPoints[0].Y);
            Assert.Equal("4", dataPoints[4].Label);
            Assert.Equal(15, dataPoints[4].Y);

            mockStatistika.Verify(s => s.KalkulacijaUkupnihProsjeka(), Times.Once);

        }


        [Fact]
        public void Statistika_Prosjek_Shown()
        {
            var mockStatistikaService = new Mock<IStatistikaService>();
            double expectedAverage = 4.5; 
            mockStatistikaService.Setup(s => s.KalkulacijaProsjeka()).Returns(expectedAverage);

            var controller = new PredmetController(
                null,null,null,null,null,
                mockStatistikaService.Object,
                null);
            var result = controller.UkupniProsjek();

            Assert.NotNull(result);
            Assert.Equal(expectedAverage, result.Value);


            mockStatistikaService.Verify(s => s.KalkulacijaProsjeka(), Times.Once);

        }
    }
}

