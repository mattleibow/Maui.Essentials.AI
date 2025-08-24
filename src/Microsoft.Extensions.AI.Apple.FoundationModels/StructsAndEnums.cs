namespace Microsoft.Extensions.AI.Apple.FoundationModels
{
	public enum SystemLanguageModelAvailability
	{
		Available = 0,
		UnavailableAppleIntelligenceNotEnabled = 1,
		UnavailableDeviceNotEligible = 2,
		UnavailableModelNotReady = 3,
		Unavailable = 4
	}
}
