using System;
using System.IO;

namespace TimelessEmulator;

public static  class Settings
{

    private const string DATA_DIRECTORY_NAME = "data";

    private const string ALTERNATE_PASSIVE_ADDITIONS_FILE_NAME = "alternate_passive_additions.json";
    private const string ALTERNATE_PASSIVE_SKILLS_FILE_NAME = "alternate_passive_skills.json";
    private const string ALTERNATE_TREE_VERSIONS_FILE_NAME = "alternate_tree_versions.json";
    private const string PASSIVE_SKILLS_FILE_NAME = "passive_skills.json";
    private const string STATS_FILE_NAME = "stats.json";

    public static readonly string ApplicationName = "TimelessEmulator";
    public static readonly Version ApplicationVersion = new Version(1, 0);

    public static readonly string BaseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
    public static readonly string DataDirectoryPath = Path.Combine(BaseDirectoryPath, DATA_DIRECTORY_NAME);

    public static readonly string AlternatePassiveAdditionsFilePath = Path.Combine(DataDirectoryPath, ALTERNATE_PASSIVE_ADDITIONS_FILE_NAME);
    public static readonly string AlternatePassiveSkillsFilePath = Path.Combine(DataDirectoryPath, ALTERNATE_PASSIVE_SKILLS_FILE_NAME);
    public static readonly string AlternateTreeVersionsFilePath = Path.Combine(DataDirectoryPath, ALTERNATE_TREE_VERSIONS_FILE_NAME);
    public static readonly string PassiveSkillsFilePath = Path.Combine(DataDirectoryPath, PASSIVE_SKILLS_FILE_NAME);
    public static readonly string StatsFilePath = Path.Combine(DataDirectoryPath, STATS_FILE_NAME);

    static Settings()
    {

    }

}
