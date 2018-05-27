namespace CoCo.UI.Models
{
    public interface IModelProvider
    {
        IOptionModel GetOption();

        void SaveOption(IOptionModel option);
    }
}