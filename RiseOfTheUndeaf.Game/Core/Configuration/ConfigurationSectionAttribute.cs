using Stride.Core.Reflection;
using System;

namespace RiseOfTheUndeaf.Core.Configuration
{
    /// <summary>
    /// Description of a configuration section for user config classes.
    /// </summary>
    [AssemblyScan]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ConfigurationSectionAttribute : Attribute
    {
        /// <summary>
        /// Description of a configuration section for user config classes.
        /// </summary>
        /// <param name="sectionName">Name of the section in the configuration file.</param>
        public ConfigurationSectionAttribute(string sectionName) => SectionName = sectionName;

        public string SectionName { get; }
    }
}
