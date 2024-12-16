using System.Collections.Immutable;

bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

string[] map = File.ReadAllLines(inputFilename);
Position startPosition =
	(from row in Enumerable.Range(0, map.Length)
	 from column in Enumerable.Range(0, map[0].Length)
	 where map[row][column] == 'S'
	 select new Position(row, column))
		.First();
Dictionary<PositionAndDirection, int> costToNodeLookup =
	(from row in Enumerable.Range(0, map.Length)
	 from column in Enumerable.Range(0, map[0].Length)
	 from direction in new Direction[] { new(-1, 0), new(0, 1), new(1, 0), new(0, -1) }
	 where map[row][column] != '#'
	 select new PositionAndDirection(new Position(row, column), direction))
		.ToDictionary(item => item, _ => int.MaxValue);
int minimumCostToEnd = int.MaxValue;
ImmutableHashSet<Position> nodesInBestPaths = ImmutableHashSet<Position>.Empty;
PositionAndDirection startPositionAndDirection = new(startPosition, new(0, 1));
costToNodeLookup[startPositionAndDirection] = 0;
PriorityQueue<(PositionAndDirection PositionAndDirection, int Cost, ImmutableHashSet<Position> Visited), int> nodesToCheck =
	new(
		GetPossibleMovements(
			startPositionAndDirection,
			0,
			ImmutableHashSet<Position>.Empty.Add(startPosition))
		.Select(item => (item, item.Cost)));

while (nodesToCheck.Count > 0)
{
	(PositionAndDirection positionAndDirection, int cost, ImmutableHashSet<Position> visited) = nodesToCheck.Dequeue();

	if (cost > minimumCostToEnd || cost > costToNodeLookup[positionAndDirection])
		continue;

	if (cost < costToNodeLookup[positionAndDirection])
		nodesInBestPaths = ImmutableHashSet<Position>.Empty;

	costToNodeLookup[positionAndDirection] = cost;

	if (map[positionAndDirection.Position.Row][positionAndDirection.Position.Column] == 'E')
	{
		minimumCostToEnd = Math.Min(minimumCostToEnd, cost);
		nodesInBestPaths = nodesInBestPaths.Union(visited);
		continue;
	}

	nodesToCheck.EnqueueRange(GetPossibleMovements(positionAndDirection, cost, visited).Select(item => (item, item.Cost)));
}

Console.WriteLine("Day 16");
Console.WriteLine($"A: The minimum point score is {minimumCostToEnd}."); // 7036, 95444
Console.WriteLine($"B: The best number of nodes is {nodesInBestPaths.Count}."); // 45, 513

IEnumerable<(PositionAndDirection PositionAndDirection, int Cost, ImmutableHashSet<Position> Visited)>
	GetPossibleMovements(PositionAndDirection positionAndDirection, int initialCost, ImmutableHashSet<Position> initialVisited)
{
	Position step = Add(positionAndDirection.Position, positionAndDirection.Direction);
	if (map[step.Row][step.Column] != '#')
		yield return (new(step, positionAndDirection.Direction), initialCost + 1, initialVisited.Add(step));
	yield return (new(positionAndDirection.Position, RotateRight(positionAndDirection.Direction)), initialCost + 1000, initialVisited);
	yield return (new(positionAndDirection.Position, RotateLeft(positionAndDirection.Direction)), initialCost + 1000, initialVisited);
}

Position Add(Position position, Direction direction) => new(position.Row + direction.DeltaRow, position.Column + direction.DeltaColumn);

Direction RotateRight(Direction direction) => new(direction.DeltaColumn, -direction.DeltaRow);

Direction RotateLeft(Direction direction) => new(-direction.DeltaColumn, direction.DeltaRow);

record struct Position(int Row, int Column);
record struct Direction(int DeltaRow, int DeltaColumn);
record struct PositionAndDirection(Position Position, Direction Direction);
