using UnityEngine;
using UnityEngine.UI;

public enum SquareState
{
    Locked,
    Unlock,
    Close,
}
public enum SquareAnimType
{
    Click,
    Vertical,
    Horizontal
}
public class Square : MonoBehaviour
{
    [SerializeField]
    private Image image;
    [SerializeField]
    private Sprite blockImg, closeImg, lockedImg;
    private SquareAnimType animType;
    public SquareState State { get; private set; }
    public void SetState(SquareState state)
    {
        State = state;
        image.sprite = state switch
        {
            SquareState.Locked => lockedImg,
            SquareState.Unlock => blockImg,
            SquareState.Close => closeImg,
            _ => null
        };
    }
    public void SetError(bool error)
    {
        image.color = error ? Color.red : Color.white;
    }

    public void PlayAnim(SquareAnimType type, float delay)
    {
        animType = type;
        anim = -delay;
        transform.localScale = Vector3.one;
    }
    private float anim;
    private void Update()
    {
        if (anim < 0) anim += Time.deltaTime;
        else if (anim < 1)
        {
            switch (animType)
            {
                case SquareAnimType.Click:
                    {
                        anim += Time.deltaTime * 3;
                        var t = Mathf.Clamp01(anim);
                        var amplitude = (1 - t) * .3f;
                        var scale = Vector3.one;
                        scale.x += Mathf.Sin(t * Mathf.PI * 4) * amplitude;
                        scale.y -= Mathf.Sin(t * Mathf.PI * 4) * amplitude;
                        transform.localScale = scale;
                    }
                    break;
                case SquareAnimType.Vertical:
                    {
                        anim += Time.deltaTime * 2;
                        var t = Mathf.Clamp01(anim);
                        t = Mathf.Abs(Mathf.Cos(t * Mathf.PI));
                        transform.localScale = new Vector3(1, t, 1);
                    }
                    break;
                case SquareAnimType.Horizontal:
                    {
                        anim += Time.deltaTime * 2;
                        var t = Mathf.Clamp01(anim);
                        t = Mathf.Abs(Mathf.Cos(t * Mathf.PI));
                        transform.localScale = new Vector3(t, 1, 1);
                    }
                    break;
            }
        }
    }
}
