using System.Collections.Generic;

public interface PickUpCandidateElementProc
{
    //予測単語の取得処理
    List<string> CreateCandidates(string input);
}
