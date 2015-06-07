using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

using Y2LCore;
using Y2LCore.SyntaxTree;

namespace Y2LInterpreter
{
	class Interpreter
	{

		public static void Run(string filePath)
		{
			
			Scanner scanner = null;
			using (TextReader input = new StreamReader(filePath))
			{
				scanner = new Scanner(input);
			}
			Parser parser = new Parser(scanner);

			//try
			//{
			Module mod = parser.Parse();

			if (parser.Errors.Count == 0)
				Run(mod);
			else
			{
				Console.ForegroundColor = ConsoleColor.DarkCyan;
				foreach (SyntaxError error in parser.Errors)
					Console.WriteLine(error);
				Console.ResetColor();		
			}
		}
		public static void Run(Module module)
		{
			Y2LExecutor evaluator = new Y2LExecutor(module);


			// evaluator.Evaluate();
			// Console.WriteLine("-----" + module.Functions[0].GetVar("e"));
			Function main = module.GetFunction("Main");


			if (main == null)
			{
                PrintError("'" + module.Name + "' does not have a Main method");
				return;
			}

            foreach (var stmt in module.Statements)
                evaluator.ExeStatement(module, stmt);

			RunFunction(evaluator,module, main);

		}
		static void RunFunction(Y2LExecutor evaluator,Module module, CodeBlock function)
		{
			foreach (Statement stmt in function.Statements)
			{				
				//RunStatement(evaluator,module, function, stmt);
				evaluator.ExeStatement(function,stmt);
			}
		}
		static void PrintError(params string[] messages)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Array.ForEach(messages, m => Console.WriteLine(m));
			Console.ResetColor();
		}
	}
}
