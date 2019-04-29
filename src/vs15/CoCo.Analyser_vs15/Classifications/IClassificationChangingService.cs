namespace CoCo.Analyser.Classifications
{
    internal interface IClassificationChangingService
    {
        event ClassificationChangedEventHandler ClassificationChanged;
    }
}