using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardNumberSlot : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI prefab;
    private readonly List<TextMeshProUGUI> list = new();
    [SerializeField]
    private bool isVertical;
    private bool[] enables;
    public bool Finish
    {
        get
        {
            foreach (var value in enables)
                if (!value)
                    return false;
            return true;
        }
    }
    public void SetCount(int count)
    {
        var rt = transform as RectTransform;
        var sd = rt.sizeDelta;
        if (isVertical) sd.x = 100 * count;
        else sd.y = 100 * count;
        rt.sizeDelta = sd;
    }
    public void SetNumbers(int[] numbers)
    {
        while (list.Count < numbers.Length)
            list.Add(Instantiate(prefab.gameObject, transform).GetComponent<TextMeshProUGUI>());
        while (list.Count > numbers.Length)
        {
            Destroy(list[^1].gameObject);
            list.RemoveAt(list.Count - 1);
        }
        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];
            item.transform.SetAsLastSibling();
            item.gameObject.SetActive(true);
            item.text = numbers[i].ToString();
            item.color = Color.white;
        }
        enables = new bool[list.Count];
    }
    public void SetNumberEnable(bool enable)
    {
        for (int i = 0; i < list.Count; i++) SetNumberEnable(i, enable);
    }
    public void SetNumberEnable(int index, bool enable)
    {
        list[index].color = enable ? Color.cyan : Color.white;
        enables[index] = enable;
    }
    public bool IsNumberEnabled(int index) { return enables[index]; }
}
