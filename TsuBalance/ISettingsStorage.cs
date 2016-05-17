namespace TsuBalance
{
    using System;

    public interface ISettingsStorage
    {
        void Save(string login, string pass, bool autorun);

        Tuple<string, string, bool> Load();
    }
}