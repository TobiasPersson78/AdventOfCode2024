using System.Text.RegularExpressions;

bool useExampleInput = false;

(string inputFilename, int width, int height) = useExampleInput
	? ("exampleInput.txt", 11, 7)
	: ("input.txt", 101, 103);

List<(int Row, int Column, int RowSpeed, int ColumnSpeed)> robots = Regex
	.Matches(File.ReadAllText(inputFilename), @"-?\d+")
	.Select(match => int.Parse(match.Groups[0].Value))
	.Chunk(4)
	.Select(item => (Row: item[1], Column: item[0], RowSpeed: item[3], ColumnSpeed: item[2]))
	.ToList();
var moveSteps = (IEnumerable<(int Row, int Column, int RowSpeed, int ColumnSpeed)> itemsToMove, int steps) => itemsToMove
	.Select(item => (Row: item.Row + steps * item.RowSpeed, Column: item.Column + steps * item.ColumnSpeed))
	.Select(item => (Row: (item.Row % height + height) % height, Column: (item.Column % width + width) % width));
var countQuadrants = (IEnumerable<(int Row, int Column)> itemsToCount) => itemsToCount
	.Aggregate(
		(TopLeft: 0, TopRight: 0, BottomLeft: 0, BottomRight: 0),
		(acc, curr) => (
			TopLeft: acc.TopLeft + (curr.Row < height / 2 && curr.Column < width / 2 ? 1 : 0),
			TopRight: acc.TopRight + (curr.Row < height / 2 && curr.Column > width / 2 ? 1 : 0),
			BottomLeft: acc.BottomLeft + (curr.Row > height / 2 && curr.Column < width / 2 ? 1 : 0),
			BottomRight: acc.BottomRight + (curr.Row > height / 2 && curr.Column > width / 2 ? 1 : 0)));
var safetyFactor = ((int TopLeft, int TopRight, int BottomLeft, int BottomRight) quadrants) =>
	quadrants.TopLeft * quadrants.TopRight * quadrants.BottomLeft * quadrants.BottomRight;
int resultPartA = safetyFactor(countQuadrants(moveSteps(robots, 100)));
int resultPartB = Enumerable.Range(1, int.MaxValue).First(steps => moveSteps(robots, steps).Distinct().Count() == robots.Count);

Console.WriteLine("Day 14");
Console.WriteLine($"A: The safety factor is {resultPartA}."); // 12, 217328832
Console.WriteLine($"B: The number of seconds is {resultPartB}."); // 1 (but really not applicable), 7412

var map = Enumerable.Range(0, height).Select(item => Enumerable.Repeat(' ', width).ToArray()).ToList();
foreach ((int Row, int Column) position in moveSteps(robots, resultPartB))
	map[position.Row][position.Column] = 'X';
Console.ForegroundColor = ConsoleColor.Green;
foreach (char[] row in map)
	Console.WriteLine(row);
