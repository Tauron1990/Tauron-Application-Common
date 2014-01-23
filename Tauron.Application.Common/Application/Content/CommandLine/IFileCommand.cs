// The file IFileCommand.cs is part of Tauron.Application.Common.
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

namespace Tauron.Application
{
    /// <summary>The FileCommand interface.</summary>
    public interface IFileCommand
    {
        #region Public Methods and Operators

        /// <summary>
        ///     The process file.
        /// </summary>
        /// <param name="file">
        ///     The file.
        /// </param>
        void ProcessFile(string file);

        /// <summary>The provide factory.</summary>
        /// <returns>
        ///     The <see cref="IShellFactory" />.
        /// </returns>
        IShellFactory ProvideFactory();

        #endregion
    }
}