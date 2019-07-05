using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using CommandDotNet.Parsing;

namespace CommandDotNet.TypeDescriptors
{
    public class ArgumentTypeDescriptors
    {
        private readonly List<IArgumentTypeDescriptor> _customDescriptors = new List<IArgumentTypeDescriptor>();

        private readonly List<IArgumentTypeDescriptor> _defaultDescriptors = new List<IArgumentTypeDescriptor>
            {
                new BoolTypeDescriptor(),
                new EnumTypeDescriptor(),

                new DelegatedTypeDescriptor<string>(Constants.TypeDisplayNames.Text, (a,v) => v),
                new DelegatedTypeDescriptor<char>(Constants.TypeDisplayNames.Character, (a,v) => char.Parse(v)),

                new DelegatedTypeDescriptor<long>(Constants.TypeDisplayNames.Number, (a, v) => long.Parse(v, CultureInfo.InvariantCulture)),
                new DelegatedTypeDescriptor<int>(Constants.TypeDisplayNames.Number, (a, v) => int.Parse(v, CultureInfo.InvariantCulture)),
                new DelegatedTypeDescriptor<short>(Constants.TypeDisplayNames.Number, (a, v) => short.Parse(v, CultureInfo.InvariantCulture)),
                new DelegatedTypeDescriptor<decimal>(Constants.TypeDisplayNames.DecimalNumber, (a, v) => decimal.Parse(v, CultureInfo.InvariantCulture)),
                new DelegatedTypeDescriptor<double>(Constants.TypeDisplayNames.DoubleNumber, (a, v) => double.Parse(v, CultureInfo.InvariantCulture)),

                new ComponentModelTypeDescriptor(),
                new StringCtorTypeDescriptor()
            }
            .Select(d => new ErrorReportingDescriptor(d))
            .Cast<IArgumentTypeDescriptor>()
            .ToList();

        public ArgumentTypeDescriptors()
        {
        }

        internal ArgumentTypeDescriptors(ArgumentTypeDescriptors origin)
        {
            _customDescriptors = new List<IArgumentTypeDescriptor>(origin._customDescriptors);
            _defaultDescriptors = new List<IArgumentTypeDescriptor>(origin._defaultDescriptors);
        }

        public void Add(IArgumentTypeDescriptor argumentTypeDescriptor)
        {
            _customDescriptors.Add(new ErrorReportingDescriptor(argumentTypeDescriptor));
        }

        public IArgumentTypeDescriptor GetDescriptor(Type type)
        {
            return _customDescriptors.FirstOrDefault(d => d.CanSupport(type))
                   ?? _defaultDescriptors.FirstOrDefault(d => d.CanSupport(type));
        }

        public IArgumentTypeDescriptor GetDescriptorOrThrow(Type type)
        {
            return GetDescriptor(type) ?? throw new ValueParsingException(
                       $"type : {type} is not supported. If it's an argument model, " +
                       $"inherit from {nameof(IArgumentModel)}, otherwise " +
                       $"implement a {nameof(TypeConverter)} or {nameof(IArgumentTypeDescriptor)} " +
                       "to support this type.");
        }
    }
}