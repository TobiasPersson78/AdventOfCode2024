bool useExampleInput = true;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

var inputLines = File.ReadAllLines(inputFilename);
Dictionary<char, List<(char Character, int Row, int Column)>> cellsForNodeType = inputLines
	.SelectMany((line, rowIndex) =>
		line.Select((item, columnIndex) => (Character: item, Row: rowIndex, Column: columnIndex)))
	.Where(item => item.Character != '.')
	.ToLookup(item => item.Character)
	.Select(item => (item.Key, Values: item.ToList()))
	.ToDictionary(item => item.Key, item => item.Values);
int maxRow = inputLines.Length - 1;
int maxColumn = inputLines[0].Length - 1;
Func<bool, IList<(int Row, int Column)>> getDistinctAntiNodes = (bool useHarmonics) =>
	cellsForNodeType
		.SelectMany(keyValuePair =>
			keyValuePair.Value
				.SelectMany((first, index) =>
					keyValuePair.Value.Skip(index + 1).Select(second => (First: first, Second: second))))
		.SelectMany(pair =>
			(useHarmonics ? Enumerable.Range(0, Math.Max(maxRow, maxColumn)) : Enumerable.Repeat(1, 1))
			.SelectMany(times => new[]
			{
				(Row: pair.First.Row - (pair.Second.Row - pair.First.Row) * times,
					Column: pair.First.Column - (pair.Second.Column - pair.First.Column) * times),
				(Row: pair.Second.Row + (pair.Second.Row - pair.First.Row) * times,
					Column: pair.Second.Column + (pair.Second.Column - pair.First.Column) * times)
			}))
		.Where(item =>
			item.Row >= 0 && item.Row <= maxRow &&
			item.Column >= 0 && item.Column <= maxColumn)
		.Distinct()
		.ToList();

Console.WriteLine("Day 8");
Console.WriteLine($"A: Unique antinodes locations: {getDistinctAntiNodes(false).Count}"); // 14, 276
Console.WriteLine($"B: Unique antinodes locations considering harmonics: {getDistinctAntiNodes(true).Count}."); // 34, 991
