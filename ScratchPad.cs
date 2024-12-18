// File that may contain copy/pasted code.

//Func<List<string>, List<string>> transpose = (List<string> input) =>
//	Enumerable
//		.Range(0, input[0].Length)
//		.Select(columnIndex => new string(
//			Enumerable
//				.Range(0, input.Count)
//				.Select(rowIndex => input[rowIndex][columnIndex])
//				.ToArray()))
//		.ToList();

//void PrintGrid(ImmutableHashSet<Position> occupiedPositions)
//{
//	var grid = Enumerable.Range(0, widthAndHeight).Select(index => Enumerable.Repeat('.', widthAndHeight).ToArray()).ToList();
//	foreach (Position occupiedPosition in occupiedPositions)
//		grid[occupiedPosition.Y][occupiedPosition.X] = '#';
//	foreach (char[] rowChars in grid)
//		Console.WriteLine(new string(rowChars));
//}
