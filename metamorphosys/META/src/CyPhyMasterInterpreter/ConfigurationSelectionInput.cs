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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GME.MGA;

namespace CyPhyMasterInterpreter
{
    public class ConfigurationSelectionOutput
    {
        /// <summary>
        /// Selected configurations.
        /// </summary>
        public GMELightObject[] SelectedConfigurations { get; set; }

        public bool PostToJobManager { get; set; }

        public bool KeepTemporaryModels { get; set; }

        public bool OpenDashboard { get; set; }

        public bool VerboseLogging { get; set; }
        // TODO: add Job Manager Instance selection (maybe url/port number is enough or some kind of descriptor)
    }

    public class ConfigurationSelectionInput
    {
        /// <summary>
        /// In which context the interpreter is running.
        /// </summary>
        public GMELightObject Context { get; set; }

        /// <summary>
        /// Top level system under test's referred object.
        /// </summary>
        public GMELightObject Target { get; set; }

        /// <summary>
        /// Configuration groups.
        /// </summary>
        public ConfigurationGroupLight[] Groups { get; set; }

        public bool IsDesignSpace { get; set; }

        /// <summary>
        /// Status text about the mode of operation.
        /// </summary>
        public string OperationModeInformation { get; set; }

        /// <summary>
        /// Name of the interpreters from the workflow.
        /// </summary>
        public string[] InterpreterNames { get; set; }

        public string OutputDirectory { get; set; }
    }

    public class ConfigurationGroupLight
    {
        /// <summary>
        /// Owner of the group: either exported configurations object or null if
        /// top level system under test points to component assembly.
        /// </summary>
        public GMELightObject Owner { get; set; }

        /// <summary>
        /// CWCs or Component assemblies from which the selection can be made.
        /// </summary>
        public GMELightObject[] Configurations { get; set; }

        /// <summary>
        /// Configurations probably out of date.
        /// </summary>
        public bool IsDirty { get; set; }

        public override string ToString()
        {
            return this.Owner == null ? string.Empty : this.Owner.Name;
        }
    }

    public class ConfigurationGroup
    {
        /// <summary>
        /// Owner of the group: either exported configurations object or null if
        /// top level system under test points to component assembly.
        /// </summary>
        public IMgaModel Owner { get; set; }

        /// <summary>
        /// CWCs or Component assemblies from which the selection can be made.
        /// </summary>
        public IMgaFCOs Configurations { get; set; }

        /// <summary>
        /// Configurations probably out of date.
        /// </summary>
        public bool IsDirty { get; set; }

        public ConfigurationGroupLight ConvertToLight()
        {
            ConfigurationGroupLight configurationGroupLight = new ConfigurationGroupLight()
            {
                Configurations = this.Configurations.Cast<MgaFCO>().Select(x => GMELightObject.GetGMELightFromMgaObject(x)).ToArray(),
                IsDirty = this.IsDirty,
                Owner = this.Owner == null ? null : GMELightObject.GetGMELightFromMgaObject(this.Owner)
            };

            return configurationGroupLight;
        }
    }

    public class GMELightObject
    {
        public static string ShortenAbsPath(string absPath)
        {
            // Example:
            // input:  /@NewTesting|kind=Testing|relpos=0/@FEA_CompoundTIP_DS|kind=CADTestBench|relpos=0/@DesignContainer|kind=TopLevelSystemUnderTest|relpos=0
            // output: /@NewTesting/@FEA_CompoundTIP_DS/@DesignContainer
            return string.Join("/", absPath.Split('/').Select(x => string.Join("", x.TakeWhile(y => y != '|'))));
        }

        public string GMEId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string AbsPath { get; set; }

        public override int GetHashCode()
        {
            return this.GMEId.GetHashCode();
        }

        public string ToolTip { get; set; }

        public override string ToString()
        {
            return this.Name;
        }

        public static GMELightObject GetGMELightFromMgaObject(IMgaObject subject)
        {
            GMELightObject gmeLightObject = new GMELightObject();

            gmeLightObject.AbsPath = subject.AbsPath;
            gmeLightObject.GMEId = subject.ID;
            
            // TODO: figure out how to show to the user configurations when they have no uniqie names.
            //// dashboard shows last 4 digit of the guid
            //var guidStr = new Guid(subject.GetGuidDisp()).ToString();
            //string guidPortion = guidStr.Substring(guidStr.Length - 4);
            //gmeLightObject.Name = string.Format("[#{0}] {1}", guidPortion , subject.Name);

            gmeLightObject.Name = subject.Name;
            // TODO: set tool tip to ParentName/ObjectName
            // TODO: would be nice to use a regexp or something like that
            gmeLightObject.ToolTip = ShortenAbsPath(subject.AbsPath);
            gmeLightObject.Type = subject.MetaBase.Name;

            return gmeLightObject;
        }
    }
}