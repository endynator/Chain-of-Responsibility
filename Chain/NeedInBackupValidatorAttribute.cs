using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnet.Chain
{
    [AttributeUsage(
        AttributeTargets.Class |
        AttributeTargets.Interface |
        AttributeTargets.GenericParameter, Inherited = true)]
    public class NeedInBackupValidatorAttribute(int postErrorRollbackCount) : Attribute
    {
        public int PostErrorRollbackCount { get; set; } = postErrorRollbackCount;
        public int ProcessCounter { get; protected internal set; }

        public bool IsReadyForBackup(bool isDataNull = false)
        {
            if (PostErrorRollbackCount <= -1) return isDataNull;
            return PostErrorRollbackCount % ProcessCounter == 0;
        }
    }
}
