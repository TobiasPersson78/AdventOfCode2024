bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

string[] matrix = File.ReadAllLines(inputFilename);
Position guardStartPosition = matrix
	.Select((row, rowIndex) => new Position(rowIndex, row.IndexOf('^')))
	.First(position => position.Column >= 0);
HashSet<Position> visitedCellsPartA = TraverseMap(new Position(-1, -1)).VisitedCells;
int loopCountPartB = visitedCellsPartA
	.Where(item => item != guardStartPosition)
	.AsParallel()
	.Count(potentialPlacementForLoop => TraverseMap(potentialPlacementForLoop).IsLoop);

Console.WriteLine("Day 6 - Guard Gallivant");
Console.WriteLine($"A: Number of cells visited by the guard is {visitedCellsPartA.Count}."); // 41, 4826
Console.WriteLine($"B: Number of potential loop placements is {loopCountPartB}."); // 6, 1721

Position Move(Position position, Direction delta) =>
	new(position.Row + delta.DeltaRow, position.Column + delta.DeltaColumn);
Direction RotateRight(Direction direction) => new(direction.DeltaColumn, -direction.DeltaRow);
bool IsInMatrix(Position position, Direction delta) =>
	position.Row + delta.DeltaRow >= 0 && position.Row + delta.DeltaRow < matrix.Length &&
	position.Column + delta.DeltaColumn >= 0 && position.Column + delta.DeltaColumn < matrix[0].Length;
bool IsOccupied(Position position, Direction delta) =>
	IsInMatrix(position, delta) &&
	matrix[position.Row + delta.DeltaRow][position.Column + delta.DeltaColumn] == '#';
(bool IsLoop, HashSet<Position> VisitedCells) TraverseMap(Position potentialPlacementForLoop)
{
	Position guardPosition = guardStartPosition;
	Direction movement = new(-1, 0);
	HashSet<(Position Position, Direction Movement)> directionIntoCell = [(guardPosition, movement)];
	while (IsInMatrix(guardPosition, movement))
	{
		while (IsOccupied(guardPosition, movement) || Move(guardPosition, movement) == potentialPlacementForLoop)
			movement = RotateRight(movement);

		guardPosition = Move(guardPosition, movement);

		if (!directionIntoCell.Add((guardPosition, movement)))
			return (true, directionIntoCell.Select(item => item.Position).ToHashSet());
	}

	return (false, directionIntoCell.Select(item => item.Position).ToHashSet());
}

record struct Position(int Row, int Column);
record struct Direction(int DeltaRow, int DeltaColumn);
