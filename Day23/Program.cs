using System.Collections.Immutable;

bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

Dictionary<string, HashSet<string>> edges = File
	.ReadAllLines(inputFilename)
	.Select(line => line.Split('-'))
	.SelectMany(item => new (string From, string To)[] { (item[0], item[1]), (item[1], item[0]) })
	.GroupBy(item => item.From)
	.Select(group => (From: group.Key, To: group.Select(item => item.To).ToHashSet()))
	.ToDictionary(item => item.From, item => item.To);

IEnumerable<List<string>> allCombinationsOfThreeNodes =
	from firstNode in edges.Keys
	from secondNode in edges[firstNode]
	from thirdNode in edges[secondNode]
	where edges[thirdNode].Contains(firstNode)
	select new[] { firstNode, secondNode, thirdNode }.Order().ToList();
IEnumerable<(string First, string Second, string Third)> distinctCombinations =
	(from nodes in allCombinationsOfThreeNodes
	 orderby nodes[0], nodes[1], nodes[2]
	 select (First: nodes[0], Second: nodes[1], Third: nodes[2]))
		.Distinct();
long resultPartA = distinctCombinations.Count(item => ("" + item.First[0] + item.Second[0] + item.Third[0]).Contains('t'));

List<ImmutableHashSet<string>> maximumCliques = new();
BronKerboshAlgorithm(
	ImmutableHashSet<string>.Empty,
	edges.Keys.ToImmutableHashSet(),
	ImmutableHashSet<string>.Empty,
	maximumCliques);
string resultPartB = string.Join(',', maximumCliques.MaxBy(item => item.Count)!.Order());

Console.WriteLine("Day 23 - LAN Party");
Console.WriteLine($"A: The number of sets containing at least one 't' is {resultPartA}."); // 7, 1302
Console.WriteLine($"B: The password is {resultPartB}."); // "co,de,ka,ta", "cb,df,fo,ho,kk,nw,ox,pq,rt,sf,tq,wi,xz"

void BronKerboshAlgorithm(
	ImmutableHashSet<string> r,
	ImmutableHashSet<string> p,
	ImmutableHashSet<string> x,
	List<ImmutableHashSet<string>> maximalCliques)
{
	if (p.IsEmpty && x.IsEmpty)
		maximalCliques.Add(r);
	else
	{
		foreach (string v in p)
		{
			BronKerboshAlgorithm(r.Add(v), p.Intersect(edges[v]), x.Intersect(edges[v]), maximalCliques);
			p = p.Remove(v);
			x = x.Add(v);
		}
	}
}
