using System.Collections.Generic;
using System.IO;
using CoCo.Settings;
using CoCo.UI;

namespace CoCo
{
    public sealed partial class MigrationService : IMigrationService
    {
        public static void MigrateSettingsTo_3_1_0()
        {
            var classificationsFileIsExist = File.Exists(Paths.CoCoClassificationSettingsFile);
            var generalFileIsExist = File.Exists(Paths.CoCoGeneralSettingsFile);

            // NOTE: assumes that if the `CoCo general.config` and `CoCo classifications.config` files exist, the migration is complete
            if (classificationsFileIsExist && generalFileIsExist) return;

            // NOTE: migrate `CoCo.config` to `CoCo classifications.config`
            if (!classificationsFileIsExist)
            {
                var oldFileName = Path.Combine(Paths.CoCoFolder, "CoCo.config");
                if (File.Exists(oldFileName))
                {
                    try
                    {
                        File.Move(oldFileName, Paths.CoCoClassificationSettingsFile);
                    }
                    catch (FileNotFoundException)
                    {
                    }
                }
            }

            // NOTE: migrate `CoCo quick info.config` to `CoCo general.config`
            if (!generalFileIsExist)
            {
                var oldFileName = Path.Combine(Paths.CoCoFolder, "CoCo quick info.config");
                if (File.Exists(oldFileName))
                {
                    try
                    {
                        File.Move(oldFileName, Paths.CoCoGeneralSettingsFile);
                    }
                    catch (FileNotFoundException)
                    {
                    }
                }
            }
        }

        public void MigrateGeneral(
           IReadOnlyDictionary<string, object> properties, ref GeneralLanguageSettings classification)
        {
            // NOTE: -> 3.1.0: Migrates `State` to `QuickInfoState`
            if (!classification.QuickInfoState.HasValue && properties.TryGetValue("State", out var obj) && obj is long value)
            {
                var castedValue = (int)value;
                if (QuickInfoStateService.SupportedState.ContainsKey(castedValue))
                {
                    classification.QuickInfoState = castedValue;
                }
            }
        }
    }
}