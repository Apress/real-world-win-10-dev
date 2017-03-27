using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;

namespace BigMountainX
{
[ServiceContract]
public interface IProfileGenerator
{
    [OperationContract]
    List<string> GetMailingList(string file_name);
}
}
