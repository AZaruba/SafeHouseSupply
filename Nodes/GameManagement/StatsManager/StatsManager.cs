using System;
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
  public string NextLevel;
  public Array<LevelEnemyData> EnemyInfo;
  public Array<HouseItem> Items;

  public readonly int InventoryMax = 3;
  public int CurrentInventory = 0;

  public int TimesSpotted = 0;
  public int WantsCollected = 0;

  public int Wants;

  public string MissionDescription;

}

public partial class StatsManager : Node
{
  public static readonly Dictionary<LocationName, string> NameLookup = new Dictionary<LocationName, string> {
    {LocationName.NONE,"None"},
    {LocationName.SAFE_HOUSE, "SafeHouse"},
    {LocationName.BANK, "BANK"},
    {LocationName.TOOLS,"TOOLS"},
    {LocationName.TAILOR,"TAILOR"},
    {LocationName.GROCERY,"GROCER"},
    {LocationName.PHARMACY,"PHARMACY"},
    {LocationName.GIFTS,"GIFTS"}
  };

  public static StatsManager Instance;

  public string CurrentLevel = "Level1";
  public LevelData CurrentLevelData;

  public bool IsComplete = false;

  public override void _Ready()
  {
    Instance = this;
    Instance.IsComplete = false;
  }

  public static void LoadLevel(string LevelName)
  {
    if (!FileAccess.FileExists("res://Assets/LevelData/" + LevelName + ".json"))
    {
      // game is complete
      
    }
    using FileAccess levelFile = FileAccess.Open("res://Assets/LevelData/" + LevelName + ".json", FileAccess.ModeFlags.Read);
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

  private static Vector2 GetFacingDirection(string dir)
  {
    switch (dir)
    {
      case "Left":
        return Vector2.Left;
      case "Right":
        return Vector2.Right;
      case "Up":
        return Vector2.Up;
      case "Down":
        return Vector2.Down;
      default:
        return Vector2.Up;
    }
  }

  public static AgentInstruction[] ParsePatrolInstructions(Array<Dictionary> agentInstructions)
  {
    AgentInstruction[] instructionsList = new AgentInstruction[agentInstructions.Count];
    for (int i = 0; i < agentInstructions.Count; i++)
    {
      Dictionary instruction = agentInstructions[i];
      Vector2 targetPosition = new Vector2(
        instruction["TargetPosition"].AsGodotDictionary()["X"].AsInt64(),
        instruction["TargetPosition"].AsGodotDictionary()["Y"].AsInt64()
      );
      AgentInstruction newInstruction = new()
      {
        Destination = targetPosition,
        FacingDirection = GetFacingDirection(instruction["FacingDirection"].AsString()),
        Speed = instruction["Speed"].AsInt64(),
        WaitTime = instruction["WaitTime"].AsSingle()
      };

      instructionsList[i] = newInstruction;
    }

    return instructionsList;
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

    Array<Dictionary> agentDict = dict["Agents"].AsGodotArray<Dictionary>();
    Array<LevelEnemyData> enemyInfo = new();
    foreach (Dictionary agent in agentDict)
    {
      LevelEnemyData levelEnemyData = new()
      {
        PatrolInstructions = ParsePatrolInstructions(agent["PatrolInstructions"].AsGodotArray<Dictionary>()),
        StartPosition = new Vector2(
          agent["StartPosition"].AsGodotDictionary()["X"].AsInt64(),
          agent["StartPosition"].AsGodotDictionary()["Y"].AsInt64()
        )
      };

      enemyInfo.Add(levelEnemyData);
    }

    LevelData dataIn = new()
    {
      Time = (float)dict["Time"],
      AgentName = dict["AgentName"].AsString(),
      NextLevel = dict["NextLevel"].AsString(),
      EnemyInfo = enemyInfo,
      Items = houseItems,
      MissionDescription = dict["MissionDescription"].AsString()
    };

    Instance.CurrentLevelData = dataIn;
  }

  public static string WriteLevelData()
  {
    LevelData data = Instance.CurrentLevelData;
    string Output = "";
    data.Wants = 0;

    Output += "Agent:" + data.AgentName;
    Output += "\n\nNeeds: ";
    bool listingWants = false;
    foreach (HouseItem item in data.Items)
    {
      if (item.Type == "Want" && !listingWants)
      {
        listingWants = true;
        Output += "\n\nWants:";
      }
      Output += "\n\t- " + item.Name + "\n\t\tFind at " + item.Location;
    }

    Output += "\n\n" + data.MissionDescription;

    return Output;
  }

  public static bool IsLevelComplete()
  {
    foreach (HouseItem item in Instance.CurrentLevelData.Items)
    {
      if (item.Type.Equals("Need") && !item.Collected)
      {
        return false;
      }
    }

    // calc before counting score
    foreach (HouseItem item in Instance.CurrentLevelData.Items)
    {
      if (item.Type.Equals("Want"))
      {
        Instance.CurrentLevelData.Wants++;
      }
    }
    return true;
  }

  public static void OnShopEntered(int LocationID)
  {
    if (LocationID == (int)LocationName.SAFE_HOUSE)
    {
      Instance.CurrentLevelData.CurrentInventory = 0;
      ScoreDisplay.WriteString("ITEMS HELD: " + Instance.CurrentLevelData.CurrentInventory + "/" + Instance.CurrentLevelData.InventoryMax);
      if (IsLevelComplete())
      {
        Instance.IsComplete = true;
        Instance.CurrentLevel = Instance.CurrentLevelData.NextLevel;
      }
      return;
    }
    if (NameLookup.TryGetValue((LocationName)LocationID, out string lname))
    {
      foreach (HouseItem item in Instance.CurrentLevelData.Items)
      {
        if (item.Location.Equals(lname) &&
          item.Collected == false &&
          Instance.CurrentLevelData.CurrentInventory < Instance.CurrentLevelData.InventoryMax)
        {
          item.Collected = true;
          Instance.CurrentLevelData.CurrentInventory++;
          ScoreDisplay.WriteString("ITEMS HELD: " + Instance.CurrentLevelData.CurrentInventory + "/" + Instance.CurrentLevelData.InventoryMax);

          if (item.Type == "Want")
          {
            Instance.CurrentLevelData.WantsCollected++;
          }
        }
      }
    }
  }

  public static LevelEnemyData[] GetCurrentLevelEnemyData()
  {
    return [.. Instance.CurrentLevelData.EnemyInfo];
  }

  public static void OnReset()
  {
    foreach (HouseItem item in Instance.CurrentLevelData.Items)
    {
      item.Collected = false;
    }
    Instance.CurrentLevelData.CurrentInventory = 0;
    Instance.CurrentLevelData.TimesSpotted = 0;
    Instance.CurrentLevelData.WantsCollected = 0;
    Instance.IsComplete = false;
  }
}
