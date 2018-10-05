using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text.Classification;

namespace CoCo.Analyser
{
    public delegate void ClassificationChangedEventHandler(ClassificationChangedEventArgs args);

    public class ClassificationChangedEventArgs : EventArgs
    {
        public ClassificationChangedEventArgs(
            IReadOnlyDictionary<IClassificationType, ClassificationInfo> changedClassifications)
        {
            ChangedClassifications = changedClassifications;
        }

        public IReadOnlyDictionary<IClassificationType, ClassificationInfo> ChangedClassifications;
    }
}