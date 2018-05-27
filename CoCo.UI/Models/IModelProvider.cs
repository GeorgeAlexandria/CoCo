namespace CoCo.UI.Models
{
    public interface IModelProvider
    {
        OptionModel GetOption();

        void SaveOption(OptionModel option);
    }
}