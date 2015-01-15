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
using CyPhy = ISIS.GME.Dsml.CyPhyML.Interfaces;
using CyPhyClasses = ISIS.GME.Dsml.CyPhyML.Classes;

namespace CyPhyMasterInterpreter.Rules
{
    /// <summary>
    /// Base class for any context checker.
    /// </summary>
    public abstract class ContextChecker
    {
        /// <summary>
        /// Stores last context check results.
        /// </summary>
        protected List<ContextCheckerResult> m_details = new List<ContextCheckerResult>();

        /// <summary>
        /// Contains detailed results failures and successes about the last context check.
        /// </summary>
        public List<ContextCheckerResult> Details { get { return m_details; } }

        /// <summary>
        /// Gets the model element where the context check starts.
        /// </summary>
        /// <returns>Model context that the context checker can check.</returns>
        public abstract IMgaModel GetContext();

        /// <summary>
        /// Checks the current context without throwing any exceptions.
        /// Clears and saves the context check results.
        /// <para><remarks>Execute within transaction.</remarks></para>
        /// </summary>
        public virtual void CheckNoThrow()
        {
            this.m_details.Clear();

            this.m_details.AddRange(this.NotLibraryOrSubtypeOrInstanceOrReadOnly());
        }

        /// <summary>
        /// Checks the current context and throws exception if any check failed.
        /// <para><remarks>Execute within transaction.</remarks></para>
        /// </summary>
        /// <exception cref="ContextCheckerException" />
        public void Check()
        {
            this.CheckNoThrow();

            this.ThrowIfFailed();
        }

        /// <summary>
        /// Tries to check the given context.
        /// <para><remarks>Execute within transaction.</remarks></para>
        /// </summary>
        /// <returns>True if the context is valid, otherwise false.</returns>
        public bool TryCheck()
        {
            try
            {
                Check();
                return true;
            }
            catch (ContextCheckerException)
            {
                return false;
            }
        }

        /// <summary>
        /// Throws a ContextCheckException if any checks failed.
        /// </summary>
        /// <exception cref="ContextCheckerException" />
        protected void ThrowIfFailed()
        {
            if (this.m_details.Any(x => x.Success == false))
            {
                throw new ContextCheckerException();
            }
        }

        /// <summary>
        /// Context is not a library element or subtype or instance or read-only.
        /// <para><remarks>Execute within transaction.</remarks></para>
        /// </summary>
        /// <returns>Checker results.</returns>
        protected IEnumerable<ContextCheckerResult> NotLibraryOrSubtypeOrInstanceOrReadOnly()
        {
            List<ContextCheckerResult> results = new List<ContextCheckerResult>();

            var context = this.GetContext();

            if (context.IsLibObject)
            {
                var feedback = new ContextCheckerResult()
                {
                    Success = false,
                    Subject = context,
                    Message = string.Format("{0} cannot be a library object.", context.MetaBase.Name)
                };

                results.Add(feedback);
            }
            else
            {
                var feedback = new ContextCheckerResult()
                {
                    Success = true,
                    Subject = context,
                    Message = string.Format("{0} is not a library object.", context.MetaBase.Name)
                };

                results.Add(feedback);
            }

            if (context.IsInstance)
            {
                var feedback = new ContextCheckerResult()
                {
                    Success = false,
                    Subject = context,
                    Message = string.Format("{0} cannot be an instance.", context.MetaBase.Name)
                };

                results.Add(feedback);
            }
            else
            {
                var feedback = new ContextCheckerResult()
                {
                    Success = true,
                    Subject = context,
                    Message = string.Format("{0} is not an instance.", context.MetaBase.Name)
                };

                results.Add(feedback);
            }

            if (context.ArcheType != null)
            {
                var feedback = new ContextCheckerResult()
                {
                    Success = false,
                    Subject = context,
                    Message = string.Format("{0} cannot be an subtype or instance.", context.MetaBase.Name)
                };

                results.Add(feedback);
            }
            else
            {
                var feedback = new ContextCheckerResult()
                {
                    Success = true,
                    Subject = context,
                    Message = string.Format("{0} is not a subtype or instance.", context.MetaBase.Name)
                };

                results.Add(feedback);
            }


            if (context.HasReadOnlyAccess())
            {
                var feedback = new ContextCheckerResult()
                    {
                        Success = false,
                        Subject = context,
                        Message = string.Format("{0} cannot be read-only.", context.MetaBase.Name)
                    };

                results.Add(feedback);
            }
            else
            {
                var feedback = new ContextCheckerResult()
                {
                    Success = true,
                    Subject = context,
                    Message = string.Format("{0} is writable.", context.MetaBase.Name)
                };

                results.Add(feedback);
            }

            return results;
        }

        /// <summary>
        /// Gets a new instance of a context checker based on a given context.
        /// Note: Execute within transaction.
        /// </summary>
        /// <param name="context">Test bench, Parameteric Exploration, or Test bench Suite</param>
        /// <returns>A new context specific checker if context is supported, otherwise null.</returns>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        public static ContextChecker GetContextChecker(IMgaModel context)
        {
            if (context == null)
            {
                throw new ArgumentNullException();
            }
            
            ContextChecker contextChecker = null;

            // get specialized context checker based on the context type.
            if (context.MetaBase.Name == typeof(CyPhy.TestBench).Name)
            {
                contextChecker = new TestBenchChecker(CyPhyClasses.TestBench.Cast(context));
            }
            else if (context.MetaBase.Name == typeof(CyPhy.CADTestBench).Name)
            {
                contextChecker = new StructuralFEATestBenchChecker(CyPhyClasses.CADTestBench.Cast(context));
            }
            else if (context.MetaBase.Name == typeof(CyPhy.KinematicTestBench).Name)
            {
                contextChecker = new KinematicTestBenchChecker(CyPhyClasses.KinematicTestBench.Cast(context));
            }
            else if (context.MetaBase.Name == typeof(CyPhy.CFDTestBench).Name)
            {
                contextChecker = new CFDTestBenchChecker(CyPhyClasses.CFDTestBench.Cast(context));
            }
            else if (context.MetaBase.Name == typeof(CyPhy.BlastTestBench).Name)
            {
                contextChecker = new BlastTestBenchChecker(CyPhyClasses.BlastTestBench.Cast(context));
            }
            else if (context.MetaBase.Name == typeof(CyPhy.BallisticTestBench).Name)
            {
                contextChecker = new BallisticTestBenchChecker(CyPhyClasses.BallisticTestBench.Cast(context));
            }
            else if (context.MetaBase.Name == typeof(CyPhy.TestBenchSuite).Name)
            {
                contextChecker = new TestBenchSuiteChecker(CyPhyClasses.TestBenchSuite.Cast(context));
            }
            else if (context.MetaBase.Name == typeof(CyPhy.ParametricExploration).Name)
            {
                contextChecker = new ParametricExplorationChecker(CyPhyClasses.ParametricExploration.Cast(context));
            }
            else
            {
                throw new ArgumentOutOfRangeException(string.Format("Given context is {0}, which is not supported. Try to run it on a test bench, PET or SoT.", context.Meta.Name));
            }

            return contextChecker;
        }
    }
}