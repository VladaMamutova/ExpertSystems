using System;

namespace SearchWithVariables.Model
{
    /// <summary>
    /// Аргумент: переменная или константа.
    /// </summary>
    class Argument
    {
        public string Value { get; }
        
        public Argument(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"The argument \"{value}\" is invalid, " +
                                            "it must consist of at least one character");
            }

            Value = value;
        }

        public bool IsConst() => char.IsUpper(Value[0]); // константа - с большой буквы

        public override bool Equals(object obj)
        {
            return obj is Argument argument && Value.Equals(argument.Value);
        }

        public override int GetHashCode() => Value.GetHashCode();
        
        public override string ToString()
        {
            return Value;
        }
    }
}
