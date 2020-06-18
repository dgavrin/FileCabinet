using System;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Represents parameter type.
    /// </summary>
    public enum TypeOfParameter
    {
        /// <summary>
        /// No parameter representation.
        /// </summary>
        NotParameter = 0,

        /// <summary>
        /// Represents the full parameter.
        /// </summary>
        FullParameter = 1,

        /// <summary>
        /// Represents a short parameter.
        /// </summary>
        ShortParameter = 2,
    }
}
