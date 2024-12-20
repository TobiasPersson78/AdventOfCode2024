bool useExampleInput = true;

(string inputFilename, int minCountCheatLimit) = useExampleInput
	? ("exampleInput.txt", 50)
	: ("input.txt", 100);

string[] map = File.ReadAllLines(inputFilename);
Position endPosition = map
	.Select((row, rowIndex) => new Position(rowIndex, row.IndexOf('E')))
	.First(item => item.Column >= 0);
Dictionary<Position, int> stepsToEndLookup =
	(from row in Enumerable.Range(0, map.Length)
	 from column in Enumerable.Range(0, map[0].Length)
	 where map[row][column] != '#'
	 select new Position(row, column))
		.ToDictionary(item => item, _ => int.MaxValue);
stepsToEndLookup[endPosition] = 0;
for (Position previous = endPosition, current = GetUnvisitedNeighbor(endPosition);
		current is { Row: > -1, Column: > -1 };
		previous = current, current = GetUnvisitedNeighbor(current))
	stepsToEndLookup[current] = stepsToEndLookup[previous] + 1;

List<IEnumerable<(Position From, Position To, int SavedSteps)>> cheatsForCheatLength = new[] { 2, 20 }
	.Select(maxCheatSteps =>
		stepsToEndLookup
			.SelectMany(fromPositionAndSteps =>
				GetPotentialCheatDestinations(fromPositionAndSteps.Key, maxCheatSteps)
					.Select(item =>
						(From: fromPositionAndSteps.Key,
						To: item.Position,
						SavedSteps: fromPositionAndSteps.Value - stepsToEndLookup[item.Position] - item.CheatSteps))))
	.ToList();

int resultPartA = cheatsForCheatLength[0].Count(item => item.SavedSteps >= minCountCheatLimit);
int resultPartB = cheatsForCheatLength[1].Count(item => item.SavedSteps >= minCountCheatLimit);

Console.WriteLine("Day 20");
Console.WriteLine($"A: The number of cheats of max 2 steps saving at least {minCountCheatLimit} steps is {resultPartA}."); // 1, 1317
Console.WriteLine($"B: The number of cheats of max 20 steps saving at least {minCountCheatLimit} steps is {resultPartB}."); // 285, 982474

Position GetUnvisitedNeighbor(Position position) =>
	new (int DeltaRow, int DeltaColumn)[] { (-1, 0), (0, 1), (1, 0), (0, -1) }
		.Select(item => new Position(position.Row + item.DeltaRow, position.Column + item.DeltaColumn))
		.FirstOrDefault(item => stepsToEndLookup.TryGetValue(item, out int steps) && steps == int.MaxValue, new(-1, -1));

IEnumerable<(Position Position, int CheatSteps)> GetPotentialCheatDestinations(Position position, int maxCheatSteps) =>
	(from deltaRow in Enumerable.Range(-maxCheatSteps, 2 * maxCheatSteps + 1)
	 from deltaColumn in Enumerable.Range(-maxCheatSteps, 2 * maxCheatSteps + 1)
	 select (Position: new Position(position.Row + deltaRow, position.Column + deltaColumn), CheatSteps: Math.Abs(deltaRow) + Math.Abs(deltaColumn)))
		.Where(item => item.CheatSteps > 0 && item.CheatSteps <= maxCheatSteps)
		.Where(item => stepsToEndLookup.ContainsKey(item.Position));

record struct Position(int Row, int Column);
