using System.Collections.Generic;
using System.Windows.Media;
using CoCo.Analyser;
using CoCo.Analyser.CSharp;
using CoCo.Analyser.VisualBasic;
using CoCo.Settings;

namespace CoCo.Services
{
    public static class PresetService
    {
        private static Dictionary<string, IReadOnlyDictionary<string, PresetSettings>> _defaultPresets;

        /// <summary>
        /// Returns the default CoCo settings that are grouped by languages
        /// </summary>
        public static IReadOnlyDictionary<string, IReadOnlyDictionary<string, PresetSettings>> GetDefaultPresets()
        {
            if (!(_defaultPresets is null)) return _defaultPresets;

            _defaultPresets = new Dictionary<string, IReadOnlyDictionary<string, PresetSettings>>();

            var defaulltIdentifierFormatting = FormattingService.GetDefaultIdentifierFormatting();

            ClassificationSettings CreateClassification(string name, byte r, byte g, byte b)
            {
                var defaultFormatting = defaulltIdentifierFormatting;
                if (ClassificationManager.TryGetDefaultNonIdentifierClassification(name, out var defaultClassification))
                {
                    defaultFormatting = FormattingService.GetDefaultFormatting(defaultClassification);
                }

                var classification = defaultFormatting.ToDefaultSettings(name);
                classification.Foreground = Color.FromRgb(r, g, b);
                return classification;
            }

            var presets = new Dictionary<string, PresetSettings>
            {
                ["CoCo light|blue theme"] = new PresetSettings
                {
                    Name = "CoCo light|blue theme",
                    Classifications = new List<ClassificationSettings>
                    {
                        CreateClassification(CSharpNames.ConstantFieldName, 255, 157, 255),
                        CreateClassification(CSharpNames.EnumFieldName, 0, 193, 193),
                        CreateClassification(CSharpNames.ExtensionMethodName, 233, 122, 0),
                        CreateClassification(CSharpNames.FieldName, 255, 157, 255),
                        CreateClassification(CSharpNames.LabelName, 120, 20, 0),
                        CreateClassification(CSharpNames.LocalMethodName, 187, 20, 0),
                        CreateClassification(CSharpNames.LocalVariableName, 128, 128, 0),
                        CreateClassification(CSharpNames.MethodName, 187, 54, 4),
                        CreateClassification(CSharpNames.ParameterName, 128, 128, 128),
                        CreateClassification(CSharpNames.PropertyName, 255, 0, 255),
                        CreateClassification(CSharpNames.RangeVariableName, 128, 128, 0),
                        CreateClassification(CSharpNames.StaticMethodName, 154, 82, 0)
                    }
                },

                ["CoCo dark theme"] = new PresetSettings
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
                        CreateClassification(CSharpNames.LocalMethodName, 150, 10, 10),
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
            _defaultPresets[Languages.CSharp] = presets;

            presets = new Dictionary<string, PresetSettings>
            {
                ["CoCo light|blue theme"] = new PresetSettings
                {
                    Name = "CoCo light|blue theme",
                    Classifications = new List<ClassificationSettings>
                    {
                        CreateClassification(VisualBasicNames.ConstantFieldName, 255, 157, 255),
                        CreateClassification(VisualBasicNames.EnumFieldName, 0, 193, 193),
                        CreateClassification(VisualBasicNames.ExtensionMethodName, 233, 122, 0),
                        CreateClassification(VisualBasicNames.FieldName, 255, 157, 255),
                        CreateClassification(VisualBasicNames.FunctionName, 187, 54, 4),
                        CreateClassification(VisualBasicNames.FunctionVariableName, 128, 64, 0),
                        CreateClassification(VisualBasicNames.LocalVariableName, 128, 128, 0),
                        CreateClassification(VisualBasicNames.ParameterName, 128, 128, 128),
                        CreateClassification(VisualBasicNames.PropertyName, 255, 0, 255),
                        CreateClassification(VisualBasicNames.RangeVariableName, 128, 128, 0),
                        CreateClassification(VisualBasicNames.SharedMethodName, 154, 82, 0),
                        CreateClassification(VisualBasicNames.StaticLocalVariableName, 64, 128, 0),
                        CreateClassification(VisualBasicNames.SubName, 187, 34, 0),
                        CreateClassification(VisualBasicNames.WithEventsPropertyName, 255, 0, 128)
                    }
                },

                ["CoCo dark theme"] = new PresetSettings
                {
                    Name = "CoCo dark theme",
                    Classifications = new List<ClassificationSettings>
                    {
                        CreateClassification(VisualBasicNames.AliasNamespaceName, 128, 0, 255),
                        CreateClassification(VisualBasicNames.ConstantFieldName, 174, 0, 174),
                        CreateClassification(VisualBasicNames.EnumFieldName, 0, 193, 193),
                        CreateClassification(VisualBasicNames.EventName, 200, 0, 128),
                        CreateClassification(VisualBasicNames.ExtensionMethodName, 224, 118, 0),
                        CreateClassification(VisualBasicNames.FieldName, 174, 0, 174),
                        CreateClassification(VisualBasicNames.FunctionName, 187, 0, 0),
                        CreateClassification(VisualBasicNames.FunctionVariableName, 128, 64, 0),
                        CreateClassification(VisualBasicNames.LocalVariableName, 128, 128, 0),
                        CreateClassification(VisualBasicNames.NamespaceName, 0, 213, 0),
                        CreateClassification(VisualBasicNames.ParameterName, 108, 108, 108),
                        CreateClassification(VisualBasicNames.PropertyName, 255, 0, 255),
                        CreateClassification(VisualBasicNames.RangeVariableName, 128, 128, 0),
                        CreateClassification(VisualBasicNames.SharedMethodName, 163, 86, 0),
                        CreateClassification(VisualBasicNames.StaticLocalVariableName, 64, 128, 0),
                        CreateClassification(VisualBasicNames.SubName, 187, 34, 0),
                        CreateClassification(VisualBasicNames.WithEventsPropertyName, 255, 0, 128)
                    }
                }
            };
            _defaultPresets[Languages.VisualBasic] = presets;

            return _defaultPresets;
        }
    }
}