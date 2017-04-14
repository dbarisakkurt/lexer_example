using System;
using System.Collections.Generic;
using System.IO;

namespace Lexer
{

    //lexer program exercise: CraftingInterpreters book
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("You must provide only 1 argument");
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }

        static internal void RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");
                Run(Console.ReadLine());
            }

        }

        static internal void RunFile(string fileName)
        {
            string fileContent = File.ReadAllText(fileName);
            Run(fileContent);
        }

        static internal void Run(string input)
        {
            Scanner scanner = new Scanner(input);
            List<Token> tokens = scanner.ScanTokens();

            // For now, just print the tokens.
            foreach (Token token in tokens)
            {
                Console.WriteLine(token.ToString());
            }

        }
    }
}
