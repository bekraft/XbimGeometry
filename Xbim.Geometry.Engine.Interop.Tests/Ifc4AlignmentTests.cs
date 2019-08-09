using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Interfaces;
using Microsoft.Extensions.Logging;
using Xbim.IO.Memory;

namespace Xbim.Geometry.Engine.Interop.Tests
{
    [TestClass]
    public class Ifc4AlignmentTests
    {
        static private IXbimGeometryEngine geomEngine;
        static private ILoggerFactory loggerFactory;
        static private ILogger logger;

        [ClassInitialize]
        static public void Initialise(TestContext context)
        {
            loggerFactory = new LoggerFactory().AddConsole(LogLevel.Trace);
            geomEngine = new XbimGeometryEngine();
            logger = loggerFactory.CreateLogger<Ifc4AlignmentTests>();
        }

        [ClassCleanup]
        static public void Cleanup()
        {
            loggerFactory = null;
            geomEngine = null;
            logger = null;
        }

        [TestMethod]
        public void BasicIfcAlignment2DHorizontal()
        {
            using (var model = MemoryModel.OpenRead(@".\TestFiles\Ifc4AlignmentFiles\horizontal-alignment-without-transition-curve.ifc"))
            {
                var hAlignment = model.Instances.OfType<IIfcAlignment2DHorizontal>().FirstOrDefault();
                Assert.IsTrue(hAlignment != null, "No IIfcAlignment2DHorizontal found");
                var hFace = geomEngine.CreateFace(hAlignment, logger);

                Assert.AreEqual(1396.495506503687, hFace.Perimeter, 1e-5);
                Assert.AreEqual(3, hFace.OuterBound.Edges.Count);
                Assert.AreEqual(4, hFace.OuterBound.Vertices.Count);
            }
        }


    }
}
