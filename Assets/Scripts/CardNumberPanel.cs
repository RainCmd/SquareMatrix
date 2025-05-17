using System.Collections.Generic;
using UnityEngine;

public class CardNumberPanel : MonoBehaviour
{
    [SerializeField]
    private CardNumberSlot prefab;
    private readonly List<CardNumberSlot> list = new();
    [SerializeField]
    private bool isVertical;
    public int SetNumbers(int[][] numbers)
    {
        while (list.Count < numbers.Length)
            list.Add(Instantiate(prefab.gameObject, transform).GetComponent<CardNumberSlot>());
        while (list.Count > numbers.Length)
        {
            Destroy(list[^1].gameObject);
            list.RemoveAt(list.Count - 1);
        }
        var max = 0;
        foreach (var value in numbers) max = Mathf.Max(value.Length, max);
        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];
            item.transform.SetAsLastSibling();
            item.gameObject.SetActive(true);
            item.SetCount(max);
            item.SetNumbers(numbers[i]);
            item.SetNumberEnable(false);
        }

        var rt = transform as RectTransform;
        var sd = rt.sizeDelta;
        if (isVertical) sd.x = 100 * max;
        else sd.y = 100 * max;
        rt.sizeDelta = sd;

        return max;
    }
    public bool IsFinish(int index)
    {
        return list[index].Finish;
    }
    public bool IsNumberEnable(int index, int slot)
    {
        return list[index].IsNumberEnabled(slot);
    }
    public void DisableAllNumber()
    {
        foreach (var item in list) item.SetNumberEnable(false);
    }
    public void SetNumberEnable(int index, int slot, bool enable)
    {
        list[index].SetNumberEnable(slot, enable);
    }
}
