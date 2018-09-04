using System.Collections.Generic;
using System.Windows.Media;
using CoCo.Analyser.CSharp;
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
                var classification = defaultFormatting.ToDefaultSettings(name);
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
                        CreateClassification(CSharpNames.AliasNamespaceName, 220, 220, 220),
                        CreateClassification(CSharpNames.ConstantFieldName, 255, 157, 255),
                        CreateClassification(CSharpNames.ConstructorName, 220, 220, 220),
                        CreateClassification(CSharpNames.DestructorName, 220, 220, 220),
                        CreateClassification(CSharpNames.EnumFieldName, 0, 193, 193),
                        CreateClassification(CSharpNames.EventName, 220, 220, 220),
                        CreateClassification(CSharpNames.ExtensionMethodName, 233, 122, 0),
                        CreateClassification(CSharpNames.FieldName, 255, 157, 255),
                        CreateClassification(CSharpNames.LabelName, 120, 20, 0),
                        CreateClassification(CSharpNames.LocalVariableName, 128, 128, 0),
                        CreateClassification(CSharpNames.MethodName, 187, 54, 4),
                        CreateClassification(CSharpNames.NamespaceName, 220, 220, 220),
                        CreateClassification(CSharpNames.ParameterName, 128, 128, 128),
                        CreateClassification(CSharpNames.PropertyName, 255, 0, 255),
                        CreateClassification(CSharpNames.RangeVariableName, 128, 128, 0),
                        CreateClassification(CSharpNames.StaticMethodName, 154, 82, 0)
                    }
                },
                new PresetSettings
                {
                    Name = "CoCo dark theme",
                    Classifications = new List<ClassificationSettings>
                    {
                        CreateClassification(CSharpNames.AliasNamespaceName, 128, 0, 255),
                        CreateClassification(CSharpNames.ConstantFieldName, 174, 0, 174),
                        CreateClassification(CSharpNames.ConstructorName, 255, 0, 0),
                        CreateClassification(CSharpNames.DestructorName, 255, 0, 0),
                        CreateClassification(CSharpNames.EnumFieldName, 0, 193, 193),
                        CreateClassification(CSharpNames.EventName, 200, 0, 128),
                        CreateClassification(CSharpNames.ExtensionMethodName, 224, 118, 0),
                        CreateClassification(CSharpNames.FieldName, 174, 0, 174),
                        CreateClassification(CSharpNames.LabelName, 90, 10, 10),
                        CreateClassification(CSharpNames.LocalVariableName, 128, 128, 0),
                        CreateClassification(CSharpNames.MethodName, 187, 0, 0),
                        CreateClassification(CSharpNames.NamespaceName, 0, 213, 0),
                        CreateClassification(CSharpNames.ParameterName, 108, 108, 108),
                        CreateClassification(CSharpNames.PropertyName, 255, 0, 255),
                        CreateClassification(CSharpNames.RangeVariableName, 128, 128, 0),
                        CreateClassification(CSharpNames.StaticMethodName, 163, 86, 0)
                    }
                }
            };

            return new Dictionary<string, List<PresetSettings>> { ["CSharp"] = presets };
        }
    }
}