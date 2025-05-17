using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomPanel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI widthText, heightText, fillingText;
    [SerializeField]
    private Slider width, height, filling;
    [SerializeField]
    private Game game;
    private void OnEnable()
    {
        RefreshValue();
    }
    public void RefreshValue()
    {
        widthText.text = width.value.ToString();
        heightText.text = height.value.ToString();
        fillingText.text = ((int)(filling.value * 100)).ToString() + "%";
    }
    public void OnOKClick()
    {
        game.Load(Game.CreateData((int)width.value, (int)height.value, (int)(width.value * height.value * filling.value)));
    }
}
