using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Analyser.Classifications
{
    public delegate void ClassificationChangedEventHandler(ClassificationsChangedEventArgs args);

    public class ClassificationsChangedEventArgs : EventArgs
    {
        public ClassificationsChangedEventArgs(
            IReadOnlyDictionary<IClassificationType, ClassificationOption> changedClassifications)
        {
            ChangedClassifications = changedClassifications;
        }

        public IReadOnlyDictionary<IClassificationType, ClassificationOption> ChangedClassifications;
    }
}