namespace Eklee.Azure.Functions.Http
{
	public interface IJwtTokenValidatorParameters
	{
		string Audience { get; }
		string[] Issuers { get; }
	}
}
