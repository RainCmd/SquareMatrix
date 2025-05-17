using UnityEngine;
using UnityEngine.UI;

public class SelectEffect : MonoBehaviour
{
    [SerializeField]
    private RectTransform left, right, top, bottom;
    [SerializeField]
    private Image[] images;
    public void SetSize(int left, int right, int top, int bottom)
    {
        var sd = this.left.sizeDelta;
        sd.x = left * 100;
        this.left.sizeDelta = sd;

        sd = this.right.sizeDelta;
        sd.x = right * 100;
        this.right.sizeDelta = sd;

        sd = this.top.sizeDelta;
        sd.y = top * 100;
        this.top.sizeDelta = sd;

        sd = this.bottom.sizeDelta;
        sd.y = bottom * 100;
        this.bottom.sizeDelta = sd;
    }
    private void Update()
    {
        var alpha = Mathf.Sin(Time.time * Mathf.PI * 2) * .25f+.5f;
        foreach (var image in images)
            image.color = new Color(1, 1, 1, alpha);
    }
}
