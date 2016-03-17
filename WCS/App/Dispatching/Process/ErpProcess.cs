using System;
using System.Collections.Generic;
using System.Text;
using MCP;
using System.Data;
using Util;
 

namespace App.Dispatching.Process
{
    public class ErpProcess : AbstractProcess
    {
        protected override void StateChanged(StateItem stateItem, IProcessDispatcher dispatcher)
        {
            string str = stateItem.State.ToString();
        }
    }
}
