// The file ProxyService.cs is part of Tauron.Application.Common.
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
// <copyright file="ProxyService.cs" company="Tauron Parallel Works">
//   Tauron Application © 2013
// </copyright>
// <summary>
//   The proxy service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Tauron.Application.Ioc.BuildUp.Exports;

#endregion

namespace Tauron.Application.Ioc.BuildUp.Strategy.DafaultStrategys
{
    /// <summary>The proxy service.</summary>
    public sealed class ProxyService : IProxyService
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProxyService" /> class.
        ///     Initialisiert eine neue Instanz der <see cref="ProxyService" /> Klasse.
        ///     Initializes a new instance of the <see cref="ProxyService" /> class.
        /// </summary>
        public ProxyService()
        {
            GenericGenerator = new ProxyGenerator {Logger = new PrivateLogger()};
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the generator.</summary>
        /// <value>The generator.</value>
        public ProxyGenerator GenericGenerator { get; private set; }

        #endregion

        /// <summary>The private logger.</summary>
        private class PrivateLogger : LevelFilteredLogger
        {
            #region Public Methods and Operators

            /// <summary>
            ///     The create child logger.
            /// </summary>
            /// <param name="loggerName">
            ///     The logger name.
            /// </param>
            /// <returns>
            ///     The <see cref="ILogger" />.
            /// </returns>
            public override ILogger CreateChildLogger(string loggerName)
            {
                return new PrivateLogger();
            }

            #endregion

            #region Methods

            /// <summary>
            ///     The log.
            /// </summary>
            /// <param name="loggerLevel">
            ///     The logger level.
            /// </param>
            /// <param name="loggerName">
            ///     The logger name.
            /// </param>
            /// <param name="message">
            ///     The message.
            /// </param>
            /// <param name="exception">
            ///     The exception.
            /// </param>
            protected override void Log(LoggerLevel loggerLevel, string loggerName, string message, Exception exception)
            {
                TraceEventType _type;

                switch (loggerLevel)
                {
                    case LoggerLevel.Debug:
                        _type = TraceEventType.Verbose;
                        break;
                    case LoggerLevel.Info:
                        _type = TraceEventType.Information;
                        break;
                    case LoggerLevel.Warn:
                        _type = TraceEventType.Warning;
                        break;
                    case LoggerLevel.Error:
                        _type = TraceEventType.Error;
                        break;
                    case LoggerLevel.Fatal:
                        _type = TraceEventType.Critical;
                        break;
                    default:
                        _type = TraceEventType.Verbose;
                        break;
                }

                Dictionary<string, object> temp = exception != null
                                                      ? new Dictionary<string, object> {{"Exception", exception}}
                                                      : null;

                if (Logger.IsLoggingEnabled()) Logger.Write(message, "Proxy Generation", -1, -1, _type, string.Empty, temp);
            }

            #endregion
        }

        public ProxyGenerator Generate(ExportMetadata metadata, ImportMetadata[] imports, out IImportInterceptor interceptor)
        {
            interceptor = null;

            return GenericGenerator;
        }
    }
}