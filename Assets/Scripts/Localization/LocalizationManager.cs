using Utils;

namespace Localization
{
    public class LocalizationManager : Singleton<LocalizationManager>
    {
        private void Awake()
        {
            foreach (var autoLocalizedString in AutoLocalizedString.allAutoLocalizedStrings)
            {
                autoLocalizedString.Init();
            }
        }
    }
}