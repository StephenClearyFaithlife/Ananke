using System;
using System.Collections.Generic;
using System.Text;
using Faithlife.Ananke.Services;

namespace Faithlife.Ananke.Tests.Util
{
    public sealed class StubSignalService : ISignalService
    {
	    public void Invoke()
	    {
			m_handler?.Invoke();
	    }

	    public void AddHandler(Action handler)
	    {
		    m_handler += handler;
	    }

	    private Action m_handler;
    }
}
