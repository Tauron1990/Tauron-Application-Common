using System;
using System.Runtime.Serialization;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.Files.Serialization
{
    [Serializable]
    public class SerializerElementNullException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public SerializerElementNullException()
        {
        }

        public SerializerElementNullException([NotNull] string message) : base(message)
        {
        }

        public SerializerElementNullException([NotNull] string message, [NotNull] Exception inner) : base(message, inner)
        {
        }

        protected SerializerElementNullException([NotNull] SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
