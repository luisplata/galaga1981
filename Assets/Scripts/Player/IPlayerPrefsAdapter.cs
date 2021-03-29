public interface IPlayerPrefsAdapter
{
    bool HasKey(string name);
    int GetInt(string name);
    void SetInt(string name, int input);
}