//
// Program.cs
//
// Author:
//       Stephane Delcroix <stdelc@microsoft.com>
//
// Copyright (c) 2017 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using Mono.Options;
using Mono.Cecil;
using System.IO;

namespace Sdx.UnSn
{
	class UnStrongName
	{
		static readonly string helpString = "unsn.exe - utility for removing assembly strong name.";
		static readonly string usageString = "unsn.exe [options] assembly.dll. Type `unsn.exe -h` for help";

		public static void Main(string[] args)
		{
			bool help = false;
			IList<string> extra = null;

			var p = new OptionSet {
				{ "h|?|help", "Print this help message", v => help = true },
			};

			try {
				extra = p.Parse(args);
			} catch (OptionException oe) {
				Console.Error.WriteLine($"decompile.exe: argument error:{oe.Message}.\n{usageString}");
				Environment.Exit(0);
			}

			if (help) {
				ShowHelp(p);
				return;
			}

			if (extra.Count != 1) {
				Console.Error.WriteLine($"unsn.exe: missing assembly parameter.\n{usageString}.");
				Environment.Exit(0);
			}

			var assemblyPath = extra[0];
			var assembly = AssemblyDefinition.ReadAssembly(Path.GetFullPath(assemblyPath));
			var name = assembly.Name;
			name.HasPublicKey = false;
			name.PublicKey = new byte[0];
			foreach(var module in assembly.Modules)
				module.Attributes &= ~ModuleAttributes.StrongNameSigned;
			assembly.Write(assemblyPath);
			
		}

		static void ShowHelp(OptionSet ops)
		{
			Console.WriteLine($"{helpString}\n{usageString}\n");
			ops.WriteOptionDescriptions(Console.Out);
		}
	}
}
