using System.Collections.Immutable;

bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

List<ImmutableList<int>> matrix = File
	.ReadAllLines(inputFilename)
	.Select(line => line.Split(' ').Select(int.Parse).ToImmutableList())
	.ToList();
Func<ImmutableList<int>, IEnumerable<int>> diff = row =>
	row.Select((item, index) => row[index + 1] - item).Take(row.Count - 1);
Func<ImmutableList<int>, bool> acceptableIncrease = row => diff(row).All(item => item is >= 1 and <= 3);
Func<ImmutableList<int>, bool> acceptableDecrease = row => diff(row).All(item => item is >= -3 and <= -1);
int numberOfSafeReports = matrix.Count(row => acceptableIncrease(row) || acceptableDecrease(row));
Func<ImmutableList<int>, bool> acceptableAfterDampening = row => Enumerable
	.Range(0, row.Count)
	.Select(row.RemoveAt)
	.Any(rowWithLevelRemoved => acceptableIncrease(rowWithLevelRemoved) || acceptableDecrease(rowWithLevelRemoved));
int numberOfSafeReportsWithDampener = matrix.Count(row =>
	acceptableIncrease(row) ||
	acceptableDecrease(row) ||
	acceptableAfterDampening(row));

Console.WriteLine("Day 2");
Console.WriteLine($"A: Number of safe reports is {numberOfSafeReports}."); // 2, 463
Console.WriteLine($"B: Number of safe reports with dampener is {numberOfSafeReportsWithDampener}."); // 4, 514
