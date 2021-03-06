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
using GME;
using GME.MGA.Meta;
//using GME.MGA.Core;

namespace GME.CSharp
{
    class MgaGateway
    {
        public MgaGateway(IMgaProject project)
        {
            this.project = project;
        }

        public IMgaProject project = null;
        public IMgaTerritory territory = null;

        #region TRANSACTION HANDLING
        public void BeginTransaction(transactiontype_enum mode = transactiontype_enum.TRANSACTION_GENERAL)
        {
            project.BeginTransaction(territory, mode);
        }

        public void CommitTransaction()
        {
            if ((project.ProjectStatus & 8) != 0)
            {
                project.CommitTransaction();
            }
        }

        public void AbortTransaction()
        {
            if ((project.ProjectStatus & 8) != 0)
            {
                project.AbortTransaction();
            }
        }

        public delegate void voidDelegate();
        public void PerformInTransaction(voidDelegate d, transactiontype_enum mode = transactiontype_enum.TRANSACTION_GENERAL)
        {
            BeginTransaction(mode);
            try
            {
                d();
                CommitTransaction();
            }
            finally
            {
                AbortTransaction();
            }
        }
        #endregion
        #region UTILITIES
        public IMgaMetaBase GetMetaByName(string name)
        {
            try
            {
                return project.RootMeta.RootFolder.get_DefinedFCOByName(name, false) as MgaMetaFCO;
            }
#pragma warning disable 0168
            catch (System.Runtime.InteropServices.COMException e)
            {
                return project.RootMeta.RootFolder.get_DefinedFolderByName(name, false) as MgaMetaFolder;
            }
#pragma warning restore 0168
        }

        #endregion


    }
}
