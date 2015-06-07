using System;
using System.Runtime.Hosting;
using System.Text;
using Y2LCore;
using System.IO;
using Y2LCore.SyntaxTree;

namespace Y2LInterpreter
{
	class Program
	{
		static void Main(string[] args)
		{
			
			Console.Title = "Y2L Interpreter 1.0";
            if (args.Length == 0)
            {

                Console.WriteLine("Y2LInterpreter [drive:][path]filename");
                return;
            }
			//   args = new string[] { "C:\\a.txt" };

			string filePath = args[0];
            try
            {
				if (filePath.EndsWith(".yexe"))
				{
					Module module = CodeIO.Load(args[0]);
					Interpreter.Run(module);
				}
				else
					Interpreter.Run(args[0]);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Runtime Error: ");
                Console.ResetColor();
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }

		//	Console.Write("\nPress any key to continue...");
		//	Console.ReadKey();
		}
	}
}
