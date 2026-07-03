using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpCandidateElementForWE : PickUpCandidateElementProc
{
    private WordEstimater _wordEstimater;

    public PickUpCandidateElementForWE(WordEmtCell wecLib)
    {
        _wordEstimater = new WordEstimater(wecLib, " ");
    }

    public List<string> CreateCandidates(string input)
    {
        return _wordEstimater.ReturnEstimatedStrs(input, 0);
    }
}
