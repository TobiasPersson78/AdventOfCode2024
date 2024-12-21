using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text;

bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

string[] input = File.ReadAllLines(inputFilename);

ImmutableDictionary<char, Position> numericKeyPad = ImmutableDictionary<char, Position>
		.Empty
		.Add('7', new(0, 0))
		.Add('8', new(0, 1))
		.Add('9', new(0, 2))
		.Add('4', new(1, 0))
		.Add('5', new(1, 1))
		.Add('6', new(1, 2))
		.Add('1', new(2, 0))
		.Add('2', new(2, 1))
		.Add('3', new(2, 2))
		.Add('0', new(3, 1))
		.Add('A', new(3, 2));
ImmutableDictionary<char, Position> directionalKeyPad = ImmutableDictionary<char, Position>
		.Empty
		.Add('^', new(0, 1))
		.Add('A', new(0, 2))
		.Add('<', new(1, 0))
		.Add('v', new(1, 1))
		.Add('>', new(1, 2));
string directionsByPriority = "<v^>";
ConcurrentDictionary<(char FromButton, char ToButton, int Level), long> memoizedCountDirectionalMoves = new();

long resultPartA = input.Sum(item => CountDirectionalMovesOverMultipleLevels(item, 2));
long resultPartB = input.Sum(item => CountDirectionalMovesOverMultipleLevels(item, 25)); ;

Console.WriteLine("Day 21");
Console.WriteLine($"A: The sum of complexities is {resultPartA}."); // 126384, 203734
Console.WriteLine($"B: The sum of complexities is {resultPartB}."); // 154115708116294, 246810588779586

long CountDirectionalMovesOverMultipleLevels(string currentString, int numberOfLevels) =>
	long.Parse(currentString[..^1]) *
	PairWise(string.Concat(PairWise(currentString, 'A')
		.Select(item => Traverse(item.From, item.To, numericKeyPad))), 'A')
		.Sum(item => CountDirectionalMoves(item.From, item.To, numberOfLevels));

long CountDirectionalMoves(char fromButton, char toButton, int level) =>
	memoizedCountDirectionalMoves.GetOrAdd((fromButton, toButton, level), _ =>
		level == 1
			? Traverse(fromButton, toButton, directionalKeyPad).Length
			: PairWise(Traverse(fromButton, toButton, directionalKeyPad), 'A')
				.Sum(item => CountDirectionalMoves(item.From, item.To, level - 1)));

string Traverse(char fromButton, char toButton, ImmutableDictionary<char, Position> keyPad)
{
	StringBuilder stringBuilder = new();
	Position toPosition = keyPad[toButton];
	Position fromPosition = keyPad[fromButton];

	DeltaPosition movement = Diff(fromPosition, toPosition);
	int indexOfDirectionToMoveIn = 0;

	while (movement is not { DeltaRow: 0, DeltaColumn: 0 })
	{
		char direction = directionsByPriority[indexOfDirectionToMoveIn++ % directionsByPriority.Length];
		DeltaPosition deltaForDirection = GetDeltaForDirection(direction);
		int numberOfStepsInDirection = deltaForDirection is { DeltaColumn: 0 }
			? movement.DeltaRow / deltaForDirection.DeltaRow
			: movement.DeltaColumn / deltaForDirection.DeltaColumn;
		if (numberOfStepsInDirection <= 0)
			continue;

		Position newPosition = GoInDirection(fromPosition, direction, numberOfStepsInDirection);
		if (!keyPad.ContainsValue(newPosition))
			continue;

		stringBuilder.Append(new string(direction, numberOfStepsInDirection));
		fromPosition = newPosition;
		movement = Diff(fromPosition, toPosition);
	}

	stringBuilder.Append('A');
	return stringBuilder.ToString();
}

Position GoInDirection(Position position, char direction, int steps) =>
	direction switch
	{
		'<' => position with { Column = position.Column - steps },
		'v' => position with { Row = position.Row + steps },
		'^' => position with { Row = position.Row - steps },
		_ => position with { Column = position.Column + steps }
	};
DeltaPosition GetDeltaForDirection(char direction) =>
	direction switch
	{
		'<' => new DeltaPosition(0, -1),
		'v' => new DeltaPosition(1, 0),
		'^' => new DeltaPosition(-1, 0),
		_ => new DeltaPosition(0, 1),
	};

IEnumerable<(char From, char To)> PairWise(string stringToPair, char prefix) =>
	stringToPair.Select((currentChar, index) => (index == 0 ? prefix : stringToPair[index - 1], currentChar));

DeltaPosition Diff(Position firstPosition, Position secondPosition) =>
	new(secondPosition.Row - firstPosition.Row, secondPosition.Column - firstPosition.Column);

record struct Position(int Row, int Column);

record struct DeltaPosition(int DeltaRow, int DeltaColumn);
