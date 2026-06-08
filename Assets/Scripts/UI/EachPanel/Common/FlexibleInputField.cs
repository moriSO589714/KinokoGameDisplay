using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// インプットフィールドのサイズを動的に制御するクラス
/// </summary>
public class FlexibleInputField
{
    private int _maxWordsPerLine;
    private float _inputFieldTxtPrefHeight;
    private Vector2 _firstFieldSize;
    private int _lastLineLength = 1;
    private InputField _myInputField;
    private RectTransform _myRectTransform;

    public FlexibleInputField(InputField inputField, int maxWordsPerLine)
    {
        _myInputField = inputField;
        _maxWordsPerLine = maxWordsPerLine;
        _myRectTransform = _myInputField.GetComponent<RectTransform>();
        _inputFieldTxtPrefHeight = _myInputField.textComponent.preferredHeight;
        _firstFieldSize = _myRectTransform.sizeDelta;
    }

    public void ChangeFieldSize(string inputTxt)
    {
        int inputTxtLength = inputTxt.Count();

        //改行記号が入っている際はその行の余り文字数を補完する
        string stack = "";
        foreach (char c in inputTxt)
        {
            if (c == '\n')
            {
                int lack = _maxWordsPerLine - stack.Count();
                inputTxtLength += lack;
                stack = "";

                continue;
            }
            else
            {
                stack += c;
            }

            if (stack.Count() == _maxWordsPerLine)
            {
                stack = "";
            }
        }

        //入力された文字数と1行に入る最大の文字数から列数を計算
        int lineLength = inputTxtLength / _maxWordsPerLine;
        if (inputTxtLength % _maxWordsPerLine != 0)
        {
            lineLength++;
        }

        //文字列数に変化がある場合、InputFieldのサイズを変更する
        if (lineLength == 0)
        {
            _myRectTransform.sizeDelta = _firstFieldSize;
        }
        else if (_lastLineLength != lineLength)
        {
            //増分または減少分を計算
            int changedLength = lineLength - _lastLineLength;
            //実際に増やす/減らす高さ
            float changeHeight = _inputFieldTxtPrefHeight * changedLength;

            //高さを変更
            _myRectTransform.sizeDelta = new Vector2(_myRectTransform.sizeDelta.x, _myRectTransform.sizeDelta.y + changeHeight);

            _lastLineLength = lineLength;
        }
    }
}
