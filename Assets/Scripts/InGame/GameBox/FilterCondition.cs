using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;


public class FilterCondition
{
    public readonly string[] _filteringCategory = new string[6] { "status", "title", "tag", "developper", "id","softs" };

    public List<GameStatus> Statuses = new List<GameStatus>();
    public List<List<string>> GameNames = new List<List<string>>();
    public List<List<string>> GameTags = new List<List<string>>();
    public List<List<string>> GameDevs = new List<List<string>>();
    public List<List<string>> GameIds = new List<List<string>>();
    public List<string> Softs = new List<string>();

    public FilterCondition(List<GameStatus> statuses, List<List<string>> names, List<List<string>> tags, List<List<string>> devs, List<List<string>> ids, List<string> softs)
    {
        Statuses = statuses;
        GameNames = names;
        GameTags = tags;
        GameDevs = devs;
        GameIds = ids;
        Softs = softs;
    }
    public FilterCondition()
    {

    }
    
    public string ReturnValueForCategory(string categoryName)
    {
        if (categoryName == _filteringCategory[0])
        {
            string returnval = "";
            foreach(GameStatus status in Statuses)
            {
                if(returnval != "")
                {
                    returnval += ",";
                }
                returnval += status.ToString();
            }
            return returnval;
        }
        else if(categoryName == _filteringCategory[1])
        {
            return MergeWList(GameNames);
        }
        else if (categoryName == _filteringCategory[2])
        {
            return MergeWList(GameTags);
        }
        else if (categoryName == _filteringCategory[3])
        {
            return MergeWList(GameDevs);
        }
        else if(categoryName == _filteringCategory[4])
        {
            return MergeWList(GameIds);
        }
        else if(categoryName == _filteringCategory[5])
        {
            return MergeWList(new List<List<string>> { Softs });
        }
        else
        {
            throw new System.Exception("存在しないカテゴリー名です");
        }
    }

    private string MergeWList(List<List<string>> wList)
    {
        string resultValue = "";
        foreach (List<string> list in wList)
        {
            if(resultValue != "")
            {
                resultValue += "or";
            }

            foreach(string s in list)
            {
                if(resultValue != "")
                {
                    resultValue += ",";
                }
                resultValue += s;
            }
        }

        return resultValue;
    }
}