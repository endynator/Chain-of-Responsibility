using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnet.Chain
{
    public abstract class VoidHandler: BaseHandler
    {
        public VoidHandler(VoidHandler? handler = null)
        {
            Next = handler;
            Type type = this.GetType();
            ValidatorAttribute =
                Attribute.GetCustomAttribute(type, typeof(NeedInBackupValidatorAttribute)) as
                NeedInBackupValidatorAttribute ?? throw new NullReferenceException("Attribute wasn't found");
        }

    }
}
