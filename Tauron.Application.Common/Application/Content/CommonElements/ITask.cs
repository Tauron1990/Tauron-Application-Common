// The file ITask.cs is part of Tauron.Application.Common.
// 
// CoreEngine is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// CoreEngine is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//  
// You should have received a copy of the GNU General Public License
//  along with Tauron.Application.Common If not, see <http://www.gnu.org/licenses/>.

#region

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITask.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The Task interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Threading.Tasks;

#endregion

namespace Tauron.Application
{
    /// <summary>The Task interface.</summary>
    public interface ITask
    {
        #region Public Properties

        /// <summary>Gets a value indicating whether synchronize.</summary>
        /// <value>The synchronize.</value>
        bool Synchronize { get; }

        /// <summary>Gets the task.</summary>
        Task Task { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The execute.</summary>
        void Execute();

        #endregion
    }
}