bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

List<string> map = File
	.ReadAllLines(inputFilename)
	.ToList();
IList<(int Row, int Column)> trailHeads =
	(from row in Enumerable.Range(0, map.Count)
	 from column in Enumerable.Range(0, map[0].Length)
	 where map[row][column] == '0'
	 select (row, column))
		.ToList();
int resultPartA = trailHeads.Sum(item => ReachableTopPositions(item).Distinct().Count());
int resultPartB = trailHeads.Sum(item => ReachableTopPositions(item).Count());

Console.WriteLine("Day 10");
Console.WriteLine($"A: Sum of number of reachable tops is {resultPartA}."); // 36, 822
Console.WriteLine($"B: Sum of trail ratings is {resultPartB}."); // 81, 1801

IEnumerable<(int Row, int Column)> ReachableTopPositions((int Row, int Column) position) =>
	map[position.Row][position.Column] == '9'
		? [position]
		: new (int DeltaRow, int DeltaColumn)[] { (0, 1), (1, 0), (0, -1), (-1, 0) }
			.Select(item => (Row: position.Row + item.DeltaRow, Column: position.Column + item.DeltaColumn))
			.Where(item =>
				item.Row >= 0 && item.Row < map.Count &&
				item.Column >= 0 && item.Column < map[0].Length)
			.Where(item => map[item.Row][item.Column] == map[position.Row][position.Column] + 1)
			.SelectMany(ReachableTopPositions);
