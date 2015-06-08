using MewPipe.Logic.Repositories;

namespace MewPipe.DataFeeder.Utils
{
	public class UnitOfWorkSingleton
	{
		private static UnitOfWork _instance;

		private UnitOfWorkSingleton()
		{
		}

		public static UnitOfWork GetInstance()
		{
			return _instance ?? (_instance = new UnitOfWork());
		}
	}
}