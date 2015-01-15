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
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Xunit;
using System.Reflection;

namespace SpiceViewerTest
{
    public class SpiceReaderFixture
    {
        private static String pathTestModel = Path.Combine(META.VersionInfo.MetaPath,
                                                           "..",
                                                           "tonka",
                                                           "test",
                                                           "SpiceViewerTest",
                                                           "model");

        private static String pathSPICE = Path.Combine(META.VersionInfo.MetaPath,
                                                       "bin",
                                                       "spice",
                                                       "bin",
                                                       "ngspice.exe");

        public static String pathRAWFile = Path.Combine(pathTestModel,
                                                         "schema.raw");

        public SpiceReaderFixture()
        {
            if (File.Exists(pathRAWFile))
            {
                return;
            }

            var process = new System.Diagnostics.Process()
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = pathSPICE,
                    Arguments = "-b -r schema.raw -o schema.log schema.cir",
                    WorkingDirectory = pathTestModel,
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            };

            process.Start();
            int minsToWait = 8;
            if (process.WaitForExit(1000 * 60 * minsToWait) == false)
            {
                process.Kill();
                throw new TimeoutException(String.Format("{0} did not complete in {1} minutes",
                                                         process.StartInfo.FileName,
                                                         minsToWait));
            }
            Assert.Equal(0, process.ExitCode);
        }
    }

    public class SpiceReader : IUseFixture<SpiceReaderFixture>
    {
        #region fixture
        private SpiceReaderFixture fixture;
        public void SetFixture(SpiceReaderFixture data)
        {
            fixture = data;
        }
        #endregion

        [Fact]
        private void TestSpiceViewer()
        {
            // This function is for testing 
            // \tonka\src\spice_viewer\spice_read.py

            String pathSpiceRead = Path.Combine(META.VersionInfo.MetaPath,
                                                "..",
                                                "tonka",
                                                "src",
                                                "spice_viewer",
                                                "spice_read.py");

            // Test parsing of RAW file
            var process = new System.Diagnostics.Process()
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo()
                {
                    Arguments = String.Format("-m SpiceVisualizer.spicedatareader \"{0}\"", SpiceReaderFixture.pathRAWFile),
                    WorkingDirectory = Path.GetDirectoryName(pathSpiceRead),
                    FileName = META.VersionInfo.PythonVEnvExe,
                    //Arguments = String.Join(" ", pathSpiceRead, SpiceReaderFixture.pathRAWFile),
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                }
            };

            StringBuilder outputData = new StringBuilder();
            process.OutputDataReceived += (o, dataArgs) =>
            {
                if (dataArgs.Data != null)
                {
                    try
                    {
                        outputData.Append(dataArgs.Data);
                    }
                    catch (ObjectDisposedException) { }
                }
            };

            StringBuilder errorData = new StringBuilder();
            process.ErrorDataReceived += (o, dataArgs) =>
            {
                if (dataArgs.Data != null)
                {
                    try
                    {
                        errorData.Append(dataArgs.Data);
                    }
                    catch (ObjectDisposedException) { }
                }
            };
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            int minsToWait = 15;
            if (process.WaitForExit(1000 * 60 * minsToWait) == false)
            {
                process.Kill();
                throw new TimeoutException(String.Format("{0} did not complete in {1} minutes",
                                                         process.StartInfo.FileName,
                                                         minsToWait));
            }

            Console.Out.Write(outputData.ToString());
            Console.Error.Write(errorData.ToString());

            Assert.Equal(0, process.ExitCode);
            Assert.True(outputData.ToString().Contains("Title:  rc time delay circuit"));
        }
    }

    class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            int ret = Xunit.ConsoleClient.Program.Main(new string[] {
                Assembly.GetExecutingAssembly().CodeBase.Substring("file:///".Length),
                //"/noshadow",
            });
            Console.In.ReadLine();
            return ret;
        }
    }

}