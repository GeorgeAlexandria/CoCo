namespace CoCo.UI.Data
{
    public interface IOptionProvider
    {
        Option ReceiveOption();

        void ReleaseOption(Option option);
    }
}