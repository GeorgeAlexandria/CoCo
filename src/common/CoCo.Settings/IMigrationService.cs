using System.Collections.Generic;

namespace CoCo.Settings
{
    public interface IMigrationService
    {
        /// <summary>
        /// Migrates <paramref name="classification"/> using additional <paramref name="properties"/>
        /// </summary>
        void MigrateClassification(
            IReadOnlyDictionary<string, object> properties, ref ClassificationSettings classification);

        ICollection<ClassificationSettings> MigrateClassifications(
            string language, ICollection<ClassificationSettings> classifications);
    }
}