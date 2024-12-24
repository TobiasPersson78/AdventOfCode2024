bool useExampleInput = false;

string inputFilename = useExampleInput
	? "exampleInput.txt"
	: "input.txt";

string[] inputSections = File
	.ReadAllText(inputFilename)
	.Split("\r\n\r\n");
Dictionary<string, bool> staticInputWires = inputSections[0]
	.Split("\r\n")
	.Select(row => row.Split(": "))
	.ToDictionary(item => item[0], item => item[1] == "1");
Dictionary<string, (string TypeOfGate, string Input1, string Input2)> operationForGateLookup = inputSections[1]
	.Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
	.Select(row => row.Split(' '))
	.Select(item => (Output: item[^1], Operation: (TypeOfGate: item[1], Input1: item[0], Input2: item[2])))
	.ToDictionary(item => item.Output, item => item.Operation);
List<string> outputWires = operationForGateLookup.Keys.Where(key => key[0] == 'z').Order().ToList();
long resultPartA = outputWires
	.Aggregate(
		(Sum: 0L, Shift: 0),
		(acc, curr) =>
			(acc.Sum +
			 ((EvaluateGate(curr) ? 1L : 0L) << acc.Shift),
			 acc.Shift + 1))
	.Sum;

// A full adder circuit has three inputs (A, B and Carry In) - and two outputs (S and Carry Out).
// They have five gates:
// - The first XOR gate has A and B as input and sends its output to the second XOR gate and the
//   second AND gate.
// - The second XOR gate's other input is Carry In and it sends its output to S.
// - The first AND gate has A and B as input and sends its output to the OR gate.
// - The second AND gate's other input is Carry In and it sends its output to the OR gate.
// - The OR gate sends its output to Carry Out.
// Here, the z wires represent S and the x and y wires represent A and B. There are also two
// exceptions, that have been manually verified:
// - z00 does not have a Carry In, and the Carry Out for (x00, y00) is only dependent on those
//   two input because there is no Carry In.
// - z45 is technically the Carry Out for (x44, y44). There are no (x45, y45) inputs.
// But the rest of the z wires should follow the above full adder circuit.
string resultPartB = "Not applicable";
if (!useExampleInput)
{
	List<string> replacedWires = [];

	string carryIn = TryGetGateForOperation("AND", "x00", "y00")!;
	for (int i = 1; i < outputWires.Count - 1; ++i)
	{
		string xInput = $"x{i:D2}";
		string yInput = $"y{i:D2}";
		string zOutput = $"z{i:D2}";

		string xor1 = TryGetGateForOperation("XOR", xInput, yInput)!;
		string and1 = TryGetGateForOperation("AND", xInput, yInput)!;

		string? xor2 = TryGetGateForOperation("XOR", carryIn, xor1);
		string? and2 = TryGetGateForOperation("AND", carryIn, xor1);

		if (xor2 is null && and2 is null)
		{
			(operationForGateLookup[xor1], operationForGateLookup[and1]) =
				(operationForGateLookup[and1], operationForGateLookup[xor1]);
			replacedWires.AddRange([xor1, and1]);
			--i;
			continue;
		}

		if (xor2 != zOutput)
		{
			(operationForGateLookup[xor2!], operationForGateLookup[zOutput]) =
				(operationForGateLookup[zOutput], operationForGateLookup[xor2!]);
			replacedWires.AddRange([xor2!, zOutput]);
			--i;
			continue;
		}

		carryIn = TryGetGateForOperation("OR", and1, and2!)!; // This loop's Carry Out becomes the next loop's Carry In.
	}

	resultPartB = string.Join(',', replacedWires.Order());
}

Console.WriteLine("Day 24");
Console.WriteLine($"A: The z number output is {resultPartA}."); // 2024, 55544677167336
Console.WriteLine($"B: The swapped wires are {resultPartB}."); // "Not applicable", "gsd,kth,qnf,tbt,vpm,z12,z26,z32"

bool EvaluateGate(string gate)
{
	if (staticInputWires.TryGetValue(gate, out bool wireValue))
		return wireValue;

	(string typeOfGate, string input1, string input2) = operationForGateLookup[gate];
	return typeOfGate switch
	{
		"AND" => EvaluateGate(input1) & EvaluateGate(input2),
		"OR" => EvaluateGate(input1) | EvaluateGate(input2),
		"XOR" => EvaluateGate(input1) ^ EvaluateGate(input2),
	};
}

string? TryGetGateForOperation(string typeOfGate, string oneOperand, string otherOperand) =>
	operationForGateLookup
		.FirstOrDefault(item =>
			item.Value.TypeOfGate == typeOfGate &&
			((item.Value.Input1 == oneOperand && item.Value.Input2 == otherOperand) ||
			 (item.Value.Input1 == otherOperand && item.Value.Input2 == oneOperand)))
		.Key;
