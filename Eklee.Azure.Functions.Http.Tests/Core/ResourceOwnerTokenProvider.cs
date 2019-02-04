namespace Eklee.Azure.Functions.Http.Tests.Core
{
	public class ResourceOwnerTokenProvider
	{
		private readonly LocalSettings _localSettings1;
		private readonly LocalSettings _localSettings2;
		private readonly LocalSettings _localSettings3;

		public LocalSettings LocalSettings1 => _localSettings1;
		public LocalSettings LocalSettings2 => _localSettings2;
		public LocalSettings LocalSettings3 => _localSettings3;


		public ResourceOwnerTokenProvider()
		{
			_localSettings1 = this.Load("local1.json");
			_localSettings2 = this.Load("local2.json");
			_localSettings3 = this.Load("local3.json");
		}
	}
}
