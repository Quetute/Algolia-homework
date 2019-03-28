using System;
using System.Collections.Generic;

public interface IQueriesProcessor
{
    int GetCount(DateTime startDate, DateTime endDate);

    IEnumerable<KeyValuePair<string, int>> GetPopular(DateTime startDate, DateTime endDate, int size);

}