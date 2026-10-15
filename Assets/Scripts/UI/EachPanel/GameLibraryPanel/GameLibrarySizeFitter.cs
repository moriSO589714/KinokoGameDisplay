using UnityEngine;
using UnityEngine.UI;

/// <summary>
///GameLibraryパネルのメイン部のUI位置を自動で調整する
/// </summary>
public class GameLibrarySizeFitter : MonoBehaviour
{
    [SerializeField] private float _ceilingSpace = 0;
    [SerializeField] private float _defaultSpace = 0;

    [SerializeField] private Text _titleText;
    [SerializeField] private Text _devNameText;
    [SerializeField] private GameObject _lineObj;
    [SerializeField] private Text _tagsText;

    [SerializeField] private GameObject _main;
    [SerializeField] private Text _description;
    [SerializeField] private Image _thumbnailImage;
    [SerializeField] private GameObject _moreInfoBlock;

    public Vector2 _currentFloorPos { get; private set; } = new Vector2();

    public Vector2 AutoFit()
    {
        Vector2 headderFloorPos = FitHeader();
        Vector2 floorPos = FitMain(headderFloorPos);
        _currentFloorPos = floorPos;
        return floorPos;
    }

    private Vector2 FitHeader()
    {
        //始点
        Vector2 startPos = Vector2.zero;

        //タイトルのテキストオブジェクトの座標
        RectTransform titleRect = _titleText.GetComponent<RectTransform>();
        Vector2 titlePos = new Vector2(startPos.x, startPos.y - _ceilingSpace);
        float titleHeight = CalcHeight(_titleText.gameObject);
        titleRect.anchoredPosition = titlePos;

        //開発者名
        RectTransform devRect = _devNameText.GetComponent<RectTransform>();        
        Vector2 devPos = new Vector2(devRect.anchoredPosition.x, titlePos.y - titleHeight - _defaultSpace);
        float devHeight = CalcHeight(_devNameText.gameObject);
        devRect.anchoredPosition = devPos;

        //区切り線
        RectTransform lineRect = _lineObj.GetComponent<RectTransform>();
        Vector2 linePos = new Vector2(startPos.x, devPos.y - devHeight - _defaultSpace);
        float lineHeight = CalcHeight(_lineObj);
        lineRect.anchoredPosition = linePos;

        //タグ
        RectTransform tagFieldRect = _tagsText.GetComponent<RectTransform>();
        Vector2 tagFieldPos = new Vector2(tagFieldRect.anchoredPosition.x, linePos.y - lineHeight - _defaultSpace);
        float tagFieldHeight = CalcHeight(_tagsText.gameObject);
        tagFieldRect.anchoredPosition = tagFieldPos;

        //Header部分の一番下の座標をGameLibrarySizeFitterが置かれている層のRectTransformに変換して返す
        Vector2 floorRectPos = new Vector2(0, tagFieldPos.y - tagFieldHeight);
        Vector2 floorCommonPos = ChangeUpperLayer(floorRectPos);
        return floorCommonPos;
    }

    private Vector2 FitMain(Vector2 startCommonPos)
    {
        //始点
        Vector2 startPos = ChangeLowerLayer(startCommonPos, _main);

        //説明部分
        RectTransform descriptionRect = _description.GetComponent<RectTransform>();
        Vector2 descriptionPos = new Vector2(descriptionRect.anchoredPosition.x, startPos.y - _defaultSpace);
        float descriptionHeight = CalcHeight(descriptionRect.gameObject);
        descriptionRect.anchoredPosition = descriptionPos;

        //サムネイル部分
        RectTransform thumbnailRect = _thumbnailImage.GetComponent<RectTransform>();
        Vector2 thumbnailPos = new Vector2(thumbnailRect.anchoredPosition.x, descriptionPos.y - descriptionHeight - _defaultSpace);
        float thumbnailHeight = CalcHeight(thumbnailRect.gameObject);
        thumbnailRect.anchoredPosition = thumbnailPos;

        //詳細情報部分
        RectTransform moreInfoBlockRect = _moreInfoBlock.GetComponent<RectTransform>();
        Vector2 moreInfoBlockPos = new Vector2(moreInfoBlockRect.anchoredPosition.x, thumbnailPos.y - thumbnailHeight - _defaultSpace);
        float moreInfoHeight = CalcHeight(moreInfoBlockRect.gameObject);
        moreInfoBlockRect.anchoredPosition = moreInfoBlockPos;

        //底辺の座標を計算して返す
        Vector2 floorRectPos = new Vector2(0, moreInfoBlockPos.y - moreInfoHeight);
        Vector2 floorCommonPos = ChangeUpperLayer(floorRectPos);
        return floorCommonPos;
    }

    private float CalcHeight(GameObject targetObj)
    {
        RectTransform targetRect = targetObj.GetComponent<RectTransform>();
        //高さ
        float sizeDeltaHeight = targetRect.sizeDelta.y;
        //scale分を修正して返す
        return sizeDeltaHeight * targetRect.localScale.y;
    }

    private Vector2 ChangeUpperLayer(Vector2 lowerPos)
    {
        //自身のanchoredPosを足すことで求められる
        return lowerPos + this.GetComponent<RectTransform>().anchoredPosition;
    }

    private Vector2 ChangeLowerLayer(Vector2 upperPos, GameObject setMotherObj)
    {
        //対象となるオブジェクトのanchoredPositionを引くことで求められる
        return upperPos - setMotherObj.GetComponent<RectTransform>().anchoredPosition;
    }
}
