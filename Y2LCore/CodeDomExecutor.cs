using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;
using System.Text;

namespace Y2LCore
{
    public class CodeDomExecutor
    {
        public static object ExecuteLines(params string[] lines)
        {

            StringBuilder str = new StringBuilder("class Foo{ public object MyMethod(){ ");

            Array.ForEach(lines, s => str.Append(s).Append(";"));

            str.Append(" return null;}}");

            CodeCompileUnit comUnit = new CodeCompileUnit();
            CodeDomProvider codeProvider = new CSharpCodeProvider();

            CompilerParameters comParam = new CompilerParameters();
            comParam.GenerateExecutable = false;
            comParam.GenerateInMemory = true;

            CompilerResults comResults = codeProvider.CompileAssemblyFromSource(comParam, str.ToString());
            if (comResults.Errors.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                foreach (CompilerError ce in comResults.Errors)
                {
                    Console.WriteLine("Line: {0}: Error Number:{1}\n{2}", ce.Line, ce.ErrorNumber, ce.ErrorText);
                }
                Console.ResetColor();
            }

            Type mType = comResults.CompiledAssembly.GetType("Foo");

            ConstructorInfo conInfo = mType.GetConstructor(Type.EmptyTypes);

            object obj = conInfo.Invoke(null);

            object ret = mType.InvokeMember("MyMethod", BindingFlags.InvokeMethod, null, obj, null);
            return ret;

        }

    }
}
