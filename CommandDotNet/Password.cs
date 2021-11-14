using System;

namespace CommandDotNet
{
    /// <summary>
    /// <see cref="Password"/> will capture input and only make it
    /// available through a method to prevent displaying the data
    /// via logging tools that use <see cref="ToString"/> or
    /// reflect properties.<br/>
    /// <see cref="Password"/> indicates intent to middleware
    /// that may capture data, i.e. Prompting for missing arguments.
    /// </summary>
    public class Password
    {
        public static readonly string ValueReplacement = "*****";

        private readonly string _password;

        public Password(string password)
        {
            _password = password ?? throw new ArgumentNullException(nameof(password));
        }


        public string GetPassword() => _password;

        public override string ToString()
        {
            return _password.IsNullOrEmpty() ? "" : ValueReplacement;
        }

        protected bool Equals(Password other)
        {
            return _password == other._password;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((Password) obj);
        }

        public override int GetHashCode()
        {
            return _password.GetHashCode();
        }
    }
}