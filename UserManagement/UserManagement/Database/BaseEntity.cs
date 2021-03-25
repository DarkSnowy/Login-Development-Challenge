using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserManagement.Database
{
    /// <summary>
    /// This provides a common base for the entities so that any entities extending this can have these valued accessed and updated by the database context.
    /// </summary>
    public interface IBaseEntity
    {
        /// <summary>
        /// Date and time a database record was last modified.
        /// </summary>
        public DateTime Modified { get; set; }

        /// <summary>
        /// Date and time a database record was first created.
        /// </summary>
        public DateTime Created { get; set; }
    }
}
