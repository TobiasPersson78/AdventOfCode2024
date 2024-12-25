using System.Collections.Immutable;

bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

List<string> map = File
	.ReadAllLines(inputFilename)
	.ToList();
HashSet<Position> visitedNodes = new();
IEnumerable<Position> allNodes =
	from row in Enumerable.Range(0, map.Count)
	from column in Enumerable.Range(0, map[0].Length)
	select new Position(row, column);

int resultPartA = 0;
int resultPartB = 0;
foreach (Position position in allNodes)
{
	ImmutableHashSet<Position>? nodes = GetArea(position);
	(int fences, int corners) = CountCorners(nodes);
	resultPartA += nodes.Count * fences;
	resultPartB += nodes.Count * corners;
}

Console.WriteLine("Day 12 - Garden Groups");
Console.WriteLine($"A: Fence cost is {resultPartA}."); // 1930, 1433460
Console.WriteLine($"B: Fence cost is {resultPartB}."); // 1206, 855082

IEnumerable<Position> GetSimilarNeighbors(Position position) =>
	new (int DeltaRow, int DeltaColumn)[] { (0, 1), (1, 0), (0, -1), (-1, 0) }
		.Select(item => new Position(position.Row + item.DeltaRow, position.Column + item.DeltaColumn))
		.Where(item =>
			item is { Row: >= 0, Column: >= 0 } &&
			item.Row < map.Count && item.Column < map[0].Length)
		.Where(item => map[item.Row][item.Column] == map[position.Row][position.Column]);

ImmutableHashSet<Position> GetArea(Position position)
{
	if (!visitedNodes.Add(position))
		return ImmutableHashSet<Position>.Empty;

	return GetSimilarNeighbors(position)
		.Aggregate(
			ImmutableHashSet<Position>.Empty.Add(position),
			(acc, curr) => acc.Union(GetArea(curr)));
}

(int Fences, int Corners) CountCorners(ImmutableHashSet<Position> nodes) =>
	nodes.Aggregate(
		(Fences: 0, Corners: 0),
		(acc, curr) =>
		{
			Position top = curr with { Row = curr.Row - 1 };
			Position right = curr with { Column = curr.Column + 1 };
			Position bottom = curr with { Row = curr.Row + 1 };
			Position left = curr with { Column = curr.Column - 1 };

			int fenceCount = new[] { top, right, bottom, left }.Count(item => !nodes.Contains(item));

			int cornerCount = 0;

			// Outer corners
			cornerCount += !nodes.Contains(top) && !nodes.Contains(right) ? 1 : 0;
			cornerCount += !nodes.Contains(right) && !nodes.Contains(bottom) ? 1 : 0;
			cornerCount += !nodes.Contains(bottom) && !nodes.Contains(left) ? 1 : 0;
			cornerCount += !nodes.Contains(left) && !nodes.Contains(top) ? 1 : 0;

			Position topLeft = new Position(curr.Row - 1, curr.Column - 1);
			Position topRight = new Position(curr.Row - 1, curr.Column + 1);
			Position bottomRight = new Position(curr.Row + 1, curr.Column + 1);
			Position bottomLeft = new Position(curr.Row + 1, curr.Column - 1);

			// Inner corners
			cornerCount += nodes.Contains(top) && nodes.Contains(right) && !nodes.Contains(topRight) ? 1 : 0;
			cornerCount += nodes.Contains(right) && nodes.Contains(bottom) && !nodes.Contains(bottomRight) ? 1 : 0;
			cornerCount += nodes.Contains(bottom) && nodes.Contains(left) && !nodes.Contains(bottomLeft) ? 1 : 0;
			cornerCount += nodes.Contains(left) && nodes.Contains(top) && !nodes.Contains(topLeft) ? 1 : 0;

			return (acc.Fences + fenceCount, acc.Corners + cornerCount);
		});

record Position(int Row, int Column);
