using System.Collections.Immutable;

bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

string[] lines = File.ReadAllLines(inputFilename);
IEnumerable<(int Before, int After)> rules = lines
	.Where(item => item.Contains('|'))
	.Select(item => item.Split('|').Select(int.Parse).ToList())
	.Select(item => (Before: item.First(), After: item.Last()));
List<ImmutableList<int>> printOrderInput = lines
	.Where(item => item.Contains(','))
	.Select(item => item.Split(',').Select(int.Parse).ToImmutableList())
	.ToList();
Func<ImmutableList<int>, bool> printOrderFulfillsRules = printOrder =>
	rules
		.All(item =>
			!(printOrder.Contains(item.Before) && printOrder.Contains(item.After)) ||
			printOrder.IndexOf(item.Before) < printOrder.IndexOf(item.After));
int resultPartA = printOrderInput
	.Where(printOrderFulfillsRules)
	.Sum(item => item[item.Count / 2]);
int resultPartB = printOrderInput
	.Where(item => !printOrderFulfillsRules(item))
	.Select(item => OrderWrongInputReversed(item).Reverse().ToList())
	.Sum(item => item[item.Count / 2]);

Console.WriteLine("Day 5 - Print Queue");
Console.WriteLine($"A: Sum of middle pages of correctly ordered printouts is {resultPartA}."); // 143, 5275
Console.WriteLine($"B: Sum of middle pages of ordered incorrectly ordered printouts is {resultPartB}."); // 123, 6191

IEnumerable<int> OrderWrongInputReversed(ImmutableList<int> input)
{
	while (input.Count > 0)
	{
		int page = input.First(page => !rules.Any(rule => rule.Before == page && input.Contains(rule.After)));
		input = input.Remove(page);

		yield return page;
	}
}
