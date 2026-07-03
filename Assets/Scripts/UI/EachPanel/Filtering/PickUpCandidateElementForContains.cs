using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 予測変換でWordEstimaterを利用せずに、任意の語が含まれている語を単語辞書から返す処理
/// (Ex.ゲームタイトルの予測変換)
/// </summary>
public class PickUpCandidateElementForContains : PickUpCandidateElementProc
{
    private List<string> _wordList = new List<string>();

    public PickUpCandidateElementForContains(List<string> wordLib)
    {
        _wordList = wordLib;
    }

    public List<string> CreateCandidates(string input)
    {
        List<string> resultList = _wordList.Where(x => x.Contains(input)).ToList();
        return resultList;
    }
}
