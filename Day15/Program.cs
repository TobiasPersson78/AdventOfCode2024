bool useExampleInput = true;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

string[] input = File
	.ReadAllText(inputFilename)
	.Split("\r\n\r\n");
string movements = input[1].Replace("\r\n", "");
List<List<char>> mapPartA = input[0].Split("\r\n").Select(row => row.ToList()).ToList();
List<List<char>> mapPartB = input[0]
	.Split("\r\n")
	.Select(row => row.Replace("#", "##").Replace("O", "[]").Replace(".", "..").Replace("@", "@.").ToList())
	.ToList();

int resultPartA = GetGpsCoordinates(TraverseMap(mapPartA)).Sum();
int resultPartB = GetGpsCoordinates(TraverseMap(mapPartB)).Sum();

Console.WriteLine("Day 15 - Warehouse Woes");
Console.WriteLine($"A: The sum of all boxes' GPS coordinates is {resultPartA}."); // 10092, 1441031
Console.WriteLine($"B: The sum of all boxes' GPS coordinates is {resultPartB}."); // 9021, 1425169

List<List<char>> TraverseMap(List<List<char>> map)
{
	Position robotPosition =
		(from row in Enumerable.Range(0, map.Count)
		 from column in Enumerable.Range(0, map[0].Count)
		 where map[row][column] == '@'
		 select new Position(row, column))
			.First();
	map[robotPosition.Row][robotPosition.Column] = '.';

	foreach (char movement in movements)
	{
		robotPosition = movement switch
		{
			'^' => TryMoveInDirection(map, robotPosition, -1, 0),
			'>' => TryMoveInDirection(map, robotPosition, 0, 1),
			'v' => TryMoveInDirection(map, robotPosition, 1, 0),
			_ => TryMoveInDirection(map, robotPosition, 0, -1)
		};
	}

	return map;
}

Position TryMoveInDirection(List<List<char>> map, Position position, int deltaRow, int deltaColumn)
{
	if (!CanMoveInDirection(map, position, deltaRow, deltaColumn))
		return position;

	MoveInDirection(map, position, deltaRow, deltaColumn);
	return Add(position, deltaRow, deltaColumn);
}

bool CanMoveInDirection(List<List<char>> map, Position position, int deltaRow, int deltaColumn) =>
	map[position.Row + deltaRow][position.Column + deltaColumn] switch
	{
		'.' => true,
		'#' => false,
		'[' when deltaRow != 0 =>
			CanMoveInDirection(map, Add(position, deltaRow, deltaColumn), deltaRow, deltaColumn) &&
			CanMoveInDirection(map, Add(position, deltaRow, deltaColumn + 1), deltaRow, deltaColumn),
		']' when deltaRow != 0 =>
			CanMoveInDirection(map, Add(position, deltaRow, deltaColumn - 1), deltaRow, deltaColumn) &&
			CanMoveInDirection(map, Add(position, deltaRow, deltaColumn), deltaRow, deltaColumn),
		_ => CanMoveInDirection(map, Add(position, deltaRow, deltaColumn), deltaRow, deltaColumn)
	};

void MoveInDirection(List<List<char>> map, Position position, int deltaRow, int deltaColumn)
{
	switch (map[position.Row + deltaRow][position.Column + deltaColumn])
	{
		case '[' when deltaRow != 0:
			MoveInDirection(map, Add(position, deltaRow, deltaColumn), deltaRow, deltaColumn);
			MoveInDirection(map, Add(position, deltaRow, deltaColumn + 1), deltaRow, deltaColumn);
			break;
		case ']' when deltaRow != 0:
			MoveInDirection(map, Add(position, deltaRow, deltaColumn - 1), deltaRow, deltaColumn);
			MoveInDirection(map, Add(position, deltaRow, deltaColumn), deltaRow, deltaColumn);
			break;
		case 'O' or '[' or ']':
			MoveInDirection(map, Add(position, deltaRow, deltaColumn), deltaRow, deltaColumn);
			break;
	}

	map[position.Row + deltaRow][position.Column + deltaColumn] = map[position.Row][position.Column];
	map[position.Row][position.Column] = '.';
}

Position Add(Position position, int deltaRow, int deltaColumn) => new(position.Row + deltaRow, position.Column + deltaColumn);

IEnumerable<int> GetGpsCoordinates(List<List<char>> map) =>
	from row in Enumerable.Range(0, map.Count)
	from column in Enumerable.Range(0, map[0].Count)
	where map[row][column] == 'O' || map[row][column] == '['
	select row * 100 + column;

record Position(int Row, int Column);
