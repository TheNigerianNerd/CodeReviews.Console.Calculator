namespace CalculatorLibrary.TheNigerianNerd
{
    using Newtonsoft.Json;
    using System;
    using System.Text.RegularExpressions;

    public class Calculator
    {
        JsonWriter writer;

        List<Operations> operations = new List<Operations>(); // This list is not used in the current code but can be useful for storing operations if needed.
        public Calculator()
        {
            StreamWriter logFile = File.CreateText("calculatorlog.json");
            logFile.AutoFlush = true;
            writer = new JsonTextWriter(logFile);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartObject();
            writer.WritePropertyName("Operations");
            writer.WriteStartArray();
        }
        //Define the OperationType enum to represent different mathematical operations.
        public double DoOperation(double num1, double num2, string op)
        {
            double result = double.NaN; // Default value is "not-a-number" if an operation, such as division, could result in an error.
            writer.WriteStartObject();
            writer.WritePropertyName("Operand1");
            writer.WriteValue(num1);
            writer.WritePropertyName("Operand2");
            writer.WriteValue(num2);
            writer.WritePropertyName("Operation");

            // Determine the operation type based on user input.
            OperationType operationType = OperationType.None;

            // Use a switch statement to do the math.
            switch (op)
            {
                case "a":
                    operationType = OperationType.Addition;
                    result = num1 + num2;
                    writer.WriteValue(operationType);
                    break;
                case "s":
                    operationType = OperationType.Subtraction;
                    result = num1 - num2;
                    writer.WriteValue(operationType);
                    break;
                case "m":
                    operationType = OperationType.Multiplication;
                    result = num1 * num2;
                    writer.WriteValue(operationType);
                    break;
                case "d":
                    operationType = OperationType.Divison;
                    // Ask the user to enter a non-zero divisor.
                    if (num2 != 0)
                    {
                        result = num1 / num2;
                    }
                    writer.WriteValue(operationType);
                    break;
                case "r":
                    operationType = OperationType.SquareRoot;
                    // Check if the number is non-negative before calculating the square root.
                    Console.WriteLine("Calculating square root...");
                    Console.WriteLine($"Discarding second operand, {num1:N2} will be used");
                    if (num1 >= 0)
                    {
                        result = Math.Sqrt(num1);
                    }
                    else
                    {
                        Console.WriteLine("Cannot calculate square root of a negative number.");
                        result = double.NaN; // Set result to NaN for invalid operation.
                    }
                    writer.WriteValue(operationType);
                    break;
                case "p":
                    operationType = OperationType.Power;
                    // Calculate the power of the first number raised to the second number.
                    result = Math.Pow(num1, num2);
                    writer.WriteValue(operationType);
                    break;
                // Return text for an incorrect option entry.
                default:
                    break;
            }
            writer.WritePropertyName("Result");
            writer.WriteValue(result);
            writer.WriteEndObject();

            // Store the operation in the list for potential future use.
            operations.Add(new Operations(num1, num2, operationType, result));

            return result;
        }
        // CalculatorLibrary.cs
        public void Finish()
        {
            writer.WriteEndArray();
            writer.WriteEndObject();
            writer.Close();
        }
        // Method to list all operations performed.
        public void List()
        {
            // Check if there are any operations to display.
            if (operations.Count == 0)
            {
                Console.WriteLine("No operations to list.");
                return;
            }
            // Display the number of operations performed.
            for (int i = 0; i < operations.Count; i++)
            {
                // Print each operation in a formatted way.
                Operations op = operations[i];
                Console.WriteLine($@"
{i + 1,-3}) Operation Summary
    ─────────────────────────────
    Operand 1 : {op.Operand1,10:N2}
    Operand 2 : {op.Operand2,10:N2}
    Operation : {op.OperationType}
    Result    : {op.Result,10:N2}
");

            }


            int index = 0;

            //Ensure user either enters d or r to delete or reuse a result
            while (true)
            {
                Console.WriteLine("\nEnter 'd' to delete an operation or 'r' to reuse a result:");
                char key = char.ToLower(Console.ReadKey().KeyChar);

                switch (key)
                {
                    case 'd':
                        // If the user chooses to delete an operation, prompt for the index.
                        Console.WriteLine($"\nEnter the index of the operation you want to delete between 1 and {operations.Count}:");


                        while (!int.TryParse(Console.ReadLine(), out index) || index < 1 || index > operations.Count)
                        {
                            Console.WriteLine($"Invalid input. Please enter a valid number within 1 and {operations.Count}:");
                        }
                        // Delete the operation at the specified index.
                        Delete(index);
                        return;
                    case 'r':
                        // If the user chooses to reuse a result, prompt for the index.
                        Console.WriteLine($"\nEnter the index of the operation you want to reuse between 1 and {operations.Count}:");
                        int operationIndex = 0;

                        while (!int.TryParse(Console.ReadLine(), out index) || index < 1 || index > operations.Count)
                        {
                            Console.WriteLine($"Invalid input. Please enter a valid number within 1 and {operations.Count}:");
                        }

                        Reuse(operations[index - 1].Result);
                        Console.WriteLine("Reuse operation completed successfully.");
                        return;
                    default:
                        Console.WriteLine("\nInvalid input. Please enter 'd' to delete an operation or 'r' to reuse a result.");
                        break;

                }
            }
        }
        // Method to reuse a result from a previous operation.
        private void Reuse(double result)
        {
            Console.WriteLine($"\nReusing result: {result:N2}");
            Console.WriteLine("Enter a new number to perform an operation with the reused result:");
            double operand2 = 0;
            bool IsValid = Double.TryParse(Console.ReadLine(), out operand2);

            while (!IsValid)
            {
                Console.WriteLine("Invalid input. Please enter a numeric value.");
                IsValid = Double.TryParse(Console.ReadLine(), out operand2);
            }
            // Ask the user to choose an operator.
            // Display the available operations using the OperationType enum.
            Console.WriteLine("Enter operation to carry out...");
            Console.WriteLine($"\ta - {OperationType.Addition}");
            Console.WriteLine($"\ts - {OperationType.Subtraction}");
            Console.WriteLine($"\tm - {OperationType.Multiplication}");
            Console.WriteLine($"\td - {OperationType.Divison}");
            Console.Write("Your option? ");

            string? op = Console.ReadLine();
            if (op == null || !Regex.IsMatch(op, "[a|s|m|d]"))
            {
                Console.WriteLine("Error: Unrecognized input.");
            }
            else
            {
                try
                {
                    result = DoOperation(result, operand2, op);
                    if (double.IsNaN(result))
                    {
                        Console.WriteLine("This operation will result in a mathematical error.\n");
                    }
                    else Console.WriteLine($"Your result: {result:0.##}\n");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Oh no! An exception occurred trying to do the math.\n - Details: " + e.Message);
                }
            }
        }

        //Delete index from list of operations
        private void Delete(int index)
        {
            operations.RemoveAt(index - 1);
            Console.WriteLine("Delete Successful \n ------------------------------------------------------");
        }
    }
}
