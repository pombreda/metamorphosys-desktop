﻿/*
Copyright (C) 2013-2015 MetaMorph Software, Inc

Permission is hereby granted, free of charge, to any person obtaining a
copy of this data, including any software or models in source or binary
form, as well as any drawings, specifications, and documentation
(collectively "the Data"), to deal in the Data without restriction,
including without limitation the rights to use, copy, modify, merge,
publish, distribute, sublicense, and/or sell copies of the Data, and to
permit persons to whom the Data is furnished to do so, subject to the
following conditions:

The above copyright notice and this permission notice shall be included
in all copies or substantial portions of the Data.

THE DATA IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
THE AUTHORS, SPONSORS, DEVELOPERS, CONTRIBUTORS, OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE DATA OR THE USE OR OTHER DEALINGS IN THE DATA.  

=======================
This version of the META tools is a fork of an original version produced
by Vanderbilt University's Institute for Software Integrated Systems (ISIS).
Their license statement:

Copyright (C) 2011-2014 Vanderbilt University

Developed with the sponsorship of the Defense Advanced Research Projects
Agency (DARPA) and delivered to the U.S. Government with Unlimited Rights
as defined in DFARS 252.227-7013.

Permission is hereby granted, free of charge, to any person obtaining a
copy of this data, including any software or models in source or binary
form, as well as any drawings, specifications, and documentation
(collectively "the Data"), to deal in the Data without restriction,
including without limitation the rights to use, copy, modify, merge,
publish, distribute, sublicense, and/or sell copies of the Data, and to
permit persons to whom the Data is furnished to do so, subject to the
following conditions:

The above copyright notice and this permission notice shall be included
in all copies or substantial portions of the Data.

THE DATA IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
THE AUTHORS, SPONSORS, DEVELOPERS, CONTRIBUTORS, OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE DATA OR THE USE OR OTHER DEALINGS IN THE DATA.  
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;
using System.Collections.Generic;
using XSD2CSharp;
using GME.MGA;

namespace ComponentExporterUnitTests
{
    public class Tests
    {
        private static readonly string InterchangeTestDirectory = Path.Combine(META.VersionInfo.MetaPath, "test", "InterchangeTest");
        private static string _interchangeBinDirectory = Path.Combine(InterchangeTestDirectory, "InterchangeXmlComparator", "bin", "Release");
        private static string _exportModelDirectory = Path.Combine(InterchangeTestDirectory, "ComponentInterchangeTest", "ExportTestModels");
        //private static string _importModelDirectory = Path.Combine(InterchangeTestDirectory, "ComponentInterchangeTest", "ImportTestModels");

        private static string gmepyPath = Path.Combine(META.VersionInfo.MetaPath, "externals", "common-scripts", "gmepy.exe");



        private void unpackXmes(string testName)
        {
            unpackXme(Path.Combine(_exportModelDirectory, testName, "DesiredResult.xme"));
            unpackXme(Path.Combine(_exportModelDirectory, testName, "InputModel.xme"));
        }

        private string unpackXme(string xmeFilename)
        {
            if (!File.Exists(xmeFilename)) return null;
            string mgaFilename = Path.ChangeExtension(xmeFilename, "mga");
            GME.MGA.MgaUtils.ImportXME(xmeFilename, mgaFilename);
            return mgaFilename;
        }

        private int runCyPhyComponentExporterCL(string testName)
        {
            var originalPath = Directory.GetCurrentDirectory();
            var testPath = Path.Combine(_exportModelDirectory, testName);

            try
            {
                Directory.SetCurrentDirectory(testPath);
                var arguments = ("DesiredResult.mga" + " " + "components").Split(' ');

                int result = -1;
                result = CyPhyComponentExporterCL.CyPhyComponentExporterCL.Main(arguments);
                Directory.SetCurrentDirectory(originalPath);

                return result;
            }
            catch (Exception ex)
            {
                Directory.SetCurrentDirectory(originalPath);
                throw ex;
            }
        }

        private int runCyPhyComponentExporterCL_OnAllComponents(string testName, string mgaName = "InputModel.mga")
        {
            var originalPath = Directory.GetCurrentDirectory();
            var testPath = Path.Combine(_exportModelDirectory, testName);

            try
            {
                Directory.SetCurrentDirectory(testPath);
                var arguments = (mgaName).Split(' ');

                int result = -1;
                result = CyPhyComponentExporterCL.CyPhyComponentExporterCL.Main(arguments);
                Directory.SetCurrentDirectory(originalPath);

                return result;
            }
            catch (Exception ex)
            {
                Directory.SetCurrentDirectory(originalPath);
                throw ex;
            }
        }

        private int runCyPhyComponentImporterCL(string testName)
        {
            var originalPath = Directory.GetCurrentDirectory();
            var testPath = Path.Combine(_exportModelDirectory, testName);

            try
            {
                Directory.SetCurrentDirectory(testPath);
                var arguments = (Path.Combine("components",
                                              "Components",
                                              "Cross_Drive_without_TC",
                                              "Cross_Drive_without_TC.component.acm")
                                + " " +
                                "InputModel.mga").Split(' ');

                int result = -1;
                result = CyPhyComponentImporterCL.CyPhyComponentImporterCL.Main(arguments);
                Directory.SetCurrentDirectory(originalPath);

                return result;
            }
            catch (Exception ex)
            {
                Directory.SetCurrentDirectory(originalPath);
                throw ex;
            }
        }

        private int runCyPhyComponentImporterCLDrawbar(string testName)
        {
            var process = new Process
                          {
                              StartInfo =
                              {
                                  FileName = Path.Combine(META.VersionInfo.MetaPath, "src", "CyPhyComponentImporterCL", "bin", "Release", "CyPhyComponentImporterCL.exe"),
                                  WorkingDirectory = Path.Combine(_exportModelDirectory, testName)
                              }
                          };

            process.StartInfo.Arguments += "\"components/Imported_Components/drawbar/drawbar.component.acm\"";
            process.StartInfo.Arguments += " InputModel.mga";

            return Common.processCommon(process);
        }

        private int runCyPhyComponentImporterCLHull(string testName)
        {
/*
            string currentDir = Directory.GetCurrentDirectory();
            try
            {
                Directory.SetCurrentDirectory(_exportModelDirectory);
                return CyPhyComponentImporterCL.CyPhyComponentImporterCL.Main(
                    new string[] { 
                    testName + "/components/Imported_Components/hull/hull.component.acm", testName + "/InputModel.mga"
                });
            }
            finally
            {
                Directory.SetCurrentDirectory(currentDir);
            }
*/
            var process = new Process
                          {
                              StartInfo =
                              {
                                  FileName = Path.Combine(META.VersionInfo.MetaPath, "src", "CyPhyComponentImporterCL", "bin", "Release", "CyPhyComponentImporterCL.exe"),
                                  WorkingDirectory = _exportModelDirectory
                              }
                          };

            process.StartInfo.Arguments += testName + "\"/components/Imported_Components/hull/hull.component.acm\"";
            process.StartInfo.Arguments += " " + testName + "/InputModel.mga";

            return Common.processCommon(process);
        }

        private void runCyPhyMLComparator(string testName)
        {
            string desiredMga = testName + "/DesiredResult.mga";
            string inputMga = testName + "/InputModel.mga";
            runCyPhyMLComparator(desiredMga, inputMga, _exportModelDirectory);
        }

        public static void runCyPhyMLComparator(string desiredMga, string inputMga, string workingDirectory)
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = Path.Combine(META.VersionInfo.MetaPath, "src", "bin", "CyPhyMLComparator.exe"),
                    WorkingDirectory = workingDirectory
                }
            };

            process.StartInfo.Arguments += " " + desiredMga;
            process.StartInfo.Arguments += " " + inputMga;

            string output;
            Assert.True(0 == Common.runProcessAndGetOutput(process, out output, err_only: true), process.StartInfo.FileName + " " + process.StartInfo.Arguments + " failed:" + output);
        }

        [Fact]
        public void ExportTest()
        {
            const string testName = "ExportTest";
            unpackXmes(testName);
            Assert.True(0 == runCyPhyComponentExporterCL(testName), "Component Exporter had exceptions");
            Assert.True(0 == runCyPhyComponentImporterCL(testName), "Component Importer had exceptions");
            runCyPhyMLComparator(testName);
        }

        public int RunPropertiesWithinConnectorsTest()
        {
            var modelsDir = Path.Combine(_exportModelDirectory, "PropInCon");
            var process = new Process
                          {
                              StartInfo =
                              {
                                  FileName = Path.Combine(META.VersionInfo.MetaPath, "src", "CyPhyComponentExporterCL", "bin", "Release", "CyPhyComponentExporterCL.exe"),
                                  WorkingDirectory = modelsDir
                              }
                          };



            //process.StartInfo.Arguments += String.Format(" -e {0} -d {1}", @"Models\PropertiesWithinConnectors\InputModel.mga", @"Models\PropertiesWithinConnectors\expected.component.acm");
            process.StartInfo.Arguments += @"InputModel.mga components";

            string output;
            var result = Common.runProcessAndGetOutput(process, out output);
            Assert.True(result == 0, output);

            //return 0;
            process = new Process
                              {
                                  StartInfo =
                                  {
                                      FileName = Path.Combine(_interchangeBinDirectory, "InterchangeXmlComparator.exe"),
                                      WorkingDirectory = modelsDir
                                  }
                              };

            process.StartInfo.Arguments += String.Format("-d \"{0}\" -e \"{1}\" -m Component", @"expected.component.acm", @"components\Imported_Components\bolt_hex__generic__m4x0p7x6_8p8_zin\bolt_hex__generic__m4x0p7x6_8p8_zin.component.acm");

            var comparatorResult = Common.runProcessAndGetOutput(process, out output);
            Assert.True(comparatorResult == 0, output);
            return comparatorResult;
        }


        private int RunXmlComparator(string exported, string desired)
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = Path.Combine(_interchangeBinDirectory, "InterchangeXmlComparator.exe"),
                    WorkingDirectory = new FileInfo(exported).DirectoryName
                }
            };


            process.StartInfo.Arguments += String.Format(" -e {0} -d {1} -m Component", exported, desired);
            return Common.processCommon(process, true);
        }

        [Fact]
        public void RoundTripExportAndImportTest()
        {
            const string testName = "RoundTripTest";
            unpackXmes(testName);
            Assert.Equal(0, runCyPhyComponentExporterCL(testName));
            Assert.Equal(0, runCyPhyComponentImporterCLDrawbar(testName));
            Assert.Equal(0, runCyPhyComponentImporterCLHull(testName));
            // META-1184
            // Assert.Equal(0, runCyPhyMLComparator(testName));
        }

        [Fact]
        public void PropertiesInConnectorsTest()
        {
            var xmePath = Path.Combine(_exportModelDirectory, "PropInCon", "InputModel.xme");
            unpackXme(xmePath);
            Assert.Equal(0, RunPropertiesWithinConnectorsTest());
        }
        
        [Fact]
        public void FormulaTest()
        {
            var xmePath = Path.Combine(_exportModelDirectory, "FormulaTest", "InputModel.xme");
            string mgaFilename = unpackXme(xmePath);
            IMgaProject project = (MgaProject) Activator.CreateInstance(Type.GetTypeFromProgID("Mga.MgaProject"));
            project.OpenEx("MGA=" + mgaFilename, "CyPhyML", null);
            try
            {
                project.BeginTransactionInNewTerr();
                try
                {
                    MgaFCO fco = (MgaFCO) project.RootFolder.ObjectByPath["/@Imported_Components/@FormulaComp"];
                    Exception e = Assert.Throws(typeof(ApplicationException),
                        () => CyPhyComponentExporter.CyPhyComponentExporterInterpreter.ExportComponentPackage(ISIS.GME.Dsml.CyPhyML.Classes.Component.Cast(fco)));
                    Assert.True(e.Message.Contains("Value assignments for Component Parameters must come from outside the Component itself."));
                }
                finally
                {
                    project.AbortTransaction();
                }
            }
            finally
            {
                project.Close(true);
            }
        }

        [Fact]
        public void ParametricProperty()
        {
            string p_TestFolder = Path.Combine(InterchangeTestDirectory,
                                                "ComponentInterchangeTest",
                                                "SharedModels",
                                                "CompParametricProperty");

            string p_DesiredResultXME = Path.Combine(p_TestFolder, "DesiredResult.xme");
            string p_DesiredResultMGA;
            GME.MGA.MgaUtils.ImportXMEForTest(p_DesiredResultXME, out p_DesiredResultMGA);
            p_DesiredResultMGA = p_DesiredResultMGA.Replace("MGA=", "");
            Assert.True(File.Exists(p_DesiredResultMGA), "DesiredResult.mga not found. Import may have failed.");

            Assert.True(0 == Common.runCyPhyComponentExporterCL(p_DesiredResultMGA), "Component exporter failed.");

            String p_expectedACM = Path.Combine(p_TestFolder, "expected.component.acm");
            String p_generatedACM = Path.Combine(p_TestFolder,
                                                "Components",
                                                "Manikin",
                                                "testcomp",
                                                "testcomp.component.acm");

            Assert.True(File.Exists(p_generatedACM), "Exported ACM file not found.");
            Assert.True(0 == RunXmlComparator(p_generatedACM, p_expectedACM), "Generated ACM files doesn't match expected output.");
        }

        [Fact]
        public void CADMetric()
        {
            string p_TestFolder = Path.Combine(InterchangeTestDirectory,
                                                "ComponentInterchangeTest",
                                                "SharedModels",
                                                "CompCadMetric");

            string p_DesiredResultXME = Path.Combine(p_TestFolder, "DesiredResult.xme");
            string p_DesiredResultMGA;
            GME.MGA.MgaUtils.ImportXMEForTest(p_DesiredResultXME, out p_DesiredResultMGA);
            p_DesiredResultMGA = p_DesiredResultMGA.Replace("MGA=", "");
            Assert.True(File.Exists(p_DesiredResultMGA), "DesiredResult.mga not found. Import may have failed.");

            Assert.True(0 == Common.runCyPhyComponentExporterCL(p_DesiredResultMGA), "Component exporter failed.");

            String p_expectedACM = Path.Combine(p_TestFolder, "expected.component.acm");
            String p_generatedACM = Path.Combine(p_TestFolder,
                                                "Components",
                                                "CompWithCADMetric",
                                                "CompWithCADMetric.component.acm");

            Assert.True(File.Exists(p_generatedACM), "Exported ACM file not found.");
            avm.Component component = AvmXmlSerializer.Deserialize<avm.Component>(File.ReadAllText(p_generatedACM));
            var parameterValue = component.Property.OfType<avm.PrimitiveProperty>().First().Value;
            var cadMetricValue = component.DomainModel.OfType<avm.cad.CADModel>().First().ModelMetric.First().Value;
            Assert.True(parameterValue.ValueExpression is avm.DerivedValue);
            Assert.True(cadMetricValue.ValueExpression is avm.FixedValue);
            // TODO: add regression test
            // Assert.True(0 == RunXmlComparator(p_generatedACM, p_expectedACM), "Generated ACM files doesn't match expected output.");
        }

        [Fact]
        [Trait("Interchange", "JoinData")]
        [Trait("Interchange", "Component Export")]
        public void JoinDataInConnectors()
        {
            var testPath = Path.Combine(_exportModelDirectory, "JDConn");
            var xmePath = Path.Combine(testPath, "InputModel.xme");
            unpackXme(xmePath);

            var mgaPath = xmePath.Replace(".xme", ".mga");

            Assert.True(File.Exists(mgaPath), "MGA file not found. Model import may have failed");

            runCyPhyComponentExporterCL_OnAllComponents(testPath);

            foreach (var modelName in JDModelNames)
            {
                Tuple<String, String> modelPaths = GetJDModelPaths(modelName);
                var generatedACM = Path.Combine(_exportModelDirectory, "JDConn", modelPaths.Item1);
                var expectedACM = Path.Combine(_exportModelDirectory, "JDConn", modelPaths.Item2);

                Assert.True(File.Exists(generatedACM), "Exported ACM file not found for " + modelName);
                Assert.True(0 == RunXmlComparator(generatedACM, expectedACM), "Generated ACM file doesn't match expected output for " + modelName);
            }
        }

        private static Tuple<String, String> GetJDModelPaths(String name)
        {
            return new Tuple<String, String>("Components/%0/%0.component.acm".Replace("%0", name), "%0.expected.acm".Replace("%0", name));
        }
        private static List<String> JDModelNames = new List<String>()
        {
            "CompBrazed",
            "CompSoldered",
            "CompGlued",
            "CompMechanical",
            "CompWelded"
        };
    }

    class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            int ret = Xunit.ConsoleClient.Program.Main(new string[] {
                Assembly.GetAssembly(typeof(Tests)).CodeBase.Substring("file:///".Length),
                //"/noshadow",
            });
            Console.In.ReadLine();
            return ret;
        }
    }
}