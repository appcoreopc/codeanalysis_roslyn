using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace C3.CodeAnalysis.Net.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public abstract class BaseAnalyzer : DiagnosticAnalyzer
    {
        public abstract string DiagnosticId { get; set; }
        public abstract string Title { get; set; }
        public abstract string MessageFormat { get; set; }
        public abstract string Description { get; set; }
        public abstract string Category { get; set; }      
    }
}
