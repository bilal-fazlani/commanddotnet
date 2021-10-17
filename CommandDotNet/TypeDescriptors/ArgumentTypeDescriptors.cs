using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CommandDotNet.Extensions;

namespace CommandDotNet.TypeDescriptors
{
    public class ArgumentTypeDescriptors: IEnumerable<IArgumentTypeDescriptor>, IIndentableToString
    {
        private readonly List<IArgumentTypeDescriptor> _customDescriptors = new List<IArgumentTypeDescriptor>();

        private readonly List<IArgumentTypeDescriptor> _defaultDescriptors = new List<IArgumentTypeDescriptor>
            {
                new BoolTypeDescriptor(),
                new EnumTypeDescriptor(),

                new DelegatedTypeDescriptor<string>(Resources.A.Type_Text, v => v),
                new DelegatedTypeDescriptor<Password>(Resources.A.Type_Text, v => new Password(v)),
                new DelegatedTypeDescriptor<char>(Resources.A.Type_Character, v => char.Parse(v)),

                new DelegatedTypeDescriptor<long>(Resources.A.Type_Number, v => long.Parse(v, CultureInfo.InvariantCulture)),
                new DelegatedTypeDescriptor<int>(Resources.A.Type_Number, v => int.Parse(v, CultureInfo.InvariantCulture)),
                new DelegatedTypeDescriptor<short>(Resources.A.Type_Number, v => short.Parse(v, CultureInfo.InvariantCulture)),
                new DelegatedTypeDescriptor<decimal>(Resources.A.Type_Decimal, v => decimal.Parse(v, CultureInfo.InvariantCulture)),
                new DelegatedTypeDescriptor<double>(Resources.A.Type_Double, v => double.Parse(v, CultureInfo.InvariantCulture)),

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
        }

        /// <summary>Registers your customer descriptor</summary>
        public void Add(IArgumentTypeDescriptor argumentTypeDescriptor)
        {
            _customDescriptors.Add(new ErrorReportingDescriptor(argumentTypeDescriptor));
        }

        /// <summary>.
        /// Returns the first descriptor that supports this type. Returns null if no descriptor found.<br/>
        /// Custom descriptors are evaluated before built-in descriptors
        /// </summary>
        public IArgumentTypeDescriptor GetDescriptor(Type type)
        {
            return _customDescriptors.FirstOrDefault(d => d.CanSupport(type))
                   ?? _defaultDescriptors.FirstOrDefault(d => d.CanSupport(type));
        }

        /// <summary>.
        /// Returns the first descriptor that supports this type. Throws AppRunnerException if no descriptor found.<br/>
        /// Custom descriptors are evaluated before built-in descriptors
        /// </summary>
        public IArgumentTypeDescriptor GetDescriptorOrThrow(Type type)
        {
            return GetDescriptor(type) ?? throw new InvalidConfigurationException(
                Resources.A.Error_Type_is_not_supported_as_argument(type.FullName));
        }

        public IEnumerator<IArgumentTypeDescriptor> GetEnumerator()
        {
            return _defaultDescriptors.Concat(_customDescriptors).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return ToString(new Indent());
        }

        public string ToString(Indent indent)
        {
            var descriptors = _defaultDescriptors
                .Concat(_customDescriptors)
                .Select(d => $"{indent}{d}")
                .ToCsv(Environment.NewLine);
            return $"{nameof(ArgumentTypeDescriptors)}:{Environment.NewLine}{descriptors}";
        }
    }
}