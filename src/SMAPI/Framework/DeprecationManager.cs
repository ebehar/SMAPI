using System;
using System.Collections.Generic;

namespace StardewModdingAPI.Framework
{
    /// <summary>Manages deprecation warnings.</summary>
    internal class DeprecationManager
    {
        /*********
        ** Properties
        *********/
        /// <summary>The deprecations which have already been logged (as 'mod name::noun phrase::version').</summary>
        private readonly HashSet<string> LoggedDeprecations = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>Encapsulates monitoring and logging for a given module.</summary>
        private readonly IMonitor Monitor;

        /// <summary>Tracks the installed mods.</summary>
        private readonly ModRegistry ModRegistry;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="monitor">Encapsulates monitoring and logging for a given module.</param>
        /// <param name="modRegistry">Tracks the installed mods.</param>
        public DeprecationManager(IMonitor monitor, ModRegistry modRegistry)
        {
            this.Monitor = monitor;
            this.ModRegistry = modRegistry;
        }

        /// <summary>Log a deprecation warning.</summary>
        /// <param name="nounPhrase">A noun phrase describing what is deprecated.</param>
        /// <param name="version">The SMAPI version which deprecated it.</param>
        /// <param name="severity">How deprecated the code is.</param>
        public void Warn(string nounPhrase, string version, DeprecationLevel severity)
        {
            this.Warn(this.ModRegistry.GetFromStack()?.DisplayName, nounPhrase, version, severity);
        }

        /// <summary>Log a deprecation warning.</summary>
        /// <param name="source">The friendly mod name which used the deprecated code.</param>
        /// <param name="nounPhrase">A noun phrase describing what is deprecated.</param>
        /// <param name="version">The SMAPI version which deprecated it.</param>
        /// <param name="severity">How deprecated the code is.</param>
        public void Warn(string source, string nounPhrase, string version, DeprecationLevel severity)
        {
            // ignore if already warned
            if (!this.MarkWarned(source ?? "<unknown>", nounPhrase, version))
                return;

            // build message
            string message = $"{source ?? "An unknown mod"} uses deprecated code ({nounPhrase} is deprecated since SMAPI {version}).";
            if (source == null)
                message += $"{Environment.NewLine}{Environment.StackTrace}";

            // log message
            switch (severity)
            {
                case DeprecationLevel.Notice:
                    this.Monitor.Log(message, LogLevel.Trace);
                    break;

                case DeprecationLevel.Info:
                    this.Monitor.Log(message, LogLevel.Debug);
                    break;

                case DeprecationLevel.PendingRemoval:
                    this.Monitor.Log(message, LogLevel.Warn);
                    break;

                default:
                    throw new NotSupportedException($"Unknown deprecation level '{severity}'");
            }
        }

        /// <summary>Mark a deprecation warning as already logged.</summary>
        /// <param name="nounPhrase">A noun phrase describing what is deprecated (e.g. "the Extensions.AsInt32 method").</param>
        /// <param name="version">The SMAPI version which deprecated it.</param>
        /// <returns>Returns whether the deprecation was successfully marked as warned. Returns <c>false</c> if it was already marked.</returns>
        public bool MarkWarned(string nounPhrase, string version)
        {
            return this.MarkWarned(this.ModRegistry.GetFromStack()?.DisplayName, nounPhrase, version);
        }

        /// <summary>Mark a deprecation warning as already logged.</summary>
        /// <param name="source">The friendly name of the assembly which used the deprecated code.</param>
        /// <param name="nounPhrase">A noun phrase describing what is deprecated (e.g. "the Extensions.AsInt32 method").</param>
        /// <param name="version">The SMAPI version which deprecated it.</param>
        /// <returns>Returns whether the deprecation was successfully marked as warned. Returns <c>false</c> if it was already marked.</returns>
        public bool MarkWarned(string source, string nounPhrase, string version)
        {
            if (string.IsNullOrWhiteSpace(source))
                throw new InvalidOperationException("The deprecation source cannot be empty.");

            string key = $"{source}::{nounPhrase}::{version}";
            if (this.LoggedDeprecations.Contains(key))
                return false;
            this.LoggedDeprecations.Add(key);
            return true;
        }
    }
}
