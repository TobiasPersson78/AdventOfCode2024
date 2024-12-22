using System.Text.RegularExpressions;

bool useExampleInput = false;

(string inputFilenamePartA, string inputFilenamePartB) = useExampleInput
	? ("exampleInputA.txt", "exampleInputB.txt")
	: ("input.txt", "input.txt");
List<long> inputPartA = ReadNumbers(inputFilenamePartA).ToList();
List<long> memoryPartB = ReadNumbers(inputFilenamePartB).Skip(3).ToList();

string resultPartA = string.Join(',', RunComputerProgram(inputPartA[0], inputPartA[1], inputPartA[3], inputPartA[3..]));
long resultPartB = FindCopyPartB(0, 0);

Console.WriteLine("Day 17");
Console.WriteLine($"A: The output string is {resultPartA}."); // "4,6,3,5,6,3,5,2,1,0", "1,6,7,4,3,0,5,0,6"
Console.WriteLine($"B: The register value that clones the program is {resultPartB}."); // 117440, 216148338630253

IEnumerable<long> ReadNumbers(string inputFilename) => Regex
	.Matches(File.ReadAllText(inputFilename), @"\d+")
	.Select(match => long.Parse(match.Groups[0].Value));

IEnumerable<long> RunComputerProgram(long registerA, long registerB, long registerC, List<long> memory)
{
	for (int pointer = 0; pointer < memory.Count; pointer += 2)
	{
		if (memory[pointer] == 5) // out
			yield return ReadComboOperand(pointer + 1) & 0b111;
		else
			_ = memory[pointer] switch
			{
				0 => registerA >>= (int)ReadComboOperand(pointer + 1), // adv
				1 => registerB ^= memory[pointer + 1], // bxl
				2 => registerB = ReadComboOperand(pointer + 1) & 0b111, // bst
				3 when registerA != 0 => pointer = (int)memory[pointer + 1] - 2, // jnz
				4 => registerB ^= registerC, // bxc
				6 => registerB = registerA >> (int)ReadComboOperand(pointer + 1), // bdv
				7 => registerC = registerA >> (int)ReadComboOperand(pointer + 1), // cdv
				_ => 0
			};
	}

	long ReadComboOperand(int operandPointer) => memory[operandPointer] switch
	{
		>= 0 and <= 3 => memory[operandPointer],
		4 => registerA,
		5 => registerB,
		_ => registerC
	};
}

long FindCopyPartB(long number, int index) => Enumerable
	.Range(0, 8)
	.Select(i =>
		RunComputerProgram(number + i, 0, 0, memoryPartB).First() == memoryPartB[memoryPartB.Count - index - 1]
			? index == memoryPartB.Count - 1
				? number + i // The last digit was found
				: FindCopyPartB((number + i) << 3, index + 1) // Digit found, continue finding more digits.
			: -1) // No digit found
	.FirstOrDefault(item => item >= 0, -1); // return the first matching number, or -1 if no match was found.
