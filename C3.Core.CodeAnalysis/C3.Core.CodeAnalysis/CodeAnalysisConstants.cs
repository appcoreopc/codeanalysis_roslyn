using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("C3.Core.CodeAnalysis.Test")]

namespace C3.Core.CodeAnalysis
{
    class CodeAnalysisConstants
    {
        public const string DateTimeOffsetConstant = "DateTimeOffset";
        public const string DateTimeDataTypeString = "datetime";

        public const string DiagnosticIdDateTimeOffset = "COR0001";
        public const string DiagnosticIdDateTimeUseOnMethodOffset = "COR0001B";
        public const string DiagnosticIdInterfaceDoc = "COR0002";
        public const string DiagnosticIdMethodInterfaceDoc = "COR0003";

    }
}
