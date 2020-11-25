using System;
using System.Collections.ObjectModel;

namespace FileCabinetApp
{
    /// <summary>
    /// Properties for search command handlers.
    /// </summary>
    public class SearchProperties
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchProperties"/> class.
        /// </summary>
        /// <param name="list">List of properties.</param>
        /// <param name="signs">List of signs of properties.</param>
        public SearchProperties(ReadOnlyCollection<Tuple<string, object>> list, ReadOnlyCollection<string> signs)
        {
            this.List = list ?? throw new ArgumentNullException(nameof(list));
            this.Signs = signs ?? throw new ArgumentNullException(nameof(signs));
        }

        /// <summary>
        /// Gets list of properties.
        /// </summary>
        /// <value>
        /// List of properties.
        /// </value>
        public ReadOnlyCollection<Tuple<string, object>> List { get; private set; }

        /// <summary>
        /// Gets list of signs of properties.
        /// </summary>
        /// <value>
        /// List of signs of properties.
        /// </value>
        public ReadOnlyCollection<string> Signs { get; private set; }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var any = obj as SearchProperties;
            if (any is null || any.Signs.Count != this.Signs.Count || any.List.Count != this.List.Count)
            {
                return false;
            }

            for (int current = 0; current < any.Signs.Count; current++)
            {
                if (!this.Signs[current].Equals(any.Signs[current], StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
            }

            for (int i = 0; i < this.List.Count; i++)
            {
                if (!this.List[i].Item1.Equals(any.List[i].Item1, StringComparison.CurrentCultureIgnoreCase))
                {
                    return false;
                }

                Type type = this.List[i].Item2.GetType();
                Type typeAny = any.List[i].Item2.GetType();
                if (type != typeAny)
                {
                    return false;
                }

                switch (this.List[i].Item2)
                {
                    case string str:
                        if (this.List[i].Item2 as string != any.List[i].Item2 as string)
                        {
                            return false;
                        }

                        break;
                    case int num:
                        if ((int)this.List[i].Item2 != (int)any.List[i].Item2)
                        {
                            return false;
                        }

                        break;
                    case char ch:
                        if ((char)this.List[i].Item2 != (char)any.List[i].Item2)
                        {
                            return false;
                        }

                        break;
                    case DateTime dat:
                        if ((DateTime)this.List[i].Item2 != (DateTime)any.List[i].Item2)
                        {
                            return false;
                        }

                        break;
                    case decimal dec:
                        if ((decimal)this.List[i].Item2 != (decimal)any.List[i].Item2)
                        {
                            return false;
                        }

                        break;
                    case short sh:
                        if ((short)this.List[i].Item2 != (short)any.List[i].Item2)
                        {
                            return false;
                        }

                        break;
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.List, this.Signs);
        }
    }
}
