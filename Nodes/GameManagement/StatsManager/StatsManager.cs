using Godot;
using Godot.Collections;

public partial class LevelEnemyData: GodotObject
{
  public Vector2 StartPosition;
  public AgentInstruction[] PatrolInstructions;
}

public partial class HouseItem : GodotObject
{
  public string Location;
  public string Name;
  public string Description;
  public string Type;

  public bool Collected = false;
}

public partial class LevelData : GodotObject
{
  public float Time;
  public string AgentName;
  public Array<LevelEnemyData> EnemyInfo;
  public Array<HouseItem> Items;

  public string MissionDescription;

}

public partial class StatsManager : Node
{
  public static StatsManager Instance;

  public string CurrentLevel;
  public LevelData CurrentLevelData;

  public override void _Ready()
  {
    Instance = this;
  }

  public static void LoadLevel(string LevelName)
  {
    if (!FileAccess.FileExists("res://Assets/LevelData/" + LevelName + ".json"))
    {
      // 
    }
    using FileAccess levelFile = FileAccess.Open("res://Assets/LevelData/Level1.json", FileAccess.ModeFlags.Read);
    Json jsonResult = new();
    string jsonString = levelFile.GetAsText();
    Error result = jsonResult.Parse(jsonString);
    if (result == Error.Ok)
    {
      // woo woo lets go
      Dictionary dict = jsonResult.Data.AsGodotDictionary();
      ParseLevelData(dict);
    }
  }

  public static void ParseLevelData(Dictionary dict)
  {
    Array<Dictionary> itemDict = dict["Items"].AsGodotArray<Dictionary>();
    Array<HouseItem> houseItems = new();
    foreach (Dictionary item in itemDict)
    {
      HouseItem houseItem = new()
      {
        Description = item["Description"].AsString(),
        Location = item["Location"].AsString(),
        Name = item["Name"].AsString(),
        Type = item["Type"].AsString()
      };

      houseItems.Add(houseItem);
    }

    LevelData dataIn = new()
    {
      Time = (float)dict["Time"],
      AgentName = dict["AgentName"].AsString(),
      EnemyInfo = dict["Agents"].AsGodotArray<LevelEnemyData>(),
      Items = houseItems,
      MissionDescription = dict["MissionDescription"].AsString()
    };

    Instance.CurrentLevelData = dataIn;
  }

  public static string WriteLevelData()
  {
    LevelData data = Instance.CurrentLevelData;
    string Output = "";

    Output += "Agent:" + data.AgentName;
    Output += "\n\nNeeds: ";
    foreach (HouseItem item in data.Items)
    {
      if (item.Type == "Want")
      {
        Output += "\n\nWants:";
      }
      Output += "\n\t- " + item.Name + "\n\t\tFind at " + item.Location;
    }

    Output += "\n\n" + data.MissionDescription;

    return Output;
  }

  public static void OnItemCollected(string Location)
  {
    foreach (HouseItem item in Instance.CurrentLevelData.Items)
    {
      if (item.Location.Equals(Location))
      {
        item.Collected = true;
      }
    }
  }
}
