using System;
using Yang_Framework.Defines.Data.Database;

namespace Yang_Framework
{
	public interface DBInterface
	{
		bool Connect();
		bool Disconnect();
        void fetch_assoc(SQLResult result);
	}
}
