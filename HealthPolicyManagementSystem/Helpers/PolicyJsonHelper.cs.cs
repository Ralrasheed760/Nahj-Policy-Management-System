using HealthPolicyManagementSystem.ViewModels;
using System.Text.Json;

namespace HealthPolicyManagementSystem.Helpers
{
    public static class PolicyJsonHelper
    {
        public static string ConvertToJson(PolicyViewModel policy)
        {
            return JsonSerializer.Serialize(policy, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        public static PolicyViewModel? ConvertFromJson(string json)
        {
            return JsonSerializer.Deserialize<PolicyViewModel>(json);
        }
    }
}
