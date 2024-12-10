bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

List<string> map = File
	.ReadAllLines(inputFilename)
	.ToList();
IList<Position> trailHeads = Enumerable
	.Range(0, map.Count)
	.SelectMany(rowIndex => Enumerable
		.Range(0, map[0].Length)
		.Where(columnIndex => map[rowIndex][columnIndex] == '0')
		.Select(columnIndex => new Position(rowIndex, columnIndex)))
	.ToList();

int resultPartA = trailHeads.Sum(item => ReachableTopPositions(item).Distinct().Count());
int resultPartB = trailHeads.Sum(item => ReachableTopPositions(item).Count());

Console.WriteLine("Day 10");
Console.WriteLine($"A: Sum of number of reachable tops is {resultPartA}."); // 36, 822
Console.WriteLine($"B: Sum of trail ratings is {resultPartB}."); // 81, 1801

IEnumerable<Position> GetUphillSlopeNeighbors(Position position) =>
	new[] { (Dx: 1, Dy: 0), (Dx: 0, Dy: 1), (Dx: -1, Dy: 0), (Dx: 0, Dy: -1) }
		.Select(item => new Position(position.Row + item.Dy, position.Column + item.Dx))
		.Where(item =>
			item.Row >= 0 && item.Row < map.Count &&
			item.Column >= 0 && item.Column < map[0].Length)
		.Where(item => map[item.Row][item.Column] == map[position.Row][position.Column] + 1);

IEnumerable<Position> ReachableTopPositions(Position position) =>
	map[position.Row][position.Column] == '9'
		? [position]
		: GetUphillSlopeNeighbors(position).SelectMany(ReachableTopPositions);

record Position(int Row, int Column);
