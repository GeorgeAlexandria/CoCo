using System.Collections.Generic;
using System.Windows.Media;
using CoCo.Analyser;
using CoCo.Settings;
using Microsoft.VisualStudio.Text.Formatting;

namespace CoCo
{
    public static class PresetService
    {
        /// <summary>
        /// Returns the default CoCo settings that are grouped by languages
        /// </summary>
        public static Dictionary<string, List<PresetSettings>> GetDefaultPresets(TextFormattingRunProperties defaultFormatting)
        {
            ClassificationSettings CreateClassification(string name, byte r, byte g, byte b)
            {
                var classification = defaultFormatting.ToSettings(name);
                classification.Foreground = Color.FromRgb(r, g, b);
                return classification;
            }

            var presets = new List<PresetSettings>
            {
                new PresetSettings
                {
                    Name = "CoCo light|blue theme",
                    Classifications = new List<ClassificationSettings>
                    {
                        CreateClassification(Names.AliasNamespaceName, 220, 220, 220),
                        CreateClassification(Names.ConstructorName, 220, 220, 220),
                        CreateClassification(Names.EnumFieldName, 0, 193, 193),
                        CreateClassification(Names.EventName, 220, 220, 220),
                        CreateClassification(Names.ExtensionMethodName, 233, 122, 0),
                        CreateClassification(Names.FieldName, 255, 157, 255),
                        CreateClassification(Names.LabelName, 120, 20, 0),
                        CreateClassification(Names.LocalMethodName, 187, 20, 0),
                        CreateClassification(Names.LocalVariableName, 128, 128, 0),
                        CreateClassification(Names.MethodName, 187, 54, 4),
                        CreateClassification(Names.NamespaceName, 220, 220, 220),
                        CreateClassification(Names.ParameterName, 128, 128, 128),
                        CreateClassification(Names.PropertyName, 255, 0, 255),
                        CreateClassification(Names.RangeVariableName, 128, 128, 0),
                        CreateClassification(Names.StaticMethodName, 154, 82, 0)
                    }
                },
                new PresetSettings
                {
                    Name = "CoCo dark theme",
                    Classifications = new List<ClassificationSettings>
                    {
                        CreateClassification(Names.AliasNamespaceName, 128, 0, 255),
                        CreateClassification(Names.ConstructorName, 255, 0, 0),
                        CreateClassification(Names.EnumFieldName, 0, 193, 193),
                        CreateClassification(Names.EventName, 200, 0, 128),
                        CreateClassification(Names.ExtensionMethodName, 224, 118, 0),
                        CreateClassification(Names.FieldName, 174, 0, 174),
                        CreateClassification(Names.LabelName, 90, 10, 10),
                        CreateClassification(Names.LocalMethodName, 150, 10, 10),
                        CreateClassification(Names.LocalVariableName, 128, 128, 0),
                        CreateClassification(Names.MethodName, 187, 0, 0),
                        CreateClassification(Names.NamespaceName, 0, 213, 0),
                        CreateClassification(Names.ParameterName, 108, 108, 108),
                        CreateClassification(Names.PropertyName, 255, 0, 255),
                        CreateClassification(Names.RangeVariableName, 128, 128, 0),
                        CreateClassification(Names.StaticMethodName, 163, 86, 0)
                    }
                }
            };

            return new Dictionary<string, List<PresetSettings>> { ["CSharp"] = presets };
        }
    }
}