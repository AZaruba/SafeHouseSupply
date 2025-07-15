
using System;

public class ListManager
{
  public HouseItem[] Items;
  public bool[] ItemStatuses;

  public ListManager(HouseItem[] Items)
  {
    this.Items = Items;
    ItemStatuses = new bool[Items.Length];
  }

  public void OnCollectItem(int index)
  {
    if (index < 0 || index >= Items.Length)
    {
      return;
    }

    ItemStatuses[index] = true;
  }
}