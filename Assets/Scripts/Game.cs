using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    private enum Direction
    {
        None,
        Vertical,
        Horizontal
    }
    [SerializeField]
    private RectTransform panel;
    [SerializeField]
    private GameObject victoryPanel;
    [SerializeField]
    private Square prefab;
    [SerializeField]
    private GridLayoutGroup gridLayoutGroup;
    [SerializeField]
    private CardNumberPanel verticalCard, horizontalCard;
    [SerializeField]
    private Coordinate verticalCoordinate, horizontalCoordinate;
    private Square[,] squares;
    private bool[,] values;

    [SerializeField]
    private SelectEffect selectEffect;
    private bool mouseDown;
    private Vector2Int startCoord, lastCoord;
    private Direction direction;
    private Vector2Int GetCoord(Vector2 mp)
    {
        for (var x = 0; x < squares.GetLength(0); x++)
            for (var y = 0; y < squares.GetLength(1); y++)
            {
                var rt = squares[x, y].transform as RectTransform;
                if (RectTransformUtility.RectangleContainsScreenPoint(rt, mp))
                    return new Vector2Int(x, y);
            }
        return new Vector2Int(-1, -1);
    }
    private void OnClickSquare(Vector2Int coord, bool lr)
    {
        var square = squares[coord.x, coord.y];
        if (square.State != SquareState.Locked) return;
        switch (direction)
        {
            case Direction.None:
                square.PlayAnim(SquareAnimType.Click, 0);
                break;
            case Direction.Vertical:
                square.PlayAnim(SquareAnimType.Vertical, 0);
                break;
            case Direction.Horizontal:
                square.PlayAnim(SquareAnimType.Horizontal, 0);
                break;
        }
        if (values[coord.x, coord.y])
        {
            square.SetState(SquareState.Unlock);
            if (!lr) square.SetError(true);
        }
        else
        {
            square.SetState(SquareState.Close);
            if (lr) square.SetError(true);
        }
        UpdateCardNumberEnable();
    }
    private void OnMouseButtonDown(Vector2Int coord, bool lr)
    {
        if (!mouseDown)
        {
            mouseDown = true;
            lastCoord = startCoord = coord;
            direction = Direction.None;
            if (startCoord.x < 0) return;
            OnClickSquare(startCoord, lr);
        }
        else if (startCoord.x >= 0)
        {
            if (coord.x < 0) return;
            if (coord != lastCoord)
            {
                if (direction == Direction.None)
                {
                    if (coord.x != lastCoord.x) direction = Direction.Horizontal;
                    else direction = Direction.Vertical;
                }
                if (direction == Direction.Horizontal) coord.y = startCoord.y;
                else coord.x = startCoord.x;

                OnClickSquare(coord, lr);

                lastCoord = coord;
            }
        }
    }
    private void Update()
    {
        if (squares == null) return;
        var coord = GetCoord(Input.mousePosition);
        if (Input.GetMouseButton(0)) OnMouseButtonDown(coord, true);
        else if (Input.GetMouseButton(1)) OnMouseButtonDown(coord, false);
        else mouseDown = false;
        selectEffect.gameObject.SetActive(coord.x >= 0);
        if (coord.x >= 0)
        {
            var square = squares[coord.x, coord.y];
            selectEffect.transform.position = square.transform.position;
            selectEffect.SetSize(coord.x, squares.GetLength(0) - coord.x, coord.y, squares.GetLength(1) - coord.y);
        }
    }


    public void ResetState()
    {
        if (squares == null) return;
        foreach (var square in squares)
        {
            square.SetState(SquareState.Locked);
            square.SetError(false);
        }
        verticalCard.DisableAllNumber();
        horizontalCard.DisableAllNumber();
    }
    public void LoadEasy()
    {
        Load(CreateData(10, 10, 80));
    }
    public void LoadHard()
    {
        Load(CreateData(15, 15, 180));
    }
    public void Reload()
    {
        if (values == null) return;
        var count = 0;
        foreach (var value in values)
            if (value) count++;
        Load(CreateData(values.GetLength(0), values.GetLength(1), count));
    }
    public void Load(bool[,] values)
    {
        this.values = values;
        if (squares != null)
        {
            foreach (var square in squares)
                Destroy(square.gameObject);
        }
        squares = new Square[values.GetLength(0), values.GetLength(1)];
        for (var y = 0; y < squares.GetLength(1); y++)
            for (var x = 0; x < squares.GetLength(0); x++)
            {
                var item = Instantiate(prefab.gameObject, transform).GetComponent<Square>();
                item.gameObject.SetActive(true);
                item.SetState(SquareState.Locked);
                item.SetError(false);
                item.PlayAnim(SquareAnimType.Click, (x + y) * .05f);
                squares[x, y] = item;
            }

        var verticalNumbers = new int[values.GetLength(1)][];
        for (var i = 0; i < verticalNumbers.Length; i++)
            verticalNumbers[i] = GetVerticalNumbers(i);
        var verticalSize = verticalCard.SetNumbers(verticalNumbers);

        var horizontalNumbers = new int[values.GetLength(0)][];
        for (var i = 0; i < horizontalNumbers.Length; i++)
            horizontalNumbers[i] = GetHorizontalNumbers(i);
        var horizontalSize = horizontalCard.SetNumbers(horizontalNumbers);

        verticalCoordinate.SetCount(verticalNumbers.Length);
        horizontalCoordinate.SetCount(horizontalNumbers.Length);

        gridLayoutGroup.constraintCount = values.GetLength(0);

        var size = new Vector2(verticalSize + horizontalNumbers.Length + 1, horizontalSize + verticalNumbers.Length + 1) * 100;
        var panelSize = panel.rect.size;
        size = new Vector2(panelSize.x / size.x, panelSize.y / size.y);
        var scale = MathF.Min(size.x, size.y);

        var rt = transform as RectTransform;
        rt.sizeDelta = new Vector2(values.GetLength(0), values.GetLength(1)) * 100;
        rt.localPosition = 100 * scale * new Vector3(verticalSize, -horizontalSize);
        rt.localScale = Vector3.one * scale;
    }
    private int[] GetVerticalNumbers(int index)
    {
        var list = new List<int>();
        var count = 0;
        for (var i = 0; i < values.GetLength(0); i++)
            if (values[i, index]) count++;
            else if (count > 0)
            {
                list.Add(count);
                count = 0;
            }
        if (count > 0) list.Add(count);
        return list.ToArray();
    }
    private int[] GetHorizontalNumbers(int index)
    {
        var list = new List<int>();
        var count = 0;
        for (var i = 0; i < values.GetLength(1); i++)
            if (values[index, i]) count++;
            else if (count > 0)
            {
                list.Add(count);
                count = 0;
            }
        if (count > 0) list.Add(count);
        return list.ToArray();
    }
    public void UpdateCardNumberEnable()
    {
        if (values == null) return;
        for (var i = 0; i < values.GetLength(1); i++)
            UpdateVerticalCardNumberEnable(i);
        for (var i = 0; i < values.GetLength(0); i++)
            UpdateHorizontalCardNumberEnable(i);

        for (var i = 0; i < values.GetLength(1); i++)
            if (!verticalCard.IsFinish(i))
                return;
        victoryPanel.SetActive(true);
    }
    private void UpdateVerticalCardNumberEnable(int index)
    {
        if (verticalCard.IsFinish(index)) return;
        var slot = 0; var value = false; var enable = true;
        for (var i = 0; i < values.GetLength(0); i++)
            if (values[i, index])
            {
                if (!value)
                {
                    value = true;
                    slot++;
                }
                if (enable && squares[i, index].State == SquareState.Locked)
                    enable = false;
            }
            else if (slot > 0)
            {
                if (enable && value) verticalCard.SetNumberEnable(index, slot - 1, true);
                enable = true;
                value = false;
            }
        if (slot > 0 && enable && value) verticalCard.SetNumberEnable(index, slot - 1, true);
        if (verticalCard.IsFinish(index))
        {
            for (var i = 0; i < values.GetLength(0); i++)
            {
                var square = squares[i, index];
                square.PlayAnim(SquareAnimType.Horizontal, i * .025f);
                if (square.State == SquareState.Locked && !values[i, index])
                    square.SetState(SquareState.Close);
            }
        }
    }
    private void UpdateHorizontalCardNumberEnable(int index)
    {
        if (horizontalCard.IsFinish(index)) return;
        var slot = 0; var value = false; var enable = true;
        for (var i = 0; i < values.GetLength(1); i++)
            if (values[index, i])
            {
                if (!value)
                {
                    value = true;
                    slot++;
                }
                if (enable && squares[index, i].State == SquareState.Locked)
                    enable = false;
            }
            else if (slot > 0)
            {
                if (enable && value) horizontalCard.SetNumberEnable(index, slot - 1, true);
                enable = true;
                value = false;
            }
        if (slot > 0 && enable && value) horizontalCard.SetNumberEnable(index, slot - 1, true);
        if (horizontalCard.IsFinish(index))
        {
            for (var i = 0; i < values.GetLength(1); i++)
            {
                var square = squares[index, i];
                square.PlayAnim(SquareAnimType.Vertical, i * .025f);
                if (square.State == SquareState.Locked && !values[index, i])
                    square.SetState(SquareState.Close);
            }
        }
    }
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public static bool[,] CreateData(int width, int height, int fill)
    {
        var array = new bool[width * height];
        for (var i = fill - 1; i >= 0; i--)
        {
            array[i] = true;
            var index = UnityEngine.Random.Range(i, array.Length);
            (array[i], array[index]) = (array[index], array[i]);
        }

        var values = new bool[width, height];
        for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
                values[x, y] = array[x * height + y];

        return values;
    }
}
