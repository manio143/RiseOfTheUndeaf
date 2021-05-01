using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using RiseOfTheUndeaf.Core.Logging;
using Stride.Core.IO;
using Stride.Core.Reflection;
using Stride.Core.Settings;

namespace RiseOfTheUndeaf.Core.Configuration
{
    public class RoamingAppSettingsProvider : IAppSettingsProvider
    {
        /// <summary>
        /// Loads application settings from a configuration file <c>config.ini</c> in the roaming folder.
        /// </summary>
        public AppSettings LoadAppSettings()
        {
            // Note: cannot log in this method, since it might be invoked in static ctor of the logger when using Stride.Core.Diagnostics.Logger
            if (!VirtualFileSystem.ApplicationRoaming.FileExists("config.ini"))
                return new AppSettings();

            IEnumerable<IniParser.Record> records;
            using (var configFile = VirtualFileSystem.ApplicationRoaming.OpenStream("config.ini", VirtualFileMode.Open, VirtualFileAccess.Read))
                records = IniParser.Parse(configFile);

            var availableSettings = AssemblyRegistry.FindAll()
                .SelectMany(assembly => (IReadOnlyCollection<Type>)AssemblyRegistry.GetScanTypes(assembly)?.Types
                    .GetValueOrDefault(typeof(ConfigurationSectionAttribute)) ?? Type.EmptyTypes)
                .Select(type => (type.GetCustomAttribute<ConfigurationSectionAttribute>()?.SectionName, type))
                .Where(pair => !string.IsNullOrWhiteSpace(pair.SectionName))
                .ToDictionary(pair => pair.SectionName, pair => pair.type);

            var sections = new Dictionary<string, object>();

            foreach (var record in records)
            {
                if (!sections.TryGetValue(record.Section, out object setting) &&
                    availableSettings.ContainsKey(record.Section))
                {
                    setting = Activator.CreateInstance(availableSettings[record.Section]);
                    sections.Add(record.Section, setting);
                }

                if (setting == null) continue;

                var type = availableSettings[record.Section];
                var prop = type.GetProperty(record.Key);

                if (prop == null) continue;

                if (prop.PropertyType == typeof(string))
                {
                    prop.SetValue(setting, record.Value);
                }
                else if (prop.PropertyType.IsEnum && Enum.TryParse(prop.PropertyType, record.Value, out object enumValue))
                {
                    prop.SetValue(setting, enumValue);
                }
                else if (prop.PropertyType.IsPrimitive)
                {
                    if (record.Value.Contains('.') && double.TryParse(record.Value, out var doubleValue))
                    {
                        prop.SetValue(setting, Convert.ChangeType(doubleValue, prop.PropertyType));
                    }
                    else if (long.TryParse(record.Value, out var longValue))
                    {
                        prop.SetValue(setting, Convert.ChangeType(longValue, prop.PropertyType));
                    }
                }
            }

            return new AppSettings(sections.Values);
        }

        /// <summary>
        /// Persists <paramref name="appSettings"/> into a configuration file <c>config.ini</c> in the roaming folder.
        /// </summary>
        /// <param name="appSettings">Settings collection.</param>
        public void WriteAppSettings(AppSettings appSettings)
        {
            this.LogInfo("Writing AppSettings to 'roaming/config.ini'.");

            using var configFile = VirtualFileSystem.ApplicationRoaming.OpenStream("config.ini", VirtualFileMode.Truncate, VirtualFileAccess.Write);
            using var writer = new StreamWriter(configFile);

            foreach (var setting in appSettings)
            {
                var sectionName = setting.GetType().GetCustomAttribute<ConfigurationSectionAttribute>()?.SectionName;

                if (string.IsNullOrWhiteSpace(sectionName))
                {
                    this.LogWarning("Settings object of type '{settingType}' has no section name configured, skipping.", setting.GetType().FullName);
                    continue;
                }

                this.LogInfo("Writing section '{sectionName}'.", sectionName);

                writer.WriteSection(sectionName);

                foreach (var prop in setting.GetType().GetProperties())
                {
                    writer.WriteKeyValue(prop.Name, prop.GetValue(setting));
                }

                writer.WriteLine(); // empty line separating sections
            }
        }
    }

    internal static class IniParser
    {
        public class Record
        {
            public string Section { get; init; }
            public string Key { get; init; }
            public string Value { get; set; }
        }

        public static IEnumerable<Record> Parse(Stream stream)
        {
            var records = new List<Record>();
            string currentSection = null;

            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                // remove comments
                if (line.Contains(';'))
                {
                    line = line.Substring(0, line.IndexOf(';'));
                }

                line = line.Trim();

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    currentSection = line[1..^1].Trim();
                }
                else
                {
                    var split = line.Split("=", System.StringSplitOptions.TrimEntries);

                    if (currentSection == null || split.Length != 2)
                        continue;

                    records.Add(new Record
                    {
                        Section = currentSection,
                        Key = split[0],
                        Value = split[1],
                    });
                }
            }

            return records;
        }

        public static void WriteSection(this StreamWriter writer, string sectionName)
        {
            writer.Write("[");
            writer.Write(sectionName);
            writer.WriteLine("]");
        }

        public static void WriteKeyValue(this StreamWriter writer, string key, object value)
        {
            writer.Write(key);
            writer.Write("=");
            writer.WriteLine(value);
        }
    }
}
