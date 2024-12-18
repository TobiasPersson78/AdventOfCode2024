using System.Collections.Immutable;

bool useExampleInput = false;

(string inputFilename, int widthAndHeight, int itemsToTake) = useExampleInput
	? ("exampleInput.txt", 6 + 1, 12)
	: ("input.txt", 70 + 1, 1024);

List<Position> input = File
	.ReadAllLines(inputFilename)
	.Select(line => line.Split(','))
	.Select(item => new Position(int.Parse(item[0]), int.Parse(item[1])))
	.ToList();
Position startPosition = new(0, 0);
Position endPosition = new(widthAndHeight - 1, widthAndHeight - 1);

int resultPartA = TraverseGridToEnd(input[..itemsToTake].ToImmutableHashSet());

int minItemsToTake = itemsToTake;
int maxItemsToTake = input.Count;
while (maxItemsToTake - minItemsToTake > 1)
{
	int midItemsToTake = (maxItemsToTake + minItemsToTake) / 2;

	if (TraverseGridToEnd(input[..midItemsToTake].ToImmutableHashSet()) < 0)
		maxItemsToTake = midItemsToTake;
	else
		minItemsToTake = midItemsToTake;
}
string resultPartB = input[maxItemsToTake - 1].X + "," + input[maxItemsToTake - 1].Y;

Console.WriteLine("Day 18");
Console.WriteLine($"A: The minimum number of steps is {resultPartA}."); // 22, 318
Console.WriteLine($"B: The end-blocking position is {resultPartB}."); // "6,1", "56,29"

int TraverseGridToEnd(ImmutableHashSet<Position> occupiedPositions)
{
	Queue<(Position Position, int Steps)> positionsToCheck = new([(startPosition, 0)]);
	HashSet<Position> visitedNodes = new();
	while (positionsToCheck.TryDequeue(out (Position Position, int Steps) currentTraversedPositions))
	{
		Position currentPosition = currentTraversedPositions.Position;

		if (!visitedNodes.Add(currentPosition))
			continue;

		if (currentPosition == endPosition)
			return currentTraversedPositions.Steps;

		foreach (Position neighbor in GetUnoccupiedNeighbors(currentPosition, occupiedPositions))
			positionsToCheck.Enqueue((neighbor, currentTraversedPositions.Steps + 1));
	}

	return -1;
}

IEnumerable<Position> GetUnoccupiedNeighbors(Position position, ImmutableHashSet<Position> occupiedPositions) =>
	new (int Dx, int Dy)[] { (0, -1), (1, 0), (0, 1), (-1, 0) }
		.Select(item => new Position(position.X + item.Dx, position.Y + item.Dy))
		.Where(item => item is ( >= 0, >= 0) && item.X < widthAndHeight && item.Y < widthAndHeight)
		.Where(item => !occupiedPositions.Contains(item));

record struct Position(int X, int Y);
