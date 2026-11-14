using System;
using System.Collections.Concurrent;
using SafeExamBrowser.Client.Contracts;

namespace SafeExamBrowser.Client;

internal class Coordinator : ICoordinator
{
	private readonly ConcurrentBag<Guid> reconfiguration;

	private readonly ConcurrentBag<Guid> session;

	internal Coordinator()
	{
		reconfiguration = new ConcurrentBag<Guid>();
		session = new ConcurrentBag<Guid>();
	}

	public bool IsReconfigurationLocked()
	{
		return !reconfiguration.IsEmpty;
	}

	public bool IsSessionLocked()
	{
		return !session.IsEmpty;
	}

	public void ReleaseReconfigurationLock()
	{
		reconfiguration.TryTake(out var _);
	}

	public void ReleaseSessionLock()
	{
		session.TryTake(out var _);
	}

	public bool RequestReconfigurationLock()
	{
		bool result = false;
		lock (reconfiguration)
		{
			if (reconfiguration.IsEmpty)
			{
				reconfiguration.Add(Guid.NewGuid());
				result = true;
			}
		}
		return result;
	}

	public bool RequestSessionLock()
	{
		bool result = false;
		lock (session)
		{
			if (session.IsEmpty)
			{
				session.Add(Guid.NewGuid());
				result = true;
			}
		}
		return result;
	}
}
