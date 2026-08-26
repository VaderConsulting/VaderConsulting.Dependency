using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace VaderConsulting.Dependency
{
    /// <summary>
    /// Interface to define objects which can be topologically sorted.
    /// </summary>
    /// <typeparam name="T">Type of underlying object.</typeparam>
    public interface ITopologicSortable<T>
    {
        /// <summary>
        /// Unique identifer of the item.
        /// </summary>
        Guid Identifier { get; }

        /// <summary>
        /// Gets or sets a list of object the current item depends on.
        /// </summary>
        ObservableCollection<T> DependsOn { get; set; }
    }
}
