namespace StickyNotes.Services
{
    using System.IO.IsolatedStorage;

    /// <summary>
    /// A generic settings manager.
    /// </summary>
    public abstract class SettingsManager
    {
        private static IsolatedStorageSettings GetIsolatedStorageSettings()
        {
            return IsolatedStorageSettings.ApplicationSettings;
        }

        public static void SaveSetting<T>(string key, T value)
        {
            var storage = GetIsolatedStorageSettings();

            if (!storage.Contains(key))
                storage.Add(key, value);
            else
                storage[key] = value;

            storage.Save();
        }

        public static T GetSetting<T>(string key, T defaultValue = default(T))
        {
            var storage = GetIsolatedStorageSettings();
            if (storage.Contains(key))
                return (T)storage[key];
            else
                return defaultValue;
        }
    }
}