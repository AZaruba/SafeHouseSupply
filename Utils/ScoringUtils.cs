
using Godot;

public class ScoringUtils
{

  public static string ScoreToRank(int Score, int factor = 1)
  {
    if (Score > 200 * factor)
    {
      return "S";
    }
    else if (Score> 160 * factor)
    {
      return "A";
    }
    else if (Score > 130 * factor)
    {
      return "B";
    }
    else if (Score > 100 * factor)
    {
      return "C";
    }
    else
    {
      return "D";
    }
  }

  public static int CalculateScore()
  {

    int WantsListed = StatsManager.Instance.CurrentLevelData.Wants;
    int WantsCollected = StatsManager.Instance.CurrentLevelData.WantsCollected;
    int TimesSpotted = StatsManager.Instance.CurrentLevelData.TimesSpotted;

    return Mathf.CeilToInt((WantsCollected / WantsListed * 100) +
                            (100 - Mathf.Min(TimesSpotted, 5) * 20) +
                            (Timer.CurrentTime / Timer.LevelTime * 100));
  }

  public static string CalculateRank()
  {
    int WantsListed = StatsManager.Instance.CurrentLevelData.Wants;
    int WantsCollected = StatsManager.Instance.CurrentLevelData.WantsCollected;
    int TimesSpotted = StatsManager.Instance.CurrentLevelData.TimesSpotted;

    int FinalScore = Mathf.CeilToInt((WantsCollected / WantsListed * 100) +
                                    (100 - Mathf.Min(TimesSpotted, 5) * 20) +
                                    (Timer.CurrentTime / Timer.LevelTime * 100));

    return ScoreToRank(FinalScore);
  }

  public static string FormatMultiDigitString(int number)
  {
    if (number < 10)
    {
      return "  " + number.ToString();
    }
    else if (number < 100)
    {
      return " " + number.ToString();
    }
    return number.ToString();
  }
}
