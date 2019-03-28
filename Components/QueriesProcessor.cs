using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

public class QueriesProcessor : IQueriesProcessor
{    
    private SortedList<DateTime, IList<string>> _sortedQueries;

    public QueriesProcessor(string path)
    {
        _sortedQueries = ParseInputFile(path);
    }

    // O(n)
    public int GetCount(DateTime startDate, DateTime endDate)
    {
        int idx = GetFirstDateIndex(startDate, endDate);
        if(idx == -1)
            return 0;

        var seen = new HashSet<string>();
        while(idx < _sortedQueries.Count && _sortedQueries.Keys[idx] < endDate)
        {
            foreach(var query in _sortedQueries[_sortedQueries.Keys[idx]])
            {
                seen.Add(query);
            }
            ++idx;
        }

        return seen.Count;
    }

    // O(n + n * log(size)) = O(n) because size << n
    public IEnumerable<KeyValuePair<string, int>> GetPopular(DateTime startDate, DateTime endDate, int size)
    {
        int idx = GetFirstDateIndex(startDate, endDate);
        if (idx == -1)
            return new List<KeyValuePair<string, int>>();

        var dict = new Dictionary<string, int>();
        while (idx < _sortedQueries.Count && _sortedQueries.Keys[idx] < endDate)
        {
            foreach (var query in _sortedQueries[_sortedQueries.Keys[idx]])
            {
                if (!dict.TryAdd(query, 1))
                {
                    dict[query]++;
                };
            }
            ++idx;
        }

        return GetMostPopularQueries(dict, size);
    }

    private static IEnumerable<KeyValuePair<string,int>> GetMostPopularQueries(Dictionary<string, int> dict, int size)
    {
        // SortedList does not accept duplicate values. 
        // Overriding the comparator to never return equality solves this.
        var mostPopularQueries = new SortedList<int, string>(Comparer<int>.Create((int a, int b) => a <= b ? 1 : -1));

        foreach (var (k, v) in dict)
        {
            if (mostPopularQueries.Count < size){
                mostPopularQueries.Add(v, k);
            }
            else if (mostPopularQueries.Last().Key < v){
                mostPopularQueries.RemoveAt(size - 1);
                mostPopularQueries.Add(v, k);
            }
        }

        return mostPopularQueries.Select(kv => new KeyValuePair<string, int>(kv.Value, kv.Key));
    }

    private static SortedList<DateTime, IList<string>> ParseInputFile(string path)
    {
        var res = new SortedList<DateTime, IList<string>>();

        using(StreamReader sr = new StreamReader(path)){
            string line;
            while((line = sr.ReadLine()) != null){
                var split = line.Split("\t", 2);
                
                if(split.Length != 2 
                    || !DateTime.TryParse(split[0], CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)){
                    throw new ArgumentException($"Line badly formatted: {line}$");
                }

                var query = split[1];
                if (!res.TryAdd(date, new List<string>{query})){
                    res[date].Add(query);
                }
            }
        }
        return res;
    }

    // Dichotomic search to find the closest date above the startDate
    private int GetFirstDateIndex(DateTime startDate, DateTime endDate)
    {
        // Check if the date range is inbound. 
        if(startDate > _sortedQueries.Last().Key || endDate < _sortedQueries.First().Key){
            return -1;
        }

        int min = 0;
        int max = _sortedQueries.Count - 1;

        while(min <= max){
            int mid = (min + max) / 2;
            if(_sortedQueries.Keys[mid] == startDate){
                return mid;
            }

            if (startDate < _sortedQueries.Keys[mid]){
                max = mid - 1;
            }
            else {
                min = mid + 1;
            }
        }

        // The first date above the startDate.
        return min;
    }
}