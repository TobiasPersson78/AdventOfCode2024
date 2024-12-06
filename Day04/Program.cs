bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

List<string> matrix = File
	.ReadAllLines(inputFilename)
	.ToList();

bool IsCellMatch(int row, int column, char match) =>
	row >= 0 && row < matrix.Count &&
	column >= 0 && column < matrix[0].Length &&
	matrix[row][column] == match;
bool IsMatchForStringOrReversedString(int row, int column, int deltaRow, int deltaColumn, string pattern) =>
	Enumerable
		.Range(0, pattern.Length)
		.All(index => IsCellMatch(row + deltaRow * index, column + deltaColumn * index, pattern[index])) ||
	Enumerable
		.Range(0, pattern.Length)
		.All(index => IsCellMatch(row + deltaRow * index, column + deltaColumn * index, pattern[pattern.Length - index - 1]));
bool IsMasInX(int row, int column) =>
	IsMatchForStringOrReversedString(row - 1, column - 1, 1, 1, "MAS") &&
	IsMatchForStringOrReversedString(row + 1, column - 1, -1, 1, "MAS");

int xmasCount = new (int DeltaRow, int DeltaColumn)[] { (0, 1), (1, 0), (1, 1), (-1, 1) }
	.Sum(direction => Enumerable
		.Range(0, matrix.Count)
		.Sum(row => Enumerable
			.Range(0, matrix[0].Length)
			.Count(column => IsMatchForStringOrReversedString(row, column, direction.DeltaRow, direction.DeltaColumn, "XMAS"))));
int masCrossedCount = Enumerable
	.Range(1, matrix.Count - 2)
	.Sum(rowIndex => Enumerable
		.Range(1, matrix[0].Length - 2)
		.Count(columnIndex => IsMasInX(rowIndex, columnIndex)));

Console.WriteLine("Day 4");
Console.WriteLine($"A: Number of XMAS occurrences is {xmasCount}."); // 18, 2507
Console.WriteLine($"B: Number of MAS in X occurrences is {masCrossedCount}."); // 9, 1969
