using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Coordinate : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI prefab;
    private readonly List<TextMeshProUGUI> list = new();
    public void SetCount(int count)
    {
        while (list.Count < count)
            list.Add(Instantiate(prefab.gameObject, transform).GetComponent<TextMeshProUGUI>());
        while (list.Count > count)
        {
            Destroy(list[^1].gameObject);
            list.RemoveAt(list.Count - 1);
        }
        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];
            item.transform.SetAsLastSibling();
            item.gameObject.SetActive(true);
            item.text = (i + 1).ToString();
        }
    }
}
