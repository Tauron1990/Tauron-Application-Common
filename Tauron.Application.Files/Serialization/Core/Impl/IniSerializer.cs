using System;
using Tauron.Application.Files.Serialization.Core.Impl.Mapper.Ini;
using Tauron.Application.Files.Serialization.Core.Managment;

namespace Tauron.Application.Files.Serialization.Core.Impl
{
    internal class IniSerializer : SerializerBase<IniContext>
    {
        public IniSerializer(ObjectBuilder builder, SimpleMapper<IniContext> mapper) 
            : base(builder, mapper, ContextMode.Text)
        {
        }

        public override IniContext BuildContext(SerializationContext context)
        {
            return new IniContext(context);
        }

        public override void CleanUp(IniContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            context.Dispose();
        }
    }
}
