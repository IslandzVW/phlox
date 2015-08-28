﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using InWorldz.Phlox.ByteCompiler;
using InWorldz.Phlox.Types;
using InWorldz.Phlox.VM;

using Antlr.Runtime;
using Antlr.Runtime.Tree;

namespace CompilerRunner
{
    class ByteCompilerFrontend
    {
        private System.IO.TextWriter _traceDestination;
        public System.IO.TextWriter TraceDestination
        {
            get
            {
                return _traceDestination;
            }
            
            set
            {
                _traceDestination = value;
            }
        }

        private ILSLListener _listener;
        public ILSLListener Listener
        {
            get
            {
                return _listener;
            }

            set
            {
                _listener = value;
            }
        }

        private CompiledScript _result;

        public CompiledScript Result
        {
            get
            {
                return _result;
            }
        }

        public void Compile(ICharStream input)
        {
            try
            {
                AssemblerLexer lex = new AssemblerLexer(input);
                CommonTokenStream tokens = new CommonTokenStream(lex);
                AssemblerParser p = new AssemblerParser(tokens);
                BytecodeGenerator gen = new BytecodeGenerator(Defaults.SystemMethods.Values);
                
                p.SetGenerator(gen);
                p.TraceDestination = _traceDestination;
                p.program();

                if (p.NumberOfSyntaxErrors > 0 && _listener != null)
                {
                    _listener.Error(Convert.ToString(p.NumberOfSyntaxErrors) + " syntax error(s)");
                    return;
                }

                _result = gen.Result;
            }
            catch (GenerationException ex)
            {
                _listener.Error(ex.Message);
            }
        }
    }
}
