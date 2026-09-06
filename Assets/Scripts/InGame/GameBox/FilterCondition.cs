using System.Collections.Generic;


public class FilterCondition
{
    public readonly string[] _filteringCategory = new string[5] { "status", "title", "tag", "developper", "softs" };

    public List<GameStatus> Statuses { get; private set; } = new List<GameStatus>();
    public List<List<string>> GameNames { get; private set; } = new List<List<string>>();
    public List<List<string>> GameTags { get; private set; } = new List<List<string>>();
    public List<List<string>> GameDevs { get; private set; } = new List<List<string>>();
    public List<string> Softs { get; private set; } = new List<string>();

    public FilterCondition(List<GameStatus> statuses, List<List<string>> names, List<List<string>> tags, List<List<string>> devs, List<string> softs)
    {
        Statuses = statuses;
        GameNames = names;
        GameTags = tags;
        GameDevs = devs;
        Softs = softs;
    }
    public FilterCondition()
    {

    }

    public void SetStatuses(List<GameStatus> statuses)
    {
        Statuses = statuses;
    }

    public void SetGameNames(List<List<string>> names)
    {
        GameNames = names;
    }

    public void SetGameTag(List<List<string>> tags)
    {
        GameTags = tags;
    }

    public void SetGameDevs(List<List<string>> devs)
    {
        GameDevs = devs;
    }

    public void SetSofts(List<string> softs)
    {
        Softs= softs;
    }
}

