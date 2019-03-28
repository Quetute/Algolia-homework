# Launch the app

1. Download .Net Core runtime and SDK [here](https://dotnet.microsoft.com/download)
2. Add your tsv file and specify the path in the `appsettings.json`
3. Build and publish the solution: `dotnet publish -r [Runtime] -c Release`  and target the runtime you want ([list here](https://github.com/dotnet/docs/blob/master/docs/core/rid-catalog.md)).
4. Launch the solution: `./bin/Release/netcoreapp2.2/[Runtime]/Algolia.exe`.
5. Make queries on your browser or preferred tools on `http://localhost:5000` and/or `https://localhost:5001`

# Design
 
The algorithm is simple. We first read the tsv file and create a list, sorted by datetime and stored in memory. When we receive a call on the `count` or the `popular` API, we just look for the first date that is inside the datePrefix and iterate over the list from here until we leave the datePrefix.

For `count`, we add elements to a structure that guarantees uniqueness. We returns the size of this structure at the end of the iteration.

For `popular`, we use a dictionary to count the number of each queries and return the top `size` at the end of the iteration.

Both of those API run in `O(n)`, `n` being the number of queries done during the specified date range.


# Improvement

This works well because the input data fits in memory. For bigger input, we could consider some kind of MapReduce algorithm, or probabilistic algorithm if an approximation is good enough (like `HyperLogLog` for example).