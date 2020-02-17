﻿using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Ifc4.Interfaces;
using Xbim.IO.Memory;

namespace Xbim.Geometry.Engine.Interop.Tests
{
    [TestClass]
    // [DeploymentItem("TestFiles")]
    public class PrimitiveGeometryTests
    {
        static private IXbimGeometryEngine geomEngine;
        static private ILoggerFactory loggerFactory;
        static private ILogger logger;

        [ClassInitialize]
        static public void Initialise(TestContext context)
        {
            loggerFactory = new LoggerFactory().AddConsole(LogLevel.Trace);
            geomEngine = new XbimGeometryEngine();
            logger = loggerFactory.CreateLogger<Ifc4GeometryTests>();
        }
        [ClassCleanup]
        static public void Cleanup()
        {
            loggerFactory = null;
            geomEngine = null;
            logger = null;
        }

        //[TestMethod]
        //public void can_build_composite_curve()
        //{
        //    using (var model = MemoryModel.OpenRead(@".\\TestFiles\Primitives\\composite_curve.ifc"))
        //    {
        //        var compCurve = model.Instances.OfType<IIfcCompositeCurve>().FirstOrDefault();
        //        Assert.IsNotNull(compCurve);
        //        var face = geomEngine.CreateFace(compCurve);
        //        Assert.IsNotNull(face.OuterBound);
        //    }
        //}
        [TestMethod]
        public void can_build_ifc_faceted_brep()
        {
            using (var model = MemoryModel.OpenRead(@"TestFiles\Primitives\ifc_faceted_brep.ifc"))
            {
                var shape = model.Instances.OfType<IIfcFacetedBrep>().FirstOrDefault();
                Assert.IsNotNull(shape);
                var geom = geomEngine.CreateSolidSet(shape).FirstOrDefault();
                Assert.IsTrue(geom.Volume > 1e-5);
            }
        }
        [TestMethod]
        public void can_build_closed_shell()
        {
            using (var model = MemoryModel.OpenRead(@"TestFiles\Primitives\faulty_closed_shell.ifc"))
            {
                var shape = model.Instances.OfType<IIfcClosedShell>().FirstOrDefault();
                Assert.IsNotNull(shape);
                var geom = geomEngine.CreateSolidSet(shape).FirstOrDefault();
                Assert.IsTrue(geom.Volume > 1e-5);
            }
        }
        [TestMethod]
        public void can_build_poorly_aligned_planar_faces()
        {
            using (var model = MemoryModel.OpenRead(@"TestFiles\Primitives\poor_face_planar_fidelity.ifc"))
            {
                var shape = model.Instances.OfType<IIfcClosedShell>().FirstOrDefault();
                Assert.IsNotNull(shape);
                var geom = geomEngine.CreateSolidSet(shape).FirstOrDefault();
                Assert.IsNull(geom, "This should fail");
            }
        }

        [TestMethod]
        public void can_trim_composite_curve()
        {
            using (var er = new EntityRepository<IIfcSurfaceCurveSweptAreaSolid>("failing_swept_area_curve", inRadians: true))
            {
                Assert.IsTrue(er.Entity != null, "No IfcSurfaceCurveSweptAreaSolid found");
                var workAroundTag = er.Entity.Model.AddWorkAroundTrimForPolylinesIncorrectlySetToOneForEntireCurve();
                var solid = geomEngine.CreateSolidSet(er.Entity, logger);
                Assert.AreEqual(1, solid.Count);
                Assert.AreEqual(12.6,solid.First().Volume.Value,  0.1);
            }
        }
    }
}
